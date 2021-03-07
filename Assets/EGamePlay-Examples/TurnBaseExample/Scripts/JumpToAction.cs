using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using ET;

public class JumpToAction : CombatAction
{
    //前置处理
    private void PreProcess()
    {
        Creator.TriggerActionPoint(ActionPointType.PreJumpTo, this);
    }

    public async ETTask ApplyJumpTo()
    {
        PreProcess();

        await TimeHelper.WaitAsync(Creator.JumpToTime);

        var attackAction = Creator.CreateAction<AttackAction>();
        attackAction.Target = Target;
        await attackAction.ApplyAttack();

        PostProcess();
    }

    //后置处理
    private void PostProcess()
    {
        Creator.TriggerActionPoint(ActionPointType.PostJumpTo, this);
    }
}
