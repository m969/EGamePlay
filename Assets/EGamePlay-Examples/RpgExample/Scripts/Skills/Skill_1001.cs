using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat.Ability;
using EGamePlay.Combat.Skill;
using EGamePlay.Combat;
using EGamePlay;
using ET;


public class AnimationData
{
    public float StartTime;
    public float Duration;
    public AnimationClip AnimationClip;
}

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
                    task.OnEnterCallback = () => { AbilityEntity.ApplyAbilityEffectsTo(InputCombatEntity); };
                    task.ExecuteTaskAsync().Coroutine();
                }
            }
        }

        var animationDatas = new List<AnimationData>();
        var rootTracks = skillExecutionAsset.GetRootTracks();
        var outputTracks = skillExecutionAsset.GetOutputTracks();
        foreach (var item in rootTracks)
        {
            if (item.hasClips)
            {
                var clips = item.GetClips();
                foreach (var clip in clips)
                {
                    if (clip.animationClip != null)
                    {
                        var animationData = new AnimationData();
                        animationData.StartTime = (float)clip.clipIn;
                        animationData.Duration = (float)clip.duration;
                        animationData.AnimationClip = clip.animationClip;
                        animationDatas.Add(animationData);
                    }
                }
            }
        }

        var tasks = new List<ETTask>();
        foreach (var item in animationDatas)
        {
            var tcs = new ETTaskCompletionSource();
            PlayAnimation(item, tcs).Coroutine();
            tasks.Add(tcs.Task);
        }

        async ETVoid PlayAnimation(AnimationData animationData, ETTaskCompletionSource tcs)
        {
            if (animationData.StartTime > 0)
            {
                await TimerComponent.Instance.WaitAsync((int)(animationData.StartTime * 1000));
            }
            Hero.Instance.AnimationComponent.PlayFade(animationData.AnimationClip);
            await TimerComponent.Instance.WaitAsync((int)(animationData.Duration * 1000));
            tcs.SetResult();
        }

        await ETTaskHelper.WaitAll(tasks.ToArray());

        EndExecute();
    }
}