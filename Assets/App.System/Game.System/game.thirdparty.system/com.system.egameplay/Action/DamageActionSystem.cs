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
            EcsObject.Destroy(entity);
        }

        //前置处理
        private static bool ActionCheckProcess(DamageAction entity)
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

            return true;
        }

        public static void Execute(DamageAction entity)
        {
            ActionSystem.ExecuteAction(entity, _ =>
            {
                return ActionCheckProcess(entity);
            }, _ =>
            {
                //伤害实际施效流程
                HealthSystem.ConsumeHealth(entity.Target, entity);
                return true;
            });

            if (HealthSystem.CheckDead(entity.Target))
            {

            }

            FinishAction(entity);
        }

        //后置处理
        private static void AfterActionProcess(DamageAction entity)
        {

        }
    }
}