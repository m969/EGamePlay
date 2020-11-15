using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat.Ability;
using EGamePlay.Combat;
using EGamePlay;
using System.Threading.Tasks;

public class PassiveSkill1004Entity : AbilityEntity
{
    private GameTimer NoDamageTimer { get; set; } = new GameTimer(4f);
    private GameTimer HealthReplyTimer { get; set; } = new GameTimer(1f);
    private bool CanReplyHealth { get; set; }


    public override void ActivateAbility()
    {
        Log.Debug($"{GetType().Name} ActivateAbility");
        base.ActivateAbility();
        CanReplyHealth = true;
        AbilityOwner.AddListener(ActionPointType.PostReceiveDamage, EndReplyHealth);
        Coroutine();
    }

    //协程计时
    private async void Coroutine()
    {
        while (true)
        {
            await Task.Delay(100);
            if (CanReplyHealth)
            {
                HealthReplyTimer.UpdateAsRepeat(0.1f, ReplyHealth);
            }
            else
            {
                NoDamageTimer.UpdateAsFinish(0.1f, StartReplyHealth);
            }
        }
    }

    //结束生命回复
    private void EndReplyHealth(CombatAction combatAction)
    {
        Log.Debug($"{GetType().Name} EndReplyHealth");
        CanReplyHealth = false;
        NoDamageTimer.Reset();
    }

    //开始生命回复
    private void StartReplyHealth()
    {
        Log.Debug($"{GetType().Name} StartReplyHealth");
        CanReplyHealth = true;
    }

    //生命回复
    private void ReplyHealth()
    {
        if (AbilityOwner.CurrentHealth.Percent() >= 1f)
        {
            return;
        }
        Log.Debug($"{GetType().Name} ReplyHealth");
        var action = CombatActionManager.CreateAction<CureAction>(AbilityOwner);
        action.Target = AbilityOwner;
        action.CureValue = AbilityOwner.CurrentHealth.PercentHealth(1);
        action.ApplyCure();
    }

    public override void EndAbility()
    {
        base.EndAbility();
    }
}
