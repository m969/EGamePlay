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

        public static void TriggerApply(AbilityEffect entity, EffectAssignAction effectAssign)
        {
            effectAssign.Target.AddComponent<AbilityItemShieldComponent>();
        }
    }
}