using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System;
using ET;
using System.ComponentModel;
using ECSGame;

namespace EGamePlay
{
    public class AssignActionSystem : AEntitySystem<EffectAssignAction>,
        IAwake<EffectAssignAction>
    {
        public void Awake(EffectAssignAction entity)
        {

        }

        public static void FinishAction(EffectAssignAction entity)
        {
            EcsObject.Destroy(entity);
        }

        //Ç°ÖÃ´¦Àí
        private static bool ActionCheckProcess(EffectAssignAction entity)
        {
            if (entity.Target == null)
            {
                entity.Target = entity.AssignTarget;
                if (entity.AssignTarget is IActionExecute actionExecute) entity.Target = actionExecute.Target;
                if (entity.AssignTarget is AbilityExecution skillExecution) entity.Target = skillExecution.InputTarget;
            }
            return true;
        }

        public static void Execute(EffectAssignAction entity)
        {
            ActionSystem.ExecuteAction(entity, _ =>
            {
                return ActionCheckProcess(entity);
            }, _ =>
            {
                AbilityEffectSystem.TriggerApply(entity);
                return true;
            });

            var decorators = entity.AbilityEffect.EffectConfig.Decorators;
            if (decorators != null)
            {
                foreach (var item in decorators)
                {
                    if (item is TriggerNewEffectWhenAssignEffectDecorator effectDecorator)
                    {
                        var effects = entity.AbilityEffect.OwnerAbility.AbilityTriggers;
                        var ExecuteTriggerType = effectDecorator.ExecuteTriggerType;
                        for (int i = 0; i < effects.Count; i++)
                        {
                            if (i == (int)ExecuteTriggerType - 1 || ExecuteTriggerType == ExecuteTriggerType.AllTriggers)
                            {
                                var effect = effects[i];
                                AbilityTriggerSystem.OnTrigger(effect, new TriggerContext() { Target = entity.Target });
                            }
                        }
                    }
                }
            }

            FinishAction(entity);
        }
    }
}