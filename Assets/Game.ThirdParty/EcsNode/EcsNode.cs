using System;
using System.Collections.Generic;
using System.Reflection;

namespace ECS
{
    public class SystemInfo
    {
        public object System { get; set; }
        public MethodInfo Action { get; set; }
    }

    public class EcsNode : EcsEntity
    {
        public int IdIndex { get; private set; }
        public long IdBaseTime { get; private set; }
        public ushort EcsIndex { get; private set; }

        public const int Mask14bit = 0x3fff;
        public const int Mask30bit = 0x3fffffff;
        public const int Mask20bit = 0xfffff;

        public EcsNode(ushort ecsIndex)
        {
            this.EcsIndex = ecsIndex;
            this.IdBaseTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / 10000;
        }

        public long NewInstanceId()
        {
            uint timeAddition = (uint)((DateTime.UtcNow.Ticks / 10000 - this.IdBaseTime) / 1000);
            int v = 0;
            lock (this)
            {
                if (++IdIndex > Mask20bit - 1)
                {
                    IdIndex = 0;
                }
                v = IdIndex;
            }

            ulong result = 0;
            result |= EcsIndex;
            result <<= 30;
            result |= timeAddition;
            result <<= 20;
            result |= (uint)IdIndex;
            return (long)result;
        }

        public Dictionary<long, EcsEntity> AllEntities { get; set; } = new();
        public Dictionary<Type, List<EcsEntity>> Type2Entities { get; set; } = new();

        public Dictionary<Type, IEcsSystem> AllSystems { get; set; } = new();
        public Dictionary<Type, List<IEcsSystem>> EntityType2Systems { get; set; } = new();
        public Dictionary<Type, Dictionary<Type, List<SystemInfo>>> AllEntitySystems { get; set; } = new();
        public Dictionary<(Type, Type), Dictionary<string, SystemInfo>> AllEntityComponentSystems { get; set; } = new();
        public Dictionary<Type, List<SystemInfo>> AllUpdateSystems { get; set; } = new();
        public Dictionary<Type, List<SystemInfo>> AllFixedUpdateSystems { get; set; } = new();
        public List<Type> DriveTypes { get; set; } = new();
        public Type[] AllTypes { get; set; }

        public void AddEntity(EcsEntity entity)
        {
            var ecsNode = entity.EcsNode;
            ecsNode.AllEntities.Add(entity.Id, entity);
            var entityType = entity.GetType();
            if (!ecsNode.Type2Entities.TryGetValue(entityType, out var list))
            {
                list = new List<EcsEntity>();
                ecsNode.Type2Entities.Add(entityType, list);
            }
            list.Add(entity);
            if (ecsNode.UpdateEntityTypes.Contains(entityType))
            {
                ecsNode.AddEntities.Enqueue(entity);
            }

            entity.AddComponent<EntityObjectComponent>();
        }

        public void RemoveEntity(EcsEntity entity)
        {
            var ecsNode = entity.EcsNode;
            ecsNode.AllEntities.Remove(entity.Id);
            var entityType = entity.GetType();
            if (ecsNode.Type2Entities.TryGetValue(entityType, out var list))
            {
                list.Remove(entity);
                if (ecsNode.UpdateEntityTypes.Contains(entityType))
                {
                    ecsNode.RemoveEntities.Enqueue(entity);
                }
            }
        }

        public void RegisterDrive<T>()
        {
            DriveTypes.Add(typeof(T));
        }

