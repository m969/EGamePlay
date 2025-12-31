using ECS;
using EGamePlay;
using EGamePlay.Combat;
using System.Collections;
using System.Collections.Generic;

namespace ECSGame
{
    public class ActorViewSystem : AEntitySystem<Actor>,
        IAwake<Actor>,
        IInit<Actor>,
        IAfterInit<Actor>,
        IUpdate<Actor>,
        IAfterExecuteAction
    {
        public void Awake(Actor entity)
        {
        }

        public void Init(Actor entity)
        {

        }

        public void AfterInit(Actor entity)
        {
            
        }

        public void Update(Actor entity)
        {
            EntityViewSystem.Update(entity);
        }

        public void AfterExecuteAction(CombatEntity entity, EcsEntity combatAction)
        {
            var animationComp = entity.Actor.GetComponent<ECSGame.AnimationComponent>();
            AnimationSystem.PlayFade(entity.Actor, animationComp.IdleAnimation);
        }
    }
}
