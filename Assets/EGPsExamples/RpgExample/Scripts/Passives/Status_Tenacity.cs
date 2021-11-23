using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat;
using EGamePlay;
using GameUtils;
using ET;

public class StatusTenacity : StatusAbility
{
    private GameTimer HealthReplyTimer { get; set; } = new GameTimer(2f);
    private bool CanReplyHealth { get; set; }
    private AbilityEffect CureAbilityEffect { get; set; }


    public override void ActivateAbility()
    {
        base.ActivateAbility();
        CureAbilityEffect = GetComponent<AbilityEffectComponent>().CureAbilityEffect;
        OwnerEntity.ListenActionPoint(ActionPointType.PostReceiveDamage, EndReplyHealth);
        OwnerEntity.ListenerCondition(ConditionType.WhenInTimeNoDamage, StartReplyHealth, 4f);
        AddComponent<UpdateComponent>();
    }

    public override void Update()
    {
        if (CanReplyHealth)
        {
            if (HealthReplyTimer.IsRunning)
            {
                HealthReplyTimer.UpdateAsRepeat(Time.deltaTime, ReplyHealth);
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
        if (OwnerEntity.CurrentHealth.IsFull() == false)
        {
            CanReplyHealth = true;
            HealthReplyTimer.Reset();
        }
    }

    //生命回复
    private void ReplyHealth()
    {
        if (OwnerEntity.CurrentHealth.IsFull())
        {
            return;
        }
        if (OwnerEntity.CureAbility.TryMakeAction(out var action))
        {
            action.Target = OwnerEntity;
            action.AbilityEffect = CureAbilityEffect;
            action.ApplyCure();
        }
    }
}
