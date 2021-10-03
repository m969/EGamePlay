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
    private AbilityEffect AbilityCureEffect { get; set; }


    public override void ActivateAbility()
    {
        base.ActivateAbility();
        AbilityCureEffect = GetComponent<AbilityEffectComponent>().GetEffect();
        OwnerEntity.ListenActionPoint(ActionPointType.PostReceiveDamage, EndReplyHealth);
        OwnerEntity.ListenerCondition(ConditionType.WhenInTimeNoDamage, StartReplyHealth, 4f);
        AddComponent<UpdateComponent>();
        //Coroutine();
    }

    public override void Update()
    {
        //if (OwnerEntity.CurrentHealth.IsFull())
        //{
        //    if (AbilityCureEffect.Enable) AbilityCureEffect.DisableEffect();
        //}
        //else
        //{
        //    AbilityCureEffect.EnableEffect();
        //}
        if (CanReplyHealth)
        {
            if (HealthReplyTimer.IsRunning)
            {
                HealthReplyTimer.UpdateAsRepeat(Time.deltaTime, ReplyHealth);
            }
        }
    }

    ////协程
    //private async void Coroutine()
    //{
    //    while (true)
    //    {
    //        if (IsDisposed)
    //        {
    //            break;
    //        }
    //        await TimeHelper.WaitAsync(100);
    //        if (CanReplyHealth)
    //        {
    //            if (OwnerEntity.CurrentHealth.Percent() < 1f)
    //            {
    //                HealthReplyTimer.UpdateAsRepeat(0.1f, ReplyHealth);
    //            }
    //        }
    //    }
    //}

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
        }
    }

    //生命回复
    private void ReplyHealth()
    {
        if (OwnerEntity.CureActionAbility.TryCreateAction(out var action))
        {
            action.Target = OwnerEntity;
            action.AbilityEffect = GetComponent<AbilityEffectComponent>().GetEffect();
            //action.CureValue = OwnerEntity.CurrentHealth.PercentHealth(0.02f);
            action.ApplyCure();
        }
    }
}
