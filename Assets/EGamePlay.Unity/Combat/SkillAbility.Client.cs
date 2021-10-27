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
    public partial class SkillAbility : AbilityEntity
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

            var markers = timelineAsset.markerTrack.GetMarkers();
            foreach (var item in markers)
            {
                if (item is ExecutionEventEmitter colliderSpawnEmitter)
                {
                    if (colliderSpawnEmitter.ExecutionEventType == ExecutionEventType.TriggerApplyEffect)
                    {
                        if (colliderSpawnEmitter.EffectApplyType == EffectApplyType.Effects)
                        {
                            foreach (var abilityEffect in AbilityEffects)
                            {
                                //abilityEffect.Name = "ApplyEffect";
                                if (abilityEffect.Name == "SpawnItem") continue;
                                if (abilityEffect.Name == "Animation") continue;
                                abilityEffect.AddComponent<EffectTimeTriggerComponent>().TriggerTime = (float)colliderSpawnEmitter.time;
                            }
                        }
                        if (colliderSpawnEmitter.EffectApplyType == EffectApplyType.Effect1)
                        {
                            var abilityEffect = AbilityEffects[0];
                            abilityEffect.AddComponent<EffectTimeTriggerComponent>().TriggerTime = (float)colliderSpawnEmitter.time;
                        }
                        if (colliderSpawnEmitter.EffectApplyType == EffectApplyType.Effect2)
                        {
                            var abilityEffect = AbilityEffects[1];
                            abilityEffect.AddComponent<EffectTimeTriggerComponent>().TriggerTime = (float)colliderSpawnEmitter.time;
                        }
                        if (colliderSpawnEmitter.EffectApplyType == EffectApplyType.Effect3)
                        {
                            var abilityEffect = AbilityEffects[2];
                            abilityEffect.AddComponent<EffectTimeTriggerComponent>().TriggerTime = (float)colliderSpawnEmitter.time;
                        }
                    }
                    if (colliderSpawnEmitter.ExecutionEventType == ExecutionEventType.TriggerSpawnCollider)
                    {
                        var abilityEffect = AddChild<AbilityEffect>();
                        abilityEffect.Name = "SpawnItem";
                        abilityEffect.AddComponent<EffectSpawnItemComponent>().ColliderSpawnData = new ColliderSpawnData() { ColliderSpawnEmitter = colliderSpawnEmitter };
                        AbilityEffectComponent.AddEffect(abilityEffect);
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
                            abilityEffect.AddComponent<EffectAnimationComponent>().AnimationData = animationData;
                            AbilityEffectComponent.AddEffect(abilityEffect);
                        }
                    }
                }
            }
        }
    }
}
