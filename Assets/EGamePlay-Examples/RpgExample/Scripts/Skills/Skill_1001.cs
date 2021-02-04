using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat.Ability;
using EGamePlay.Combat.Skill;
using EGamePlay.Combat;
using EGamePlay;
using ET;
using Log = EGamePlay.Log;


public class AnimationData
{
    public bool HasStart;
    public bool HasEnded;
    public float StartTime;
    public float EndTime;
    public float Duration;
    public AnimationClip AnimationClip;
}

public class ColliderSpawnData
{
    public bool HasStart;
    public ColliderSpawnEmitter ColliderSpawnEmitter;
}

public class Skill1001Ability : SkillAbility
{
    public override AbilityExecution CreateAbilityExecution()
    {
        var abilityExecution = Entity.CreateWithParent<Skill1001Execution>(this.GetParent<CombatEntity>(), this);
        abilityExecution.AddComponent<UpdateComponent>();
        return abilityExecution;
    }
}

public class Skill1001Execution : SkillAbilityExecution
{
    public long OriginTime { get; set; }
    public List<AnimationData> AnimationDatas { get; set; } = new List<AnimationData>();
    public List<ColliderSpawnData> ColliderSpawnDatas { get; set; } = new List<ColliderSpawnData>();


    public override void Awake(object initData)
    {
        base.Awake(initData);

        OriginTime = TimeHelper.Now();

        var skillExecutionAsset = SkillAbility.SkillConfigObject.SkillExecutionAsset;

        var markers = skillExecutionAsset.markerTrack.GetMarkers();
        foreach (var item in markers)
        {
            if (item is ColliderSpawnEmitter colliderSpawnEmitter)
            {
                ColliderSpawnDatas.Add(new ColliderSpawnData() { ColliderSpawnEmitter = colliderSpawnEmitter });
            }
        }

        var rootTracks = skillExecutionAsset.GetRootTracks();
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
                        animationData.EndTime = animationData.StartTime + animationData.Duration;
                        animationData.AnimationClip = clip.animationClip;
                        AnimationDatas.Add(animationData);
                    }
                }
            }
        }
    }

    public override void Update()
    {
        if (SkillAbility.Spelling == false)
        {
            return;
        }

        var nowSeconds = (double)(TimeHelper.Now() - OriginTime) / 1000;
        foreach (var item in ColliderSpawnDatas)
        {
            if (item.HasStart == false)
            {
                if (nowSeconds >= item.ColliderSpawnEmitter.time)
                {
                    item.HasStart = true;
                    StartColliderSpawn(item.ColliderSpawnEmitter);
                }
            }
        }

        var allAnimationEnded = true;
        foreach (var item in AnimationDatas)
        {
            if (item.HasStart == false)
            {
                allAnimationEnded = false;
                if (nowSeconds >= item.StartTime)
                {
                    item.HasStart = true;
                    Hero.Instance.AnimationComponent.PlayFade(item.AnimationClip);
                }
            }
            else
            {
                if (item.HasEnded == false)
                {
                    allAnimationEnded = false;
                    if (nowSeconds >= item.EndTime)
                    {
                        item.HasEnded = true;
                    }
                }
            }
        }
        if (allAnimationEnded)
        {
            EndExecute();
        }
    }

    private void StartColliderSpawn(ColliderSpawnEmitter colliderSpawnEmitter)
    {
        if (colliderSpawnEmitter.ColliderType == ColliderType.TargetFly)
        {
            var taskData = new CastProjectileTaskData();
            taskData.FlyTime = 0.3f;
            taskData.TargetEntity = InputCombatEntity;
            taskData.ProjectilePrefab = SkillAbility.SkillConfigObject.SkillEffectObject;
            var task = Entity.CreateWithParent<CastProjectileTask>(OwnerEntity, taskData);
            task.OnEnterCallback = () => { AbilityEntity.ApplyAbilityEffectsTo(InputCombatEntity); };
            task.ExecuteTaskAsync().Coroutine();
        }
    }

    public override async void BeginExecute()
    {
        base.BeginExecute();

        Hero.Instance.StopMove();
        Hero.Instance.SkillPlaying = true;

        Hero.Instance.transform.GetChild(0).LookAt(InputCombatEntity.Position);

        //var tasks = new List<ETTask>();
        //foreach (var item in AnimationDatas)
        //{
        //    var tcs = new ETTaskCompletionSource();
        //    PlayAnimation(item, tcs).Coroutine();
        //    tasks.Add(tcs.Task);
        //}

        //async ETVoid PlayAnimation(AnimationData animationData, ETTaskCompletionSource tcs)
        //{
        //    if (animationData.StartTime > 0)
        //    {
        //        await TimerComponent.Instance.WaitAsync((int)(animationData.StartTime * 1000));
        //    }
        //    Hero.Instance.AnimationComponent.PlayFade(animationData.AnimationClip);
        //    await TimerComponent.Instance.WaitAsync((int)(animationData.Duration * 1000));
        //    tcs.SetResult();
        //}

        //await ETTaskHelper.WaitAll(tasks.ToArray());

        //Hero.Instance.SkillPlaying = false;
        //EndExecute();
    }

    public override void EndExecute()
    {
        Hero.Instance.SkillPlaying = false;
        Hero.Instance.AnimationComponent.PlayFade(Hero.Instance.AnimationComponent.IdleAnimation);
        base.EndExecute();
    }
}