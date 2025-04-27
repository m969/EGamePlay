using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vector3 = UnityEngine.Vector3;

namespace ECSGame
{
    public class MoveSystem : AComponentSystem<EcsEntity, MoveComponent>,
IAwake<EcsEntity, MoveComponent>
    {
        public void Awake(EcsEntity actor, MoveComponent moveComponent)
        {
        }

        public static void SetSpeed(EcsEntity entity, int value)
        {
            var moveComp = entity.GetComponent<MoveComponent>();
            moveComp.Speed = value;
        }

        //public static void SetStopSpeed(EcsEntity entity, int value)
        //{
        //    var moveComp = entity.GetComponent<MoveComponent>();
        //    moveComp.StopSpeed = value;
        //}

        //public static void ChangeMove(EcsEntity entity, Vector3 target)
        //{
        //    var moveComp = entity.GetComponent<MoveComponent>();
        //    moveComp.TrueDirection = target;
        //}

        //public const float SpeedAdaptive = 0.01f;

        //public static void SetMovePosition(EcsEntity actor, Vector3 position)
        //{
        //    var transComp = actor.GetComponent<TransformComponent>();
        //    var beforePos = transComp.Position;
        //    transComp.Position = position;

        //    //EventSystem.Dispatch(new EntityUpdateCmd()
        //    //{
        //    //    Entity = actor,
        //    //    ChangeComponent = actor.GetComponent<MoveComponent>(),
        //    //});
        //}

        //public static void SetMoveForecastPosition(EcsEntity actor, Vector3 position)
        //{
        //    var transComp = actor.GetComponent<TransformComponent>();
        //    var beforePos = transComp.ForecastPosition;
        //    transComp.ForecastPosition = position;

        //    EventSystem.Dispatch(new EntityUpdateCmd()
        //    {
        //        Entity = actor,
        //        ChangeComponent = actor.GetComponent<MoveComponent>(),
        //    });
        //}
    } 
}
