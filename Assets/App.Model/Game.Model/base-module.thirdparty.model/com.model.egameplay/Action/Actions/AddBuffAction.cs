using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using EGamePlay.Combat;
using ET;
using GameUtils;
using ECS;

namespace EGamePlay.Combat
{
    public class AddBuffAbility : EcsEntity, IActionAbility
    {
        public CombatEntity OwnerEntity { get { return GetParent<CombatEntity>(); } set { } }


        public bool TryMakeAction(out AddBuffAction action)
        {
            if (Enable == false)
            {
                action = null;
            }
            else
            {
                action = OwnerEntity.AddChild<AddBuffAction>();
                action.ActionAbility = this;
                action.Creator = OwnerEntity;
            }
            return Enable;
        }
    }

    /// <summary>
    /// 施加状态行动
    /// </summary>
    public class AddBuffAction : EcsEntity, IActionExecute
    {
        public EcsEntity SourceAbility { get; set; }
        public AddStatusEffect AddStatusEffect => SourceAssignAction.AbilityEffect.EffectConfig as AddStatusEffect;
        public Ability BuffAbility { get; set; }

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