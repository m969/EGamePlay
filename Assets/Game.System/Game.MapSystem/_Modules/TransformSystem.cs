using ECS;
using System.Collections;
using System.Collections.Generic;
using Vector3 = UnityEngine.Vector3;

namespace ECSGame
{
    public class TransformSystem : AComponentSystem<EcsEntity, TransformComponent>,
    IAwake<EcsEntity, TransformComponent>
    {
        public void Awake(EcsEntity entity, TransformComponent component)
        {
        }

        public static Vector3 GetPosition(EcsEntity entity)
        {
            return entity.GetComponent<TransformComponent>().Position;
        }

        public static void ChangePosition(EcsEntity entity, Vector3 target)
        {
            entity.GetComponent<TransformComponent>().Position = target;
            EntitySystem.ComponentChange<EcsEntity, TransformComponent>(entity);
        }

        public static void ChangeForward(EcsEntity entity, Vector3 foward)
        {
            entity.GetComponent<TransformComponent>().Forward = foward;
            EntitySystem.ComponentChange<EcsEntity, TransformComponent>(entity);
        }
    }
}