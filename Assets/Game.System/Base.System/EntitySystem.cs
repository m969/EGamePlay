using ECS;
using ECSGame;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public class EntitySystem : AEntitySystem<EcsEntity>,
        IAwake<EcsEntity>,
        IInit<EcsEntity>
    {
        /// <summary>
        /// 所有实体的唤醒回调
        /// </summary>
        public void Awake(EcsEntity entity)
        {
            
        }

        /// <summary>
        /// 所有实体的初始化回调
        /// </summary>
        public void Init(EcsEntity entity)
        {
            
        }

        //public static void ComponentChange<T, T2>(T entity) where T : EcsEntity, new() where T2 : EcsComponent, new()
        //{
        //    foreach (var item in entity.Components.Values)
        //    {
        //        entity.EcsNode.DriveComponentSystems(entity, item, typeof(IOnChange));
        //    }
        //    entity.EcsNode.DriveEntitySystems(entity, typeof(IOnChange));
        //}
    }
}