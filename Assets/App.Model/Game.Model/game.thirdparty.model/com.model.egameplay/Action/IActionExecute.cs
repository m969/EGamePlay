using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay.Combat;
using Sirenix.OdinInspector;
using ECS;

namespace EGamePlay.Combat
{
    [LabelText("行动类型")]
    public enum ActionType
    {
        [LabelText("施放技能")]
        SpellSkill,
        [LabelText("造成伤害")]
        CauseDamage,
        [LabelText("给予治疗")]
        GiveCure,
        [LabelText("赋给效果")]
        AssignEffect,
    }

    /// <summary>
    /// 行动执行接口，造成伤害、治疗英雄、赋给效果等属于战斗行动，战斗行动是实际应用技能效果 <see cref="AbilityEffect"/>，对战斗直接产生影响的行为
    /// 技能和buff都是挂在角色身上的一种状态，而技能表现则是一系列连续的行为（行动、事件）的组合所造成的表现和数值变化
    /// </summary>
    /// <remarks>
    /// 战斗行动由战斗实体主动发起，包含本次行动所需要用到的所有数据，并且会触发一系列行动点事件 <see cref="BehaviourPoint"/>
    /// </remarks>
    public interface IActionExecute
    {
        /// 行动能力
        public EcsEntity ActionAbility { get; set; }
        /// 效果赋给行动源
        public EffectAssignAction SourceAssignAction { get; set; }

        /// 行动实体
        public CombatEntity Creator { get; set; }
        /// 目标对象
        public EcsEntity Target { get; set; }
    }
}