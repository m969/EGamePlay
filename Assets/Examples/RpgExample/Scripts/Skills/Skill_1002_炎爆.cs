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

public class Skill1002Execution : SkillAbilityExecution
{
    public override async void BeginExecute()
    {
        base.BeginExecute();

        var taskData2 = new CreateTriggerTaskData();
        taskData2.Position = InputPoint;
        taskData2.TriggerPrefab = (AbilityEntity as Skill1002Entity).SkillConfigObject.AreaCollider;
        var task2 = EntityFactory.CreateWithParent<CreateTriggerTask>(this, taskData2);
        task2.OnTriggerEnterCallbackAction = (other) => {
            AbilityEntity.ApplyAbilityEffect(other.GetComponent<Monster>().CombatEntity);
        };
        task2.ExecuteTaskAsync().Coroutine();

        var taskData = new CreateExplosionTaskData();
        taskData.TargetPoint = InputPoint;
        taskData.ExplosionPrefab = (AbilityEntity as Skill1002Entity).SkillConfigObject.SkillEffectObject;
        var task = EntityFactory.CreateWithParent<CreateExplosionTask>(this, taskData);
        await task.ExecuteTaskAsync();

        EndExecute();
    }
}