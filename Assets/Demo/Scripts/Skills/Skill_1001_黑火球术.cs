using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat.Ability;
using EGamePlay.Combat;
using EGamePlay;

public class Skill1001Entity : AbilityEntity
{
    public override AbilityExecution CreateAbilityExecution()
    {
        var abilityExecution = EntityFactory.CreateWithParent<Skill1001Execution>(this.GetParent<CombatEntity>(), this);
        return abilityExecution;
    }
}

public class Skill1001Execution : AbilityExecution
{
    public override async void BeginExecute()
    {
        base.BeginExecute();

        var task = EntityFactory.CreateWithParent<CastProjectileAbilityTask>(this, InputCombatEntity.Position);
        await task.ExecuteTaskAsync();

        AbilityEntity.ApplyAbilityEffect(InputCombatEntity);

        EndExecute();
    }
}