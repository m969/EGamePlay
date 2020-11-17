using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat.Ability;
using EGamePlay.Combat.Skill;
using EGamePlay;

public class Skill1002Entity : SkillAbilityEntity
{

}

public class Skill1002Execution : AbilityExecution
{
    public override async void BeginExecute()
    {
        base.BeginExecute();

        var task = EntityFactory.CreateWithParent<CastProjectileAbilityTask>(this, InputPoint);
        await task.ExecuteTaskAsync();

        AbilityEntity.ApplyAbilityEffect(InputCombatEntity);

        EndExecute();
    }

    public override void EndExecute()
    {
        base.EndExecute();
    }
}