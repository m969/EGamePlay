using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat.Ability;
using EGamePlay;

public class Skill_1001 : AbilityEntity
{
    public override void ActivateAbility()
    {
        base.ActivateAbility();

        EndAbility();
    }

    public override void EndAbility()
    {
        base.EndAbility();

    }
}


public class Skill_1001_Execution : AbilityExecution
{
    public override async void BeginExecute()
    {
        base.BeginExecute();

        var task = EntityFactory.CreateWithParent<CastProjectileAbilityTask>(this, InputPoint);
        await task.ExecuteTaskAsync();

        AbilityEntity.ApplyAbilityEffect(AbilityExecutionTarget);

        EndExecute();
    }

    public override void EndExecute()
    {
        base.EndExecute();
    }
}