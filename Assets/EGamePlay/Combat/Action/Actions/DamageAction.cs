using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using System;
using B83.ExpressionParser;
using GameUtils;

namespace EGamePlay.Combat
{
    public class DamageActionAbility : Entity, IActionAbility
    {
        public CombatEntity OwnerEntity { get { return GetParent<CombatEntity>(); } set { } }
        public bool Enable { get; set; }


        public override void Awake()
        {
            AddComponent<DamageBloodSuckComponent>();
        }

        public bool TryMakeAction(out DamageAction action)
        {
            if (Enable == false)
            {
                action = null;
            }
            else
            {
                action = OwnerEntity.AddChild<DamageAction>();
                action.ActionAbility = this;
                action.Creator = OwnerEntity;
            }
            return Enable;
        }
    }

    /// <summary>
    /// 伤害行动
    /// </summary>
    public class DamageAction : Entity, IActionExecute
    {
        public DamageActionAbility DamageAbility => ActionAbility as DamageActionAbility;
        public DamageEffect DamageEffect => SourceAssignAction.AbilityEffect.EffectConfig as DamageEffect;
        /// 伤害来源
        public DamageSource DamageSource { get; set; }
        /// 伤害攻势
        public int DamagePotential { get; set; }
        /// 防御架势
        public int DefensePosture { get; set; }
        /// 伤害数值
        public int DamageValue { get; set; }
        /// 是否是暴击
        public bool IsCritical { get; set; }

        /// 行动能力
        public Entity ActionAbility { get; set; }
        /// 效果赋给行动源
        public EffectAssignAction SourceAssignAction { get; set; }
        /// 行动实体
        public CombatEntity Creator { get; set; }
        /// 目标对象
        public Entity Target { get; set; }


        public void FinishAction()
        {
            Entity.Destroy(this);
        }

        /// 前置处理
        private void ActionProcess()
        {
            if (DamageSource == DamageSource.Attack)
            {
                IsCritical = (RandomHelper.RandomRate() / 100f) < Creator.GetComponent<AttributeComponent>().CriticalProbability.Value;
                var Atk = Creator.GetComponent<AttributeComponent>().Attack.Value;
                var Def = Target.GetComponent<AttributeComponent>().Defense.Value;
                var coeff = Atk / (Atk + Def);
                var Dam1 = Atk * coeff;

                DamageValue = Mathf.CeilToInt(Dam1);
                //DamageValue = Mathf.CeilToInt(Mathf.Max(1, Atk - Def));
                if (IsCritical)
                {
                    DamageValue = Mathf.CeilToInt(DamageValue * 1.5f);
                }
            }

            if (DamageSource == DamageSource.Skill)
            {
                if (DamageEffect.CanCrit)
                {
                    IsCritical = (RandomHelper.RandomRate() / 100f) < Creator.GetComponent<AttributeComponent>().CriticalProbability.Value;
                }
                DamageValue = SourceAssignAction.AbilityEffect.GetComponent<EffectDamageComponent>().GetDamageValue();
                if (IsCritical)
                {
                    DamageValue = Mathf.CeilToInt(DamageValue * 1.5f);
                }
            }

            if (DamageSource == DamageSource.Buff)
            {
                if (DamageEffect.CanCrit)
                {
                    IsCritical = (RandomHelper.RandomRate() / 100f) < Creator.GetComponent<AttributeComponent>().CriticalProbability.Value;
                }
                DamageValue = SourceAssignAction.AbilityEffect.GetComponent<EffectDamageComponent>().GetDamageValue();
            }

            var executionDamageReduceWithTargetCountComponent = SourceAssignAction.AbilityEffect.GetComponent<EffectDamageReduceWithTargetCountComponent>();
            if (executionDamageReduceWithTargetCountComponent != null)
            {
                if (SourceAssignAction.TriggerContext.AbilityItem.TryGet(out AbilityItemTargetCounterComponent targetCounterComponent))
                {
                    var damagePercent = executionDamageReduceWithTargetCountComponent.GetDamagePercent(targetCounterComponent.TargetCounter);
                    DamageValue = Mathf.CeilToInt(DamageValue * damagePercent);
                }
            }

            //触发 执行行动前 行动点
            this.TriggerCreatorActionPoint(ActionPointType.PreExecuteDamage);
            //触发 遭遇行动前 行动点
            this.TriggerTargetActionPoint(ActionPointType.PreSufferDamage);
        }

        public void Execute()
        {
            ActionProcess();

            this.TriggerCreatorApplyPoint(ApplyPointType.PreCauseDamage);
            this.TriggerTargetApplyPoint(ApplyPointType.PreReceiveDamage);

            //伤害实际施效流程
            Target.GetComponent<HealthPointComponent>().ReceiveDamage(this);

            this.TriggerCreatorApplyPoint(ApplyPointType.PostCauseDamage);
            this.TriggerTargetApplyPoint(ApplyPointType.PostReceiveDamage);

            AfterActionProcess();

            if (Target.GetComponent<HealthPointComponent>().CheckDead())
            {
                var deadEvent = new EntityDeadEvent() { DeadEntity = Target };
                Target.Publish(deadEvent);
                CombatContext.Instance.Publish(deadEvent);
            }

            FinishAction();
        }

        /// 后置处理
        private void AfterActionProcess()
        {
            //触发 执行行动后 行动点
            this.TriggerCreatorActionPoint(ActionPointType.PostExecuteDamage);
            //触发 遭遇行动后 行动点
            this.TriggerTargetActionPoint(ActionPointType.PostSufferDamage);
        }
    }

    public enum DamageSource
    {
        Attack,/// 普攻
        Skill,/// 技能
        Buff,/// Buff
    }
}