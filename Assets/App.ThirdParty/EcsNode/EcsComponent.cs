
using System;
using System.Collections.Generic;

namespace ECS
{
    public interface IEcsComponent
    {

    }

    /// <summary>
    /// 组件是分离组合的机制，各个组件平级且分离
    /// </summary>
    public class EcsComponent : EcsObject, IEcsComponent
    {
        public EcsEntity Entity { get; set; }
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
                        Entity.EcsNode.DriveComponentSystems(Entity, this, typeof(IEnable));
                    }
                    if (enable && !value)
                    {
                        enable = value;
                        Entity.EcsNode.DriveComponentSystems(Entity, this, typeof(IDisable));
                    }
                }
            }
        }

        public void Dispatch<T>(Action<T> action) where T : IDispatch
        {
            var entity = this.Entity;
            if (entity == null || entity.IsDisposed)
            {
                return;
            }
            if (entity.EcsNode.AllEntityComponentSystems.TryGetValue((typeof(EcsEntity), typeof(EcsComponent)), out var systems1))
            {
                foreach (var item in systems1)
                {
                    if (item is T eventHandleSystem)
                    {
                        action.Invoke(eventHandleSystem);
                    }
                }
            }
            if (entity.EcsNode.AllEntityComponentSystems.TryGetValue((typeof(EcsEntity), GetType()), out var systems2))
            {
                foreach (var item in systems2)
                {
                    if (item is T eventHandleSystem)
                    {
                        action.Invoke(eventHandleSystem);
                    }
                }
            }
            if (entity.EcsNode.AllEntityComponentSystems.TryGetValue((entity.GetType(), typeof(EcsComponent)), out var systems3))
            {
                foreach (var item in systems3)
                {
                    if (item is T eventHandleSystem)
                    {
                        action.Invoke(eventHandleSystem);
                    }
                }
            }
            if (entity.EcsNode.AllEntityComponentSystems.TryGetValue((entity.GetType(), GetType()), out var systems4))
            {
                foreach (var item in systems4)
                {
                    if (item is T eventHandleSystem)
                    {
                        action.Invoke(eventHandleSystem);
                    }
                }
            }
        }
    }

    public class EcsComponent<T> : EcsComponent where T : EcsEntity
    {
        public new T Entity { get; set; }
    }
}