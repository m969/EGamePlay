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

        public static Vector3 GetForecastPosition(EcsEntity entity)
        {
            return entity.GetComponent<TransformComponent>().ForecastPosition;
        }

        public static void ChangePosition(EcsEntity actor, Vector3 target)
        {
            actor.GetComponent<TransformComponent>().Position = target;

            EventSystem.Dispatch(new EntityUpdateCmd()
            {
                Entity = actor,
                ChangeComponent = actor.GetComponent<TransformComponent>(),
            });
        }

        public static void ChangeForward(EcsEntity actor, Vector3 target)
        {
            actor.GetComponent<TransformComponent>().Forward = target;

            EventSystem.Dispatch(new EntityUpdateCmd()
            {
                Entity = actor,
                ChangeComponent = actor.GetComponent<TransformComponent>(),
            });
        }
    }
}