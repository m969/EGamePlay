using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using System;
using B83.ExpressionParser;
using GameUtils;

namespace EGamePlay.Combat
{
    public class DamageActionAbility : EffectActionAbility<DamageAction>
    {

    }

    /// <summary>
    /// 伤害行动
    /// </summary>
    public class DamageAction : ActionExecution<DamageActionAbility>
    {
        public DamageEffect DamageEffect => AbilityEffect.EffectConfig as DamageEffect;
        //伤害来源
        public DamageSource DamageSource { get; set; }
        //伤害数值
        public int DamageValue { get; set; }
        //是否是暴击
        public bool IsCritical { get; set; }


        //前置处理
        private void PreProcess()
        {
            if (DamageSource == DamageSource.Attack)
            {
                IsCritical = (RandomHelper.RandomRate() / 100f) < Creator.GetComponent<AttributeComponent>().CriticalProbability.Value;
                DamageValue = (int)Mathf.Max(1, Creator.GetComponent<AttributeComponent>().Attack.Value - Target.GetComponent<AttributeComponent>().Defense.Value);
                if (IsCritical)
                {
                    DamageValue = (int)(DamageValue * 1.5f);
                }
            }
            if (DamageSource == DamageSource.Skill)
            {
                if (DamageEffect.CanCrit)
                {
                    IsCritical = (RandomHelper.RandomRate() / 100f) < Creator.GetComponent<AttributeComponent>().CriticalProbability.Value;
                }
                DamageValue = AbilityEffect.GetComponent<EffectDamageComponent>().GetDamageValue();
                if (IsCritical)
                {
                    DamageValue = (int)(DamageValue * 1.5f);
                }
            }
            if (DamageSource == DamageSource.Buff)
            {
                if (DamageEffect.CanCrit)
                {
                    IsCritical = (RandomHelper.RandomRate() / 100f) < Creator.GetComponent<AttributeComponent>().CriticalProbability.Value;
                }
                DamageValue = AbilityEffect.GetComponent<EffectDamageComponent>().GetDamageValue();
            }
        }

        //应用伤害
        public void ApplyDamage()
        {
            PreProcess();

            Target.ReceiveDamage(this);

            PostProcess();

            if (Target.CheckDead())
            {
                Target.Publish(new DeadEvent());
                CombatContext.Instance.OnCombatEntityDead(Target);
            }

            ApplyAction();
        }

        //后置处理
        private void PostProcess()
        {
            //触发 造成伤害后 行动点
            Creator.TriggerActionPoint(ActionPointType.PostCauseDamage, this);
            //触发 承受伤害后 行动点
            Target.TriggerActionPoint(ActionPointType.PostReceiveDamage, this);
        }
    }

    public class DeadEvent
    {

    }

    public enum DamageSource
    {
        Attack,//普攻
        Skill,//技能
        Buff,//Buff
    }
}