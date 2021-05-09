using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using ET;

namespace EGamePlay.Combat
{
    public class TurnActionAbility : ActionAbility<TurnAction>
    {

    }

    /// <summary>
    /// 回合行动
    /// </summary>
    public class TurnAction : ActionExecution<TurnActionAbility>
    {
        public int TurnActionType { get; set; }


        //前置处理
        private void PreProcess()
        {

        }

        public async ETTask ApplyTurn()
        {
            PreProcess();

            if (Creator.JumpToActionAbility.TryCreateAction(out var jumpToAction))
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