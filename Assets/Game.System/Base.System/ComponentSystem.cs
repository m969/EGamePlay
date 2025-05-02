using ECS;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public class ComponentSystem : AComponentSystem<EcsEntity, EcsComponent>,
        IAwake<EcsEntity, EcsComponent>,
        IInit<EcsEntity, EcsComponent>
    {
        /// <summary>
        /// 所有组件的唤醒回调
        /// </summary>
        public void Awake(EcsEntity entity, EcsComponent component)
        {
            //Debug.Log($"EntityComponentSystem Awake {entity.GetType().Name}");
        }

        /// <summary>
        /// 所有组件的初始化回调
        /// </summary>
        public void Init(EcsEntity entity, EcsComponent component)
        {
            //Debug.Log($"EntityComponentSystem Init {entity.GetType().Name}");
        }
    }
}
