using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat.Ability;
using EGamePlay.Combat.Skill;
using EGamePlay.Combat;
using EGamePlay;
using System.Threading.Tasks;

public class PassiveSkill1004Entity : SkillAbilityEntity
{
    private GameTimer HealthReplyTimer { get; set; } = new GameTimer(1f);
    private bool CanReplyHealth { get; set; }


    public override void ActivateAbility()
    {
        base.ActivateAbility();
        CanReplyHealth = true;
        AbilityOwner.ListenActionPoint(ActionPointType.PostReceiveDamage, EndReplyHealth);
        AbilityOwner.AddListener(ConditionType.WhenInTimeNoDamage, StartReplyHealth, 4f);
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
            await Task.Delay(100);
            if (CanReplyHealth)
            {
                if (AbilityOwner.CurrentHealth.Percent() < 1f)
                {
                    HealthReplyTimer.UpdateAsRepeat(0.1f, ReplyHealth);
                }
            }
        }
    }

    //结束生命回复
    private void EndReplyHealth(CombatAction combatAction)
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
        var action = CombatActionManager.CreateAction<CureAction>(AbilityOwner);
        action.Target = AbilityOwner;
        action.CureValue = AbilityOwner.CurrentHealth.PercentHealth(1);
        action.ApplyCure();
    }
}
