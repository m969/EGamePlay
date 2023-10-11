using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;

namespace EGamePlay.Combat
{
    public class MotionActionAbility : Entity, IActionAbility
    {
        public CombatEntity OwnerEntity { get { return GetParent<CombatEntity>(); } set { } }
        public bool Enable { get; set; }


        public bool TryMakeAction(out MotionAction action)
        {
            if (Enable == false)
            {
                action = null;
            }
            else
            {
                action = OwnerEntity.AddChild<MotionAction>();
                action.ActionAbility = this;
                action.Creator = OwnerEntity;
            }
            return Enable;
        }
    }

    /// <summary>
    /// 动作行动
    /// </summary>
    public class MotionAction : Entity, IActionExecute
    {
        public int MotionType { get; set; }

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

        //前置处理
        private void PreProcess()
        {

        }

        public void ApplyMotion()
        {
            PreProcess();

            PostProcess();
        }

        //后置处理
        private void PostProcess()
        {
            //Creator.TriggerActionPoint(ActionPointType.PostGiveCure, this);
            //Target.TriggerActionPoint(ActionPointType.PostReceiveCure, this);
        }
    }
}