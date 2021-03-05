using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;

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

    public void ApplyTurnAction()
    {
        PreProcess();

        var attackAction = Creator.CreateAction<AttackAction>();
        attackAction.Target = Creator;
        attackAction.ApplyAttack();

        PostProcess();
    }

    //后置处理
    private void PostProcess()
    {

    }
}
