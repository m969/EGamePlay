using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;

namespace EGamePlay
{
    public class EffectAddBuffSystem : AComponentSystem<AbilityEffect, EffectAddBuffComponent>,
        IAwake<AbilityEffect, EffectAddBuffComponent>
    {
        public void Awake(AbilityEffect entity, EffectAddBuffComponent component)
        {
            component.AddStatusEffect = entity.EffectConfig as AddStatusEffect;
            component.Duration = component.AddStatusEffect.Duration;
        }

        public static int GetNumericValue(AbilityEffect entity)
        {
            return 1;
        }

        public static void OnTriggerApplyEffect(AbilityEffect entity, EcsEntity effectAssign)
        {
            //Log.Debug($"EffectCureComponent OnAssignEffect");
            var effectAssignAction = effectAssign as EffectAssignAction;
            if (entity.OwnerEntity.AddStatusAbility.TryMakeAction(out var action))
            {
                action.SourceAssignAction = effectAssignAction;
                action.Target = effectAssignAction.Target;
                action.SourceAbility = effectAssignAction.SourceAbility;
                AddBuffActionSystem.Execute(action);
            }
        }

        public static IActionExecute GetActionExecution(AbilityEffect entity)
        {
            if (entity.OwnerEntity.AddStatusAbility.TryMakeAction(out var action))
            {
                return action;
            }
            return null;
        }
    }
}