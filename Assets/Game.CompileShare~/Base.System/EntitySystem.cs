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
    }
}