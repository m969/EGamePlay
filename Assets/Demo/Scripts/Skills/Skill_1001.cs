using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat.Ability;
using EGamePlay;

public class Skill_1001 : AbilityEntity
{
    public override async void ActivateAbility()
    {
        base.ActivateAbility();

        var task = EntityFactory.CreateWithParent<CastProjectileAbilityTask>(this);
        await task.ExecuteTaskAsync();

        ApplyAbilityEffect();

        EndAbility();
    }

    public override void EndAbility()
    {
        base.EndAbility();

    }
}
