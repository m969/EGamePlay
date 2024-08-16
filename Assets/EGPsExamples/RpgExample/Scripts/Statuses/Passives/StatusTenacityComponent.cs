using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat;
using EGamePlay;
using GameUtils;
using ET;

/// <summary>
/// 状态的坚韧效果组件
/// </summary>
public class StatusTenacityComponent : EGamePlay.Component
{
    private GameTimer HealthReplyTimer { get; set; } = new GameTimer(2f);
    private bool CanReplyHealth { get; set; }
    private AbilityEffect CureAbilityEffect { get; set; }


    public override void Awake()
    {
        //Entity.OnEvent(nameof(Ability.ActivateAbility), OnActivateAbility);
    }

    public void OnActivateAbility(Entity entity)
    {
        var statusAbility = entity.As<Ability>();
        var OwnerEntity = statusAbility.OwnerEntity;
        CureAbilityEffect = statusAbility.GetComponent<AbilityEffectComponent>().CureAbilityEffect;
        OwnerEntity.ListenActionPoint(ActionPointType.PostReceiveDamage, EndReplyHealth);
        //OwnerEntity.ListenCondition(ConditionEventType.WhenInTimeNoDamage, StartReplyHealth, "4");
        statusAbility.AddComponent<UpdateComponent>();
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
    private void EndReplyHealth(Entity combatAction)
    {
        CanReplyHealth = false;
    }

    //开始生命回复
    private void StartReplyHealth()
    {
        if (GetEntity<Ability>().OwnerEntity.CurrentHealth.IsFull() == false)
        {
            CanReplyHealth = true;
            HealthReplyTimer.Reset();
        }
    }

    //生命回复
    private void ReplyHealth()
    {
        var OwnerEntity = GetEntity<Ability>().OwnerEntity;
        if (OwnerEntity.CurrentHealth.IsFull())
        {
            return;
        }
        var abilityTriggerComp = Entity.GetComponent<AbilityTriggerComponent>();
        abilityTriggerComp.GetTrigger(0).OnTrigger(new TriggerContext() { Target = OwnerEntity });
    }
}
