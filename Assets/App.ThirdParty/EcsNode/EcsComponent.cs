
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
        public T GetEntity<T>() where T : EcsEntity { return Entity as T; }
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
    }

    public class EcsComponent<T> : EcsComponent where T : EcsEntity
    {
        public new T Entity { get; set; }
    }
}