        public void AddSystems(Type[] types)
        {
            AllTypes = types;

            var allSystems = new Dictionary<Type, IEcsSystem>();
            var entityType2Systems = new Dictionary<Type, List<IEcsSystem>>();
            var allEntitySystems = new Dictionary<Type, Dictionary<Type, List<SystemInfo>>>();
            var allEntityComponentSystems = new Dictionary<(Type, Type), Dictionary<string, SystemInfo>>();
            var allUpdateSystems = new Dictionary<Type, List<SystemInfo>>();
            var allFixedUpdateSystems = new Dictionary<Type, List<SystemInfo>>();
            var updateEntityTypes = new List<Type>();

            foreach (var systemType in types)
            {
                if (systemType.BaseType == null)
                {
                    continue;
                }
                if (systemType.BaseType.BaseType == null)
                {
                    continue;
                }
                if (!systemType.BaseType.BaseType.IsAssignableFrom(typeof(IEcsSystem)))
                {
                    continue;
                }
                if (systemType.ContainsGenericParameters)
                {
                    continue;
                }

                var system = Activator.CreateInstance(systemType) as IEcsSystem;
                allSystems.Add(systemType, system);

                if (system is IEcsEntitySystem ecsEntitySystem)
                {
                    var entityType = ecsEntitySystem.EntityType;

                    if (!entityType2Systems.TryGetValue(entityType, out var ecsSystems))
                    {
                        ecsSystems = new List<IEcsSystem>();
                        entityType2Systems.Add(entityType, ecsSystems);
                    }
                    ecsSystems.Add(system);

                    if (!allEntitySystems.TryGetValue(entityType, out var typeSystems))
                    {
                        typeSystems = new Dictionary<Type, List<SystemInfo>>();
                        allEntitySystems.Add(entityType, typeSystems);
                    }

                    var interfaces = systemType.GetInterfaces();
                    foreach (var interfaci in interfaces)
                    {
                        foreach (var item in DriveTypes)
                        {
                            if (interfaci.IsAssignableFrom(item))
                            {
                                var arr = item.Name.ToCharArray();
                                var methodName = string.Empty;
                                for (int i = 1; i < arr.Length; i++)
                                {
                                    methodName += arr[i];
                                }
                                var systemAction = systemType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
                                var systemInfo = new SystemInfo() { System = system, Action = systemAction };
                                if (typeSystems.ContainsKey(item) == false)
                                {
                                    typeSystems.Add(item, new List<SystemInfo>());
                                }
                                typeSystems[item].Add(systemInfo);

                                if (item == typeof(IUpdate))
                                {
                                    if (allUpdateSystems.ContainsKey(entityType) == false)
                                    {
                                        allUpdateSystems.Add(entityType, new List<SystemInfo>());
                                    }
                                    allUpdateSystems[entityType].Add(systemInfo);
                                    updateEntityTypes.Add(entityType);
                                }

                                if (item == typeof(IFixedUpdate))
                                {
                                    if (allFixedUpdateSystems.ContainsKey(entityType) == false)
                                    {
                                        allFixedUpdateSystems.Add(entityType, new List<SystemInfo>());
                                    }
                                    allFixedUpdateSystems[entityType].Add(systemInfo);

                                    if (updateEntityTypes.Contains(entityType) == false)
                                    {
                                        updateEntityTypes.Add(entityType);
                                    }
                                }

                                break;
                            }
                        }
                    }
                }

                if (system is IEcsComponentSystem ecsEntitySystem2)
                {
                    var entityType = ecsEntitySystem2.EntityType;

                    if (!entityType2Systems.TryGetValue(entityType, out var ecsSystems))
                    {
                        ecsSystems = new List<IEcsSystem>();
                        entityType2Systems.Add(entityType, ecsSystems);
                    }
                    ecsSystems.Add(system);

                    var componentType = ecsEntitySystem2.ComponentType;
                    var tuple = (entityType, componentType);
                    allEntityComponentSystems.TryGetValue(tuple, out var pairs);
                    if (pairs == null)
                    {
                        pairs = new Dictionary<string, SystemInfo>();
                        allEntityComponentSystems.Add(tuple, pairs);
                    }

                    var interfaces = systemType.GetInterfaces();
                    foreach (var interfaci in interfaces)
                    {
                        foreach (var item in DriveTypes)
                        {
                            if (interfaci.IsAssignableFrom(item))
                            {
                                var arr = item.Name.ToCharArray();
                                var methodName = string.Empty;
                                for (int i = 1; i < arr.Length; i++)
                                {
                                    methodName += arr[i];
                                }
                                var systemAction = systemType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
                                var systemInfo = new SystemInfo() { System = system, Action = systemAction };
                                pairs.Add($"{item.Name}_{systemType.Name}", systemInfo);
                                break;
                            }
                        }
                    }
                }
            }

            AllSystems = allSystems;
            EntityType2Systems = entityType2Systems;
            AllEntitySystems = allEntitySystems;
            AllEntityComponentSystems = allEntityComponentSystems;
            AllUpdateSystems = allUpdateSystems;
            AllFixedUpdateSystems = allFixedUpdateSystems;
            UpdateEntityTypes = updateEntityTypes;
        }

        public T GetSystem<T>() where T : class, IEcsSystem, new()
        {
            var systemType = typeof(T);
            AllSystems.TryGetValue(systemType, out var system);
            return system as T;
        }

