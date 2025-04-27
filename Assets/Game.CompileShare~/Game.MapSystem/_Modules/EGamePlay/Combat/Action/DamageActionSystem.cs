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
    public class DamageActionSystem : AEntitySystem<DamageAction>,
        IAwake<DamageAction>
    {
        public void Awake(DamageAction entity)
        {

        }

        public static void FinishAction(DamageAction entity)
        {
            EcsEntity.Destroy(entity);
        }

        //前置处理
        private static void ActionProcess(DamageAction entity)
        {
            if (entity.DamageSource == DamageSource.Attack)
            {
                entity.IsCritical = (GameUtils.RandomHelper.RandomRate() / 100f) < entity.Creator.GetComponent<AttributeComponent>().CriticalProbability.Value;
                var Atk = entity.Creator.GetComponent<AttributeComponent>().Attack.Value;
                var Def = entity.Target.GetComponent<AttributeComponent>().Defense.Value;
                var coeff = Atk / (Atk + Def);
                var Dam1 = Atk * coeff;

                entity.DamageValue = Mathf.CeilToInt(Dam1);
                if (entity.IsCritical)
                {
                    entity.DamageValue = Mathf.CeilToInt(entity.DamageValue * 1.5f);
                }
            }

            if (entity.DamageSource == DamageSource.Skill)
            {
                if (entity.DamageEffect.CanCrit)
                {
                    entity.IsCritical = (GameUtils.RandomHelper.RandomRate() / 100f) < entity.Creator.GetComponent<AttributeComponent>().CriticalProbability.Value;
                }
                entity.DamageValue = EffectDamageSystem.GetDamageValue(entity.SourceAssignAction.AbilityEffect);
                if (entity.IsCritical)
                {
                    entity.DamageValue = Mathf.CeilToInt(entity.DamageValue * 1.5f);
                }
            }

            if (entity.DamageSource == DamageSource.Buff)
            {
                if (entity.DamageEffect.CanCrit)
                {
                    entity.IsCritical = (GameUtils.RandomHelper.RandomRate() / 100f) < entity.Creator.GetComponent<AttributeComponent>().CriticalProbability.Value;
                }
                entity.DamageValue = EffectDamageSystem.GetDamageValue(entity.SourceAssignAction.AbilityEffect);
            }

            //var executionDamageReduceWithTargetCountComponent = entity.SourceAssignAction.AbilityEffect.GetComponent<EffectDamageReduceWithTargetCountComponent>();
            //if (executionDamageReduceWithTargetCountComponent != null)
            //{
            //    if (entity.SourceAssignAction.TriggerContext.AbilityItem.GetComponent<AbilityItemTargetCounterComponent>() is { } targetCounterComponent)
            //    {
            //        var damagePercent = executionDamageReduceWithTargetCountComponent.GetDamagePercent(targetCounterComponent.TargetCounter);
            //        entity.DamageValue = Mathf.CeilToInt(entity.DamageValue * damagePercent);
            //    }
            //}

            BehaviourPointSystem.TriggerActionPoint(entity.Creator, ActionPointType.PreExecuteDamage, entity);
            BehaviourPointSystem.TriggerActionPoint(entity.Target, ActionPointType.PreSufferDamage, entity);
        }

        public static void Execute(DamageAction entity)
        {
            ActionProcess(entity);

            BehaviourPointSystem.TriggerApplyPoint(entity.Creator, ApplyPointType.PreCauseDamage, entity);
            BehaviourPointSystem.TriggerApplyPoint(entity.Target, ApplyPointType.PreReceiveDamage, entity);

            //伤害实际施效流程
            HealthPointSystem.ReceiveDamage(entity.Target, entity);

            BehaviourPointSystem.TriggerApplyPoint(entity.Creator, ApplyPointType.PostCauseDamage, entity);
            BehaviourPointSystem.TriggerApplyPoint(entity.Target, ApplyPointType.PostReceiveDamage, entity);

            AfterActionProcess(entity);

            if (HealthPointSystem.CheckDead(entity.Target))
            {
                var deadEvent = new EntityDeadEvent() { DeadEntity = entity.Target };
                //entity.Target.Publish(deadEvent);
                //CombatContext.Instance.Publish(deadEvent);
            }

            FinishAction(entity);
        }

        //后置处理
        private static void AfterActionProcess(DamageAction entity)
        {
            BehaviourPointSystem.TriggerActionPoint(entity.Creator, ActionPointType.PostExecuteDamage, entity);
            BehaviourPointSystem.TriggerActionPoint(entity.Target, ActionPointType.PostSufferDamage, entity);
        }
    }
}