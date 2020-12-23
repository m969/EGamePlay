using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat.Ability;
using EGamePlay.Combat.Skill;
using EGamePlay.Combat;
using EGamePlay;
using ET;

public class Skill1004Entity : SkillAbility
{
    public override AbilityExecution CreateAbilityExecution()
    {
        var abilityExecution = EntityFactory.CreateWithParent<Skill1004Execution>(this.GetParent<CombatEntity>(), this);
        return abilityExecution;
    }
}

public class Skill1004Execution : SkillAbilityExecution
{
    public override async void BeginExecute()
    {
        base.BeginExecute();

        var taskData = new CreateEffectTaskData();
        taskData.Position = GetParent<CombatEntity>().Position;
        taskData.Direction = InputDirection;
        taskData.LifeTime = 2000;
        taskData.EffectPrefab = GetAbilityEntity<Skill1004Entity>().SkillConfigObject.SkillEffectObject;
        var task = EntityFactory.CreateWithParent<CreateEffectTask>(this, taskData);
        task.ExecuteTaskAsync().Coroutine();

        await TimerComponent.Instance.WaitAsync(1000);

        var taskData2 = new CreateTriggerTaskData();
        taskData2.Position = GetParent<CombatEntity>().Position;
        taskData2.Direction = InputDirection;
        taskData2.LifeTime = 200;
        taskData2.TriggerPrefab = GetAbilityEntity<Skill1004Entity>().SkillConfigObject.AreaCollider;
        var task2 = EntityFactory.CreateWithParent<CreateTriggerTask>(this, taskData2);
        task2.OnTriggerEnterCallbackAction = (other) => {
            AbilityEntity.ApplyAbilityEffect(other.GetComponent<Monster>().CombatEntity);
        };
        await task2.ExecuteTaskAsync();

        EndExecute();
    }
}