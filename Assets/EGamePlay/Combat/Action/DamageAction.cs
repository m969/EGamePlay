using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using System;
using B83.ExpressionParser;
using GameUtils;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 伤害行动
    /// </summary>
    public class DamageAction : CombatAction
    {
        public DamageEffect DamageEffect { get; set; }
        //伤害来源
        public DamageSource DamageSource { get; set; }
        //伤害数值
        public int DamageValue { get; set; }
        //是否是暴击
        public bool IsCritical { get; set; }


        private int ParseDamage()
        {
            var expression = ExpressionHelper.ExpressionParser.EvaluateExpression(DamageEffect.DamageValueProperty);
            if (expression.Parameters.ContainsKey("自身攻击力"))
            {
                expression.Parameters["自身攻击力"].Value = Creator.GetComponent<AttributeComponent>().AttackPower.Value;
            }
            return (int)expression.Value;
        }

        //前置处理
        private void PreProcess()
        {
            if (DamageSource == DamageSource.Attack)
            {
                IsCritical = (RandomHelper.RandomRate() / 100f) < Creator.GetComponent<AttributeComponent>().CriticalProbability.Value;
                DamageValue = (int)Mathf.Max(1, Creator.GetComponent<AttributeComponent>().AttackPower.Value - Target.GetComponent<AttributeComponent>().AttackDefense.Value);
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
                DamageValue = ParseDamage();
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
                DamageValue = ParseDamage();
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