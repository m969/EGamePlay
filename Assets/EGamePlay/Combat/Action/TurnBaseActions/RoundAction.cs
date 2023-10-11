using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using ET;

namespace EGamePlay.Combat
{
    public class RoundActionAbility : Entity, IActionAbility
    {
        public CombatEntity OwnerEntity { get { return GetParent<CombatEntity>(); } set { } }
        public bool Enable { get; set; }


        public bool TryMakeAction(out RoundAction action)
        {
            if (Enable == false)
            {
                action = null;
            }
            else
            {
                action = OwnerEntity.AddChild<RoundAction>();
                action.ActionAbility = this;
                action.Creator = OwnerEntity;
            }
            return Enable;
        }

        //public void TryActivateAbility() => ActivateAbility();

        //public void ActivateAbility() => Enable = true;

        //public void DeactivateAbility() { }

        //public void EndAbility() { }

        //public Entity CreateExecution()
        //{
        //    var execution = OwnerEntity.MakeAction<RoundAction>();
        //    execution.ActionAbility = this;
        //    return execution;
        //}

        //public bool TryMakeAction(out RoundAction abilityExecution)
        //{
        //    if (Enable == false)
        //    {
        //        abilityExecution = null;
        //    }
        //    else
        //    {
        //        abilityExecution = CreateExecution() as RoundAction;
        //    }
        //    return Enable;
        //}
    }

    /// <summary>
    /// 回合行动
    /// </summary>
    public class RoundAction : Entity, IActionExecute
    {
        public int RoundActionType { get; set; }

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

        public async ETTask ApplyRound()
        {
            PreProcess();

            if (Creator.JumpToAbility.TryMakeAction(out var jumpToAction))
            {
                jumpToAction.Target = Target;
                await jumpToAction.ApplyJumpTo();
            }

            PostProcess();
        }

        //后置处理
        private void PostProcess()
        {

        }
    }
}