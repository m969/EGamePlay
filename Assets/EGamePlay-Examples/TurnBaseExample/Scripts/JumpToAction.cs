using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using ET;
using DG.Tweening;

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

        var targetPoint = Target.ModelObject.transform.position + Target.ModelObject.transform.forward * 2;
        Creator.ModelObject.transform.DOMove(targetPoint, Creator.JumpToTime / 1000f).SetEase(Ease.Linear);
        var AnimationComponent = Creator.ModelObject.GetComponent<MonsterObjectData>().AnimationComponent;
        AnimationComponent.Speed = 2f;
        AnimationComponent.PlayFade(AnimationComponent.RunAnimation);

        await TimeHelper.WaitAsync(Creator.JumpToTime);

        PostProcess();

        var attackAction = Creator.CreateAction<AttackAction>();
        attackAction.Target = Target;
        await attackAction.ApplyAttackAsync();

        ApplyAction();
    }

    //后置处理
    private void PostProcess()
    {
        Creator.TriggerActionPoint(ActionPointType.PostJumpTo, this);
    }
}
