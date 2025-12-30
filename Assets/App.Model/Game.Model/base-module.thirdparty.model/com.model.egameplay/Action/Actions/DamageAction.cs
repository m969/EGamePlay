using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using System;
using B83.ExpressionParser;
using GameUtils;
using ECS;

namespace EGamePlay.Combat
{
    public class DamageAbility : EcsEntity, IActionAbility
    {
        public CombatEntity OwnerEntity { get { return GetParent<CombatEntity>(); } set { } }

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
    public class DamageAction : EcsEntity, IActionExecute
    {
        public DamageAbility DamageAbility => ActionAbility as DamageAbility;
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
        public EcsEntity ActionAbility { get; set; }
        /// 效果赋给行动源
        public EffectAssignAction SourceAssignAction { get; set; }
        /// 行动实体
        public CombatEntity Creator { get; set; }
        /// 目标对象
        public EcsEntity Target { get; set; }
    }

    public enum DamageSource
    {
        Attack,/// 普攻
        Skill,/// 技能
        Buff,/// Buff
    }
}