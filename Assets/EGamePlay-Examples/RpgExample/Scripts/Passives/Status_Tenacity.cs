using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat.Status;
using EGamePlay.Combat.Ability;
using EGamePlay.Combat;
using EGamePlay;
using GameUtils;
using ET;

public class StatusTenacity : StatusAbility
{
    private GameTimer HealthReplyTimer { get; set; } = new GameTimer(2f);
    private bool CanReplyHealth { get; set; }


    public override void ActivateAbility()
    {
        base.ActivateAbility();
        CanReplyHealth = true;
        OwnerEntity.ListenActionPoint(ActionPointType.PostReceiveDamage, EndReplyHealth);
        OwnerEntity.ListenerCondition(ConditionType.WhenInTimeNoDamage, StartReplyHealth, 4f);
        Coroutine();
    }

    //协程
    private async void Coroutine()
    {
        while (true)
        {
            if (IsDisposed)
            {
                break;
            }
            await TimeHelper.WaitAsync(100);
            if (CanReplyHealth)
            {
                if (OwnerEntity.CurrentHealth.Percent() < 1f)
                {
                    HealthReplyTimer.UpdateAsRepeat(0.1f, ReplyHealth);
                }
            }
        }
    }

    //结束生命回复
    private void EndReplyHealth(ActionExecution combatAction)
    {
        CanReplyHealth = false;
    }

    //开始生命回复
    private void StartReplyHealth()
    {
        CanReplyHealth = true;
    }

    //生命回复
    private void ReplyHealth()
    {
        if (OwnerEntity.CureActionAbility.TryCreateAction(out var action))
        {
            action.Target = OwnerEntity;
            action.CureValue = OwnerEntity.CurrentHealth.PercentHealth(2);
            action.ApplyCure();
        }
    }
}
