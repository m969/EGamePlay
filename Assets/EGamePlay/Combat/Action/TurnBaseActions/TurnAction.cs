using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using ET;

/// <summary>
/// 回合行动
/// </summary>
public class TurnAction : CombatAction
{
    public int TurnActionType { get; set; }


    //前置处理
    private void PreProcess()
    {

    }

    public async ETTask ApplyTurn()
    {
        PreProcess();

        var jumpToAction = Creator.CreateAction<JumpToAction>();
        jumpToAction.Target = Target;
        await jumpToAction.ApplyJumpTo();

        PostProcess();
    }

    //后置处理
    private void PostProcess()
    {

    }
}
