using System;
using System.Collections.Generic;

namespace ECS
{
    /// <summary>
    /// 实体是树形节点的机制，父子节点具有包含的属性，同类节点具有独立分离的属性，能同时满足系统和组件的需求
    /// </summary>
    public class EcsEntity : EcsObject
    {
        public long Id { get; set; }
        //public long InstanceId { get; set; }
        //public long UniqueId { get; set; }
        public bool IsDisposed
        {
            get
            {
                return Id == 0;
            }
        }
        public Dictionary<long, EcsEntity> Id2Children = new();
        public Dictionary<Type, EcsComponent> Components { get; set; } = new();
        public EcsEntity Parent { get; set; }
        private bool enable;
        public bool Enable
        {
            get { return enable; }
            set
            {
                if (enable != value)
                {
                    if (!enable && value)
                    {
                        enable = value;
                        foreach (var item in Components.Values)
                        {
                            item.Enable = value;
                        }
                        EcsNode.DriveEntitySystems(this, typeof(IEnable), new object[] { this });
                    }
                    if (enable && !value)
                    {
                        enable = value;
                        foreach (var item in Components.Values)
                        {
                            item.Enable = value;
                        }
                        EcsNode.DriveEntitySystems(this, typeof(IDisable), new object[] { this });
                    }
                }
            }
        }

        public EcsNode EcsNode
        {
            get
            {
                if (this is EcsNode node)
                    return node;
                return Parent.EcsNode;
            }
        }

        public T GetParent<T>() where T : EcsEntity
        {
            return (T)Parent;
        }

        public T As<T>() where T : EcsEntity
        {
            return (T)this;
        }

        public T AddChild<T>(Action<T> beforeAwake = null) where T : EcsEntity, new()
        {
            return AddChild(EcsNode.NewInstanceId(), beforeAwake);
        }

        public T AddChild<T>(long id, Action<T> beforeAwake = null) where T : EcsEntity, new()
        {
            var entity = new T();
            entity.Id = id;
            entity.Parent = this;
            Id2Children.Add(entity.Id, entity);
            beforeAwake?.Invoke(entity);
            EcsNode.AddEntity(entity);
            DriveAwake(entity);
            return entity;
        }

        public EcsEntity AddChild(Type type, Action<EcsEntity> beforeAwake = null)
        {
            return AddChild(EcsNode.NewInstanceId(), type, beforeAwake);
        }

        public EcsEntity AddChild(long id, Type type, Action<EcsEntity> beforeAwake = null)
        {
            var entity = Activator.CreateInstance(type) as EcsEntity;
            entity.Id = id;
            entity.Parent = this;
            Id2Children.Add(entity.Id, entity);
            beforeAwake?.Invoke(entity);
            EcsNode.AddEntity(entity);
            DriveAwake(entity);
            return entity;
        }

        public T GetChild<T>(long id) where T : EcsEntity, new()
        {
            Id2Children.TryGetValue(id, out var entity);
            return (T)entity;
        }

        public void RemoveChild(EcsEntity entity)
        {
            DriveDestroy(entity);
            Id2Children.Remove(entity.Id);
            EcsNode.RemoveEntity(entity);
        }

        public T AddComponent<T>(Action<T> beforeAwake = null) where T : EcsComponent, new()
        {
            var component = new T();
            component.Entity = this;
            Components.Add(typeof(T), component);
            beforeAwake?.Invoke(component);
            DriveAwake(component);
            EcsNode.DriveEntitySystems(this, typeof(IOnAddComponent), new object[] { this, component });
            return component;
        }

        public void RemoveComponent<T>() where T : EcsComponent, new()
        {
            RemoveComponent(typeof(T));
        }

        public void RemoveComponent(Type type)
        {
            DriveDestroy(Components[type]);
            Components.Remove(type);
        }

        public T GetComponent<T>() where T : EcsComponent, new()
        {
            Components.TryGetValue(typeof(T), out var component);
            return component as T;
        }

        public void ComponentChange<T>() where T : EcsComponent, new()
        {
            var entity = this;
            foreach (var item in entity.Components.Values)
            {
                entity.EcsNode.DriveComponentSystems(entity, item, typeof(IOnChange));
            }
            entity.EcsNode.DriveEntitySystems(entity, typeof(IOnChange), new object[] { entity });
        }

        public void Change<T>(Action<T> changeAction) where T : EcsComponent, new()
        {
            changeAction?.Invoke(GetComponent<T>());
            ComponentChange<T>();
        }

        private void DriveAwake(EcsEntity entity)
        {
            EcsNode.DriveEntitySystems(entity, typeof(IAwake), new object[] { entity });
        }

        private void DriveAwake<T>(T component) where T : EcsComponent, new()
        {
            EcsNode.DriveComponentSystems(this, component, typeof(IAwake));
        }

        public void DriveDestroy(EcsEntity entity)
        {
            EcsNode.DriveEntitySystems(entity, typeof(IDestroy), new object[] { entity });
        }

        public void DriveDestroy<T>(T component) where T : EcsComponent, new()
        {
            EcsNode.DriveComponentSystems(this, component, typeof(IDestroy));
        }

        public void Init()
        {
            foreach (var item in Components.Values)
            {
                EcsNode.DriveComponentSystems(this, item, typeof(IInit));
            }
            EcsNode.DriveEntitySystems(this, typeof(IInit), new object[] { this });

            foreach (var item in Components.Values)
            {
                EcsNode.DriveComponentSystems(this, item, typeof(IAfterInit));
            }
            EcsNode.DriveEntitySystems(this, typeof(IAfterInit), new object[] { this });
        }
    }
}