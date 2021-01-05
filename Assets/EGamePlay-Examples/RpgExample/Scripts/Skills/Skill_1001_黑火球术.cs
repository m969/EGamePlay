using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat.Ability;
using EGamePlay.Combat.Skill;
using EGamePlay.Combat;
using EGamePlay;

public class Skill1001Entity : SkillAbility
{
    public override AbilityExecution CreateAbilityExecution()
    {
        var abilityExecution = EntityFactory.CreateWithParent<Skill1001Execution>(this.GetParent<CombatEntity>(), this);
        return abilityExecution;
    }
}

public class Skill1001Execution : SkillAbilityExecution
{
    public override async void BeginExecute()
    {
        base.BeginExecute();

        //OwnerEntity.Publish(new PlayAnimationTask());
        //EntityFactory.CreateWithParent<PlayAnimationTask>(this, "施法动作").ExecuteTaskAsync().Coroutine();

        var taskData = new CastProjectileTaskData();
        taskData.ProjectilePrefab = (AbilityEntity as Skill1001Entity).SkillConfigObject.SkillEffectObject;
        var task = EntityFactory.CreateWithParent<CastProjectileTask>(this, taskData);
        await task.ExecuteTaskAsync();

        AbilityEntity.ApplyAbilityEffectsTo(InputCombatEntity);

        EndExecute();
    }
}