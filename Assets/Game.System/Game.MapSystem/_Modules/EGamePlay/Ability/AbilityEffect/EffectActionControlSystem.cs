using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;

namespace EGamePlay
{
    public class EffectActionControlSystem : AComponentSystem<AbilityEffect, EffectActionControlComponent>,
        IAwake<AbilityEffect, EffectActionControlComponent>,
        IEnable<AbilityEffect, EffectActionControlComponent>,
        IDisable<AbilityEffect, EffectActionControlComponent>
    {
        public void Awake(AbilityEffect entity, EffectActionControlComponent component)
        {
            component.ActionControlEffect = entity.EffectConfig as ActionControlEffect;
        }

        public void Enable(AbilityEffect entity, EffectActionControlComponent component)
        {
            BuffSystem.OnBuffChanged(entity.Parent.Parent as CombatEntity, entity.GetParent<Ability>());
        }

        public void Disable(AbilityEffect entity, EffectActionControlComponent component)
        {
            BuffSystem.OnBuffChanged(entity.Parent.Parent as CombatEntity, entity.GetParent<Ability>());
        }

        public static void TriggerApply(AbilityEffect entity, EffectAssignAction effectAssign)
        {
            entity.GetComponent<EffectActionControlComponent>().Enable = true;
        }
    }
}