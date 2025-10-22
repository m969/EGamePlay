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
            entity.Change<TransformComponent>(x => x.Position = target);
        }

        public static void ChangeForward(EcsEntity entity, Vector3 foward)
        {
            entity.Change<TransformComponent>(x => x.Forward = foward);
        }
    }
}