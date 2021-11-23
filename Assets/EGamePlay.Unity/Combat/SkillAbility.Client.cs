using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EGamePlay.Combat;
using ET;
using Log = EGamePlay.Log;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SkillAbility
    {
        public GameObject SkillExecutionAsset { get; set; }
        public float SkillExecuteTime { get; set; }


        public void ParseAbilityEffects()
        {
            SkillExecutionAsset = Resources.Load<GameObject>($"SkillExecution_{this.SkillConfig.Id}");

            if (SkillExecutionAsset == null)
                return;
            var timelineAsset = SkillExecutionAsset.GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
            if (timelineAsset == null)
                return;

            SkillExecuteTime = (float)timelineAsset.duration;

            //技能执行体事件解析
            var markers = timelineAsset.markerTrack.GetMarkers();
            foreach (var item in markers)
            {
                if (item is ExecutionEventEmitter colliderSpawnEmitter)
                {
                    //事件:触发应用效果给目标
                    if (colliderSpawnEmitter.ExecutionEventType == ExecutionEventType.TriggerApplyEffect)
                    {
                        var abilityEffect_e = AddChild<AbilityEffect>();
                        abilityEffect_e.Name = "ApplyToTarget";
                        abilityEffect_e.EffectSourceType = EffectSourceType.Execution;
                        var applyToTargetComponent = abilityEffect_e.AddComponent<EffectExecutionApplyToTargetComponent>();
                        applyToTargetComponent.TriggerTime = (float)colliderSpawnEmitter.time;
                        applyToTargetComponent.EffectApplyType = colliderSpawnEmitter.EffectApplyType;
                        AbilityEffectComponent.AddEffect(abilityEffect_e);
                    }
                    //事件:触发生成碰撞体
                    if (colliderSpawnEmitter.ExecutionEventType == ExecutionEventType.TriggerSpawnCollider)
                    {
                        var abilityEffect_e = AddChild<AbilityEffect>();
                        abilityEffect_e.Name = "SpawnItem";
                        abilityEffect_e.EffectSourceType = EffectSourceType.Execution;
                        abilityEffect_e.AddComponent<EffectExecutionSpawnItemComponent>().ColliderSpawnData = new ColliderSpawnData() { ColliderSpawnEmitter = colliderSpawnEmitter };
                        AbilityEffectComponent.AddEffect(abilityEffect_e);
                    }
                }
            }

            var rootTracks = timelineAsset.GetRootTracks();
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
                            //AnimationDatas.Add(animationData);
                            var abilityEffect = AddChild<AbilityEffect>();
                            abilityEffect.Name = "Animation";
                            abilityEffect.AddComponent<EffectExecutionAnimationComponent>().AnimationData = animationData;
                            AbilityEffectComponent.AddEffect(abilityEffect);
                        }
                    }
                }
            }
        }
    }
}

//if (colliderSpawnEmitter.EffectApplyType == EffectApplyType.Effects)
//{
//    foreach (var abilityEffect in AbilityEffects)
//    {
//        if (abilityEffect.Name == "SpawnItem") continue;
//        if (abilityEffect.Name == "Animation") continue;
//        if (abilityEffect.Name == "ApplyToTarget") continue;
//        applyToTargetComponent.ApplyAbilityEffects.Add(abilityEffect);
//    }
//}
//AbilityEffect abilityEffect2 = null;
//if (colliderSpawnEmitter.EffectApplyType == EffectApplyType.Effect1)
//{
//    abilityEffect2 = AbilityEffects[0];
//}
//if (colliderSpawnEmitter.EffectApplyType == EffectApplyType.Effect2)
//{
//    abilityEffect2 = AbilityEffects[1];
//}
//if (colliderSpawnEmitter.EffectApplyType == EffectApplyType.Effect3)
//{
//    abilityEffect2 = AbilityEffects[2];
//}
//applyToTargetComponent.ApplyAbilityEffects.Add(abilityEffect2);