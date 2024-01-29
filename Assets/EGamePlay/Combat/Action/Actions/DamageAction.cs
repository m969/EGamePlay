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
        public CombatEntity Target { get; set; }


        public void FinishAction()
        {
            Entity.Destroy(this);
        }

        /// 前置处理
        private void PreProcess()
        {
            //Target = SourceAssignAction.Target;
            //if (Target == null)
            //{
            //    if (SourceAssignAction.AssignTarget is CombatEntity combatEntity)
            //    {
            //        Target = combatEntity;
            //    }
            //    if (SourceAssignAction.AssignTarget is SkillExecution skillExecution)
            //    {
            //        Target = skillExecution.InputTarget;
            //    }
            //    if (SourceAssignAction.AssignTarget is IActionExecute actionExecute)
            //    {
            //        Target = actionExecute.Target;
            //    }
            //}

            if (DamageSource == DamageSource.Attack)
            {
                IsCritical = (RandomHelper.RandomRate() / 100f) < Creator.GetComponent<AttributeComponent>().CriticalProbability.Value;
                DamageValue = Mathf.CeilToInt(Mathf.Max(1, Creator.GetComponent<AttributeComponent>().Attack.Value - Target.GetComponent<AttributeComponent>().Defense.Value));
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
                    //Log.Debug($"{targetCounterComponent.TargetCounter} {damagePercent}");
                    DamageValue = Mathf.CeilToInt(DamageValue * damagePercent);
                }
                //executionDamageReduceWithTargetCountComponent.AddOneTarget();
            }
            //if (ExecutionEffect != null)
            //{

            //}

            //触发 造成伤害前 行动点
            Creator.TriggerActionPoint(ActionPointType.PreCauseDamage, this);
            //触发 承受伤害前 行动点
            Target.TriggerActionPoint(ActionPointType.PreReceiveDamage, this);
        }

        /// 应用伤害
        public void ApplyDamage()
        {
            PreProcess();

            //Log.Debug($"DamageAction ApplyDamage");
            Target.ReceiveDamage(this);

            PostProcess();

            if (Target.CheckDead())
            {
                var deadEvent = new EntityDeadEvent() { DeadEntity = Target };
                Target.Publish(deadEvent);
                CombatContext.Instance.Publish(deadEvent);
            }

            FinishAction();
        }

        /// 后置处理
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
        Attack,/// 普攻
        Skill,/// 技能
        Buff,/// Buff
    }
}