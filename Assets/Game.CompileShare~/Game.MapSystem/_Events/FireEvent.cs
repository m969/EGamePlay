//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using ECS;
//using ET;

//namespace ECSGame
//{
//    public class FireEvent : AEventRun<FireEvent, Actor, TSVector>
//    {
//        protected override async ETTask Run(Actor actor, TSVector direction)
//        {
//            var game = actor.GetParent<TrueGame>();
//            var item = game.AddChild<Item>();
//            item.AddComponent<TransformComponent>();
//            item.AddComponent<CollisionComponent>();
//            item.AddComponent<MoveComponent>();
//            item.GetComponent<CollisionComponent>().Layer = actor.GetComponent<CollisionComponent>().Layer;
//            var actorTrans = actor.GetComponent<TransformComponent>();
//            TransformSystem.ChangePosition(item, actorTrans.ForecastPosition);
//            TransformSystem.ChangeForward(item, actorTrans.Forward);
//            MoveSystem.SetSpeed(item, 150);
//            MoveSystem.ChangeMove(item, actorTrans.Forward.normalized);

//            EventSystem.Dispatch(new EntityCreateCmd()
//            {
//                Entity = item
//            });
//        }
//    }
//}
