using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using Sirenix.OdinInspector;

namespace EGamePlay.Combat
{
    public enum OperationType
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
    /// 战斗行动概念，造成伤害、治疗英雄、赋给效果等都属于战斗行动，需要继承自CombatOperation
    /// 战斗行动由战斗实体主动创建并应用，包含本次行动所需要用到的所有数据，并且会触发一系列战斗行为表现
    /// 战斗行动是可以嵌套调用的，比如 施放技能 触发 造成伤害 触发 赋给效果 
    /// </summary>
    public class CombatOperation
    {
        public OperationType OperationType { get; set; }
        public CombatEntity Creator { get; set; }
        public CombatEntity Target { get; set; }

        public virtual void ApplyOperation()
        {

        }
    }
}