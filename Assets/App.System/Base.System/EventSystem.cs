using ECS;
using ECSGame;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public class EventSystem : AEntitySystem<EcsNode>,
        IAwake<EcsNode>
    {
        public void Awake(EcsNode entity)
        {
        }

        public static void OnChange<T, T2>(T entity) where T : EcsEntity where T2 : EcsComponent
        {
            if (entity.Components.TryGetValue(typeof(T2), out var component))
            {
                entity.EcsNode.DriveComponentSystems(entity, component, typeof(IOnChange));
            }
            else
            {
                // 如果没有该组件，则不执行
                return;
            }
            OnChange(entity);
        }

        public static void OnChange<T>(T entity) where T : EcsEntity
        {
            entity.EcsNode.DriveEntitySystems(entity, typeof(IOnChange));
        }

        //public static void Update(EcsNode entity)
        //{

        //}
    }
}
