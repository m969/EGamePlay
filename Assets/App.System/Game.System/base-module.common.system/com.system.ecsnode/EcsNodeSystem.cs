using ECS;
using ECSUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ECSGame
{
    public class EcsNodeSystem : AEntitySystem<EcsNode>, IUpdate<EcsNode>, IDestroy<EcsNode>, IAwake<EcsNode>
    {
        public void Awake(EcsNode entity)
        {
            ConsoleLog.Debug($"EcsNodeSystem Awake: {entity.GetType().Name}, Id: {entity.Id}, InstanceId: {entity.InstanceId}");
        }

        public void Update(EcsNode ecsNode)
        {
            if (ecsNode.GetComponent<TimerComponent>() is { } timerComponent)
            {
                TimerSystem.Update(ecsNode, timerComponent);
            }
            //EventSystem.Update(ecsNode);
        }

        public void Destroy(EcsNode entity)
        {
            EventBus.Instance.RemoveEcs(entity);
        }

        public static T Create<T>(ushort nodeIndex, Assembly systemAssembly) where T : EcsNode
        {
            var ecsNode = (T)Activator.CreateInstance(typeof(T), new object[] { nodeIndex });
            ecsNode.Id = ecsNode.NewEntityId();
            ecsNode.InstanceId = ecsNode.NewInstanceId();
            ecsNode.RegisterDrives(typeof(EcsNode).Assembly.GetTypes());

            RegisterSystems(ecsNode, systemAssembly);

            // ecsNode.DriveEntitySystems(ecsNode, typeof(IAwake));

            EventBus.Instance.AddEcs(ecsNode);

            ecsNode.AddComponent<TimerComponent>();
            ecsNode.AddComponent<ReloadComponent>();
            ecsNode.GetComponent<ReloadComponent>().SystemAssembly = systemAssembly;

            return ecsNode;
        }

        public static EcsNode RegisterSystems(EcsNode ecsNode, Assembly systemAssembly)
        {
            var gameType = ((int)AppStatic.GameType);
            var ecsType = ecsNode.EcsTypeId;
            var allTypes = systemAssembly.GetTypes();
            var tempTypes = new List<Type>(allTypes);

            foreach (var type in allTypes)
            {
                if (type.IsAbstract || type.IsInterface)
                {
                    continue;
                }
                var platformFilters = type.GetCustomAttributes<SystemPlatformFilterAttribute>();
                var gameFilters = type.GetCustomAttributes<SystemGameFilterAttribute>();
                var ecsFilters = type.GetCustomAttributes<SystemEcsFilterAttribute>();
                var platformFilterPass = true;
                var gameFilterPass = true;
                var ecsFilterPass = true;
                if (platformFilters.Count() > 0)
                {
                    platformFilterPass = false;
                    foreach (var platformFilter in platformFilters)
                    {
                        if (platformFilter.Platform == ((int)Application.platform))
                        {
                            platformFilterPass = true;
                            break;
                        }
                    }
                }
                if (gameFilters.Count() > 0)
                {
                    gameFilterPass = false;
                    foreach (var gameFilter in gameFilters)
                    {
                        if (gameFilter.GameType == gameType)
                        {
                            gameFilterPass = true;
                            break;
                        }
                    }
                }
                if (ecsFilters.Count() > 0)
                {
                    ecsFilterPass = false;
                    foreach (var ecsFilter in ecsFilters)
                    {
                        if (ecsFilter.EcsType == ecsType)
                        {
                            ecsFilterPass = true;
                            break;
                        }
                    }
                }

                if (!platformFilterPass || !gameFilterPass || !ecsFilterPass)
                {
                    tempTypes.Remove(type);
                    //ConsoleLog.Debug(type.Name + $" not registered, because of filter. {ecsNode.GetType().Name},{platformFilterPass},{gameFilterPass},{ecsFilterPass}");
                }
            }

            ecsNode.RegisterSystems(tempTypes.ToArray());
            return ecsNode;
        }
    }
}
