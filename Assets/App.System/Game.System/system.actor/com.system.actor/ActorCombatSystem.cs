using ECS;
using EGamePlay.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;

namespace ECSGame
{
    public class ActorCombatSystem : AComponentSystem<Actor, ActorCombatComponent>,
        IAwake<Actor, ActorCombatComponent>,
        IInit<Actor, ActorCombatComponent>
    {
        public void Awake(Actor entity, ActorCombatComponent component)
        {
            entity.CombatEntity = CombatEntitySystem.Create(entity);
        }

        public void Init(Actor entity, ActorCombatComponent component)
        {
        }
    }
}
