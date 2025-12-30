using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using ECS;

namespace EGamePlay.Combat
{
    public class CureAbility : EcsEntity, IActionAbility
    {
        public CombatEntity OwnerEntity { get { return GetParent<CombatEntity>(); } set { } }


        public bool TryMakeAction(out CureAction action)
        {
            if (Enable == false)
            {
                action = null;
            }
            else
            {
                action = OwnerEntity.AddChild<CureAction>();
                action.ActionAbility = this;
                action.Creator = OwnerEntity;
            }
            return Enable;
        }
    }

    /// <summary>
    /// 治疗行动
    /// </summary>
    public class CureAction : EcsEntity, IActionExecute
    {
        public CureEffect CureEffect => SourceAssignAction.AbilityEffect.EffectConfig as CureEffect;
        /// 治疗数值
        public int CureValue { get; set; }

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