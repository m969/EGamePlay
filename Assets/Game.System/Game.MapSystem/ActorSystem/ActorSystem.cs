using ECS;
using EGamePlay.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;

namespace ECSGame
{
    public class ActorSystem : AEntitySystem<Actor>,
        IAwake<Actor>,
        IInit<Actor>,
        IUpdate<Actor>,
        IOnChange<Actor>
    {
        public void Awake(Actor entity)
        {
            
        }

        public void Init(Actor entity)
        {
            MoveSystem.SetSpeed(entity, 2);
            if (entity.CombatEntity.IsHero)
            {
                CombatEntitySystem.HeroInit(entity.CombatEntity);
            }
            else
            {
                CombatEntitySystem.MonsterInit(entity.CombatEntity);
            }
        }

        public void Update(Actor entity)
        {
            if (entity.GetComponent<TransformComponent>() is { } transComp)
            {
                entity.CombatEntity.Position = transComp.Position;
                entity.CombatEntity.Rotation = transComp.Rotation;
            }
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

        public void OnChange(Actor entity)
        {
            
        }
    } 
}
