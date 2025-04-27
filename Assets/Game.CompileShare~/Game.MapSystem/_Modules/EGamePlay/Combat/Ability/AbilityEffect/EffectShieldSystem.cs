using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;

namespace EGamePlay
{
    public class EffectShieldSystem : AComponentSystem<AbilityEffect, EffectShieldDefenseComponent>,
        IAwake<AbilityEffect, EffectShieldDefenseComponent>
    {
        public void Awake(AbilityEffect entity, EffectShieldDefenseComponent component)
        {
            component.ShieldDefenseEffect = entity.EffectConfig as ShieldDefenseEffect;
        }

        public void OnTriggerApplyEffect(EcsEntity effectAssign)
        {
            var effectAssignAction = effectAssign as EffectAssignAction;
            var target = effectAssignAction.Target;
            target.AddComponent<AbilityItemShieldComponent>();
        }
    }
}