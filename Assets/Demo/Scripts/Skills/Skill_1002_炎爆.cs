using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat.Ability;
using EGamePlay.Combat.Skill;
using EGamePlay.Combat;
using EGamePlay;

public class Skill1002Entity : SkillAbilityEntity
{
    public override AbilityExecution CreateAbilityExecution()
    {
        var abilityExecution = EntityFactory.CreateWithParent<Skill1002Execution>(this.GetParent<CombatEntity>(), this);
        return abilityExecution;
    }
}

public class Skill1002Execution : AbilityExecution
{
    public override async void BeginExecute()
    {
        base.BeginExecute();

        var taskData = new CreateExplosionTaskData();
        taskData.TargetPoint = InputPoint;
        taskData.ExplosionPrefab = (AbilityEntity as Skill1002Entity).SkillConfigObject.SkillEffectObject;
        var task = EntityFactory.CreateWithParent<CreateExplosionTask>(this, taskData);
        await task.ExecuteTaskAsync();

        AbilityEntity.ApplyAbilityEffect(InputCombatEntity);

        EndExecute();
    }
}