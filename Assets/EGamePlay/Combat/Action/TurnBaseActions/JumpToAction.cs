using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using ET;

namespace EGamePlay.Combat
{
    public class JumpToActionAbility : Entity, IActionAbility
    {
        public CombatEntity OwnerEntity { get { return GetParent<CombatEntity>(); } set { } }
        public bool Enable { get; set; }


        public bool TryMakeAction(out JumpToAction action)
        {
            if (Enable == false)
            {
                action = null;
            }
            else
            {
                action = OwnerEntity.AddChild<JumpToAction>();
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
        //    var execution = OwnerEntity.MakeAction<JumpToAction>();
        //    execution.ActionAbility = this;
        //    return execution;
        //}

        //public bool TryMakeAction(out JumpToAction abilityExecution)
        //{
        //    if (Enable == false)
        //    {
        //        abilityExecution = null;
        //    }
        //    else
        //    {
        //        abilityExecution = CreateExecution() as JumpToAction;
        //    }
        //    return Enable;
        //}
    }

    public class JumpToAction : Entity, IActionExecute
    {
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
            Creator.TriggerActionPoint(ActionPointType.PreJumpTo, this);
        }

        public async ETTask ApplyJumpTo()
        {
            PreProcess();

            await TimeHelper.WaitAsync(Creator.JumpToTime);

            PostProcess();

            if (Creator.AttackSpellAbility.TryMakeAction(out var attackAction))
            {
                attackAction.Target = Target;
                await attackAction.ApplyAttackAwait();
            }

            FinishAction();
        }

        //后置处理
        private void PostProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.PostJumpTo, this);
        }
    }
}