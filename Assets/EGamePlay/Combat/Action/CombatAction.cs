using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using Sirenix.OdinInspector;

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
    /// 战斗行动概念，造成伤害、治疗英雄、赋给效果等属于战斗行动，需要继承自CombatAction
    /// </summary>
    /// <remarks>
    /// 战斗行动由战斗实体主动发起，包含本次行动所需要用到的所有数据，并且会触发一系列行动点事件 <see cref="ActionPoint"/>
    /// </remarks>
    public abstract class CombatAction : Entity
    {
        public ActionType ActionType { get; set; }
        public CombatEntity Creator { get; set; }
        public CombatEntity Target { get; set; }

        public virtual void ApplyAction()
        {
            Entity.Destroy(this);
        }
    }
}