        public void DriveEntitySystems(EcsEntity entity, Type entityType, Type driveType, object[] args)
        {
            AllEntitySystems.TryGetValue(entityType, out var systems);
            if (systems == null)
            {
                return;
            }
            foreach (var item in systems)
            {
                if (item.Key.IsAssignableFrom(driveType))
                {
                    var systemList = item.Value;
                    foreach (var systemInfo in systemList)
                    {
                        var system = systemInfo.System;
                        var method = systemInfo.Action;
                        method.Invoke(system, args);
                    }
                }
            }
        }

        public void DriveEntitySystems<T1>(T1 entity, Type driveType, object[] args) where T1 : EcsEntity
        {
            DriveEntitySystems(entity, typeof(EcsEntity), driveType, args);
            DriveEntitySystems(entity, entity.GetType(), driveType, args);
        }

        public void DriveComponentSystems<T1, T2>(T1 entity, T2 component, Type entityType, Type driveType) where T1 : EcsEntity where T2 : EcsComponent
        {
            AllEntityComponentSystems.TryGetValue((entityType, component.GetType()), out var systems);
            if (systems == null)
            {
                return;
            }
            foreach (var item in systems)
            {
                if (item.Key.StartsWith(driveType.Name))
                {
                    var systemInfo = item.Value;
                    var system = systemInfo.System;
                    var method = systemInfo.Action;
                    method.Invoke(system, new object[] { entity, component });
                }
            }
        }

        public void DriveComponentSystems<T1, T2>(T1 entity, T2 component, Type driveType) where T1 : EcsEntity where T2 : EcsComponent
        {
            DriveComponentSystems(entity, component, typeof(EcsEntity), driveType);
            DriveComponentSystems(entity, component, entity.GetType(), driveType);
        }

        public List<Type> UpdateEntityTypes { get; set; } = new();
        public Dictionary<Type, List<EcsEntity>> UpdateEntities { get; set; } = new();
        public Queue<EcsEntity> AddEntities { get; set; } = new();
        public Queue<EcsEntity> RemoveEntities { get; set; } = new();

        public void DriveEntityUpdate()
        {
            while (AddEntities.Count > 0)
            {
                var entity = AddEntities.Dequeue();
                var entityType = entity.GetType();
                if (!UpdateEntities.TryGetValue(entityType, out var list))
                {
                    list = new List<EcsEntity>();
                    UpdateEntities.Add(entityType, list);
                }
                list.Add(entity);
            }

            while (RemoveEntities.Count > 0)
            {
                var entity = RemoveEntities.Dequeue();
                var entityType = entity.GetType();
                if (UpdateEntities.TryGetValue(entityType, out var list))
                {
                    list.Remove(entity);
                }
            }

            var systems = AllUpdateSystems;
            foreach (var item in systems)
            {
                var entityType = item.Key;
                if (entityType == typeof(EcsNode))
                {
                    var systemList = item.Value;
                    foreach (var systemInfo in systemList)
                    {
                        var system = systemInfo.System;
                        var method = systemInfo.Action;
                        if (this.IsDisposed) continue;
                        method.Invoke(system, new object[] { this });
                    }
                    continue;
                }

                if (UpdateEntities.TryGetValue(entityType, out var entities))
                {
                    var systemList = item.Value;
                    foreach (var systemInfo in systemList)
                    {
                        var system = systemInfo.System;
                        var method = systemInfo.Action;
                        foreach (var entity in entities)
                        {
                            if (entity.IsDisposed) continue;
                            method.Invoke(system, new object[] { entity });
                        }
                    }
                }
            }
        }

        public void DriveEntityFixedUpdate()
        {
            var systems = AllFixedUpdateSystems;
            foreach (var item in systems)
            {
                var entityType = item.Key;
                if (entityType == typeof(EcsNode))
                {
                    var systemList = item.Value;
                    foreach (var systemInfo in systemList)
                    {
                        var system = systemInfo.System;
                        var method = systemInfo.Action;
                        if (this.IsDisposed) continue;
                        method.Invoke(system, new object[] { this });
                    }
                    continue;
                }

                if (UpdateEntities.TryGetValue(entityType, out var entities))
                {
                    var systemList = item.Value;
                    foreach (var systemInfo in systemList)
                    {
                        var system = systemInfo.System;
                        var method = systemInfo.Action;
                        foreach (var entity in entities)
                        {
                            if (entity.IsDisposed) continue;
                            method.Invoke(system, new object[] { entity });
                        }
                    }
                }
            }
        }
    }
}