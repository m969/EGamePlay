using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat.Ability;
using EGamePlay.Combat.Skill;
using EGamePlay.Combat;
using EGamePlay;
using ET;

public class Skill1001Ability : SkillAbility
{
    public override AbilityExecution CreateAbilityExecution()
    {
        var abilityExecution = Entity.CreateWithParent<Skill1001Execution>(this.GetParent<CombatEntity>(), this);
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

        var skillExecutionAsset = SkillAbility.SkillConfigObject.SkillExecutionAsset;
        var markers = skillExecutionAsset.markerTrack.GetMarkers();
        foreach (var item in markers)
        {
            if (item is ColliderSpawnEmitter colliderSpawnEmitter)
            {
                if (colliderSpawnEmitter.ColliderType == ColliderType.TargetFly)
                {
                    await TimerComponent.Instance.WaitAsync((int)(colliderSpawnEmitter.time * 1000));

                    var taskData = new CastProjectileTaskData();
                    taskData.FlyTime = 0.3f;
                    taskData.TargetEntity = InputCombatEntity;
                    taskData.ProjectilePrefab = SkillAbility.SkillConfigObject.SkillEffectObject;
                    var task = Entity.CreateWithParent<CastProjectileTask>(OwnerEntity, taskData);
                    task.ExecuteTaskAsync().Coroutine();
                }
            }
        }

        AbilityEntity.ApplyAbilityEffectsTo(InputCombatEntity);

        EndExecute();
    }
}