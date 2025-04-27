using ECS;
using EGamePlay.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSGame
{
    public class ActorSystem : AEntitySystem<Actor>,
IAwake<Actor>,
IInit<Actor>,
IUpdate<Actor>
    {
        public void Awake(Actor entity)
        {
            
        }

        public void Init(Actor entity)
        {
            MoveSystem.SetSpeed(entity, 2);
            //MoveSystem.SetStopSpeed(entity, 5);
        }

        public void Update(Actor entity)
        {

        }

        public static Actor CreateHero(Game game, long actorId)
        {
            var actor = game.AddChild<Actor>(actorId, beforeAwake: x => x.Type = 1);
            actor.AddComponent<TransformComponent>();
            actor.AddComponent<CollisionComponent>();
            actor.AddComponent<MoveComponent>();
            actor.AddComponent<MotionComponent>();
            return actor;
        }

        public static Actor CreateMonster(Game game, long actorId)
        {
            var actor = game.AddChild<Actor>(actorId, beforeAwake: x => x.Type = 1);
            actor.AddComponent<TransformComponent>();
            actor.AddComponent<CollisionComponent>();
            actor.AddComponent<MoveComponent>();
            actor.AddComponent<MotionComponent>();
            return actor;
        }
    } 
}
