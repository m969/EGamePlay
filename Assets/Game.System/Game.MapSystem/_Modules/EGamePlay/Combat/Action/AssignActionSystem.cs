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
            EcsEntity.Destroy(entity);
        }

        //前置处理
        private static void ActionProcess(EffectAssignAction entity)
        {
            if (entity.Target == null)
            {
                entity.Target = entity.AssignTarget;
                if (entity.AssignTarget is IActionExecute actionExecute) entity.Target = actionExecute.Target;
                if (entity.AssignTarget is AbilityExecution skillExecution) entity.Target = skillExecution.InputTarget;
            }
        }

        public static void Execute(EffectAssignAction entity)
        {
            ActionProcess(entity);
            AbilityEffectSystem.TriggerApply(entity);
            //foreach (var item in entity.AbilityEffect.Components.Values)
            //{
            //    if (item is IEffectTriggerSystem effectTriggerSystem)
            //    {
            //        effectTriggerSystem.OnTriggerApplyEffect(entity);
            //    }
            //}

            AfterActionProcess(entity);

            FinishAction(entity);
        }

        //后置处理
        private static void AfterActionProcess(EffectAssignAction entity)
        {
            BehaviourPointSystem.TriggerActionPoint(entity.Creator, ActionPointType.ExecuteAssignEffect, entity);
            BehaviourPointSystem.TriggerActionPoint(entity.Target, ActionPointType.SufferAssignEffect, entity);

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
        }

        ///// 前置处理
        //private void PreProcess()
        //{
        //    if (Target == null)
        //    {
        //        Target = AssignTarget;
        //        if (AssignTarget is IActionExecute actionExecute) Target = actionExecute.Target;
        //        if (AssignTarget is AbilityExecution skillExecution) Target = skillExecution.InputTarget;
        //    }
        //}

        //public void AssignEffect()
        //{
        //    PreProcess();
        //    foreach (var item in AbilityEffect.Components.Values)
        //    {
        //        if (item is IEffectTriggerSystem effectTriggerSystem)
        //        {
        //            effectTriggerSystem.OnTriggerApplyEffect(this);
        //        }
        //    }

        //    PostProcess();

        //    FinishAction();
        //}

        ///// 后置处理
        //private void PostProcess()
        //{
        //    Creator.TriggerActionPoint(ActionPointType.ExecuteAssignEffect, this);
        //    if (!Target.IsDisposed)
        //    {
        //        Target.GetComponent<BehaviourPointComponent>().TriggerActionPoint(ActionPointType.SufferAssignEffect, this);
        //    }

        //    var decorators = AbilityEffect.EffectConfig.Decorators;
        //    if (decorators != null)
        //    {
        //        foreach (var item in decorators)
        //        {
        //            if (item is TriggerNewEffectWhenAssignEffectDecorator effectDecorator)
        //            {
        //                var abilityTriggerComp = AbilityEffect.OwnerAbility.GetComponent<AbilityTriggerComponent>();
        //                var effects = abilityTriggerComp.AbilityTriggers;
        //                var ExecuteTriggerType = effectDecorator.ExecuteTriggerType;
        //                for (int i = 0; i < effects.Count; i++)
        //                {
        //                    if (i == (int)ExecuteTriggerType - 1 || ExecuteTriggerType == ExecuteTriggerType.AllTriggers)
        //                    {
        //                        var effect = effects[i];
        //                        effect.OnTrigger(new TriggerContext() { Target = Target });
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //public void FinishAction()
        //{
        //    Entity.Destroy(this);
        //}
    }
}