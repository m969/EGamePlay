using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using System;

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


        //前置处理
        private void PreProcess()
        {
            if (DamageSource == DamageSource.Attack)
            {
                IsCritical = (RandomHelper.RandomRate() / 100f) < Creator.NumericBox.CriticalProb_F.Value;
                DamageValue = Mathf.Max(1, Creator.NumericBox.PhysicAttack_I.Value - Target.NumericBox.PhysicDefense_I.Value);
                if (IsCritical)
                {
                    DamageValue = (int)(DamageValue * 1.5f);
                }
            }
            if (DamageSource == DamageSource.Skill)
            {
                if (DamageEffect.CanCrit)
                {
                    IsCritical = (RandomHelper.RandomRate() / 100f) < Creator.NumericBox.CriticalProb_F.Value;
                }
                DamageValue = int.Parse(DamageEffect.DamageValueFormula);
            }
            if (DamageSource == DamageSource.Buff)
            {
                if (DamageEffect.CanCrit)
                {
                    IsCritical = (RandomHelper.RandomRate() / 100f) < Creator.NumericBox.CriticalProb_F.Value;
                }
                DamageValue = int.Parse(DamageEffect.DamageValueFormula);
            }
        }

        //应用伤害
        public void ApplyDamage()
        {
            PreProcess();
            //Log.Debug("DamageAction ApplyDamage");
            Target.ReceiveDamage(this);
            PostProcess();
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

    public enum DamageSource
    {
        Attack,//普攻
        Skill,//技能
        Buff,//Buff
    }
}