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
        public SkillExecutionData SkillExecutionData { get; set; }


        public void ParseAbilityEffects()
        {
            SkillExecutionData = new SkillExecutionData();
            var executionObj = Resources.Load<GameObject>($"SkillExecution_{this.SkillConfig.Id}");
            if (executionObj == null)
            {
                return;
            }
            SkillExecutionData.SkillExecutionAsset = executionObj;
            if (executionObj.GetComponent<PlayableDirector>() == null)
            {
                return;
            }
            var timelineAsset = SkillExecutionData.SkillExecutionAsset.GetComponent<PlayableDirector>().playableAsset as TimelineAsset;
            if (timelineAsset == null)
                return;
            SkillExecutionData.TimelineAsset = timelineAsset;

            SkillExecutionData.SkillExecuteTime = (float)timelineAsset.duration;

            //技能执行体事件解析
            var markers = timelineAsset.markerTrack.GetMarkers();
            foreach (var item in markers)
            {
                if (item is ExecutionEventEmitter colliderSpawnEmitter)
                {
                    //事件:触发应用效果给目标
                    if (colliderSpawnEmitter.ExecutionEventType == ExecutionEventType.TriggerApplyEffect)
                    {
                        var effect = new ApplyToTargetEffect();
                        effect.TriggerTime = (float)colliderSpawnEmitter.time;
                        effect.EffectApplyType = colliderSpawnEmitter.EffectApplyType;
                        SkillExecutionData.ExecutionEffects.Add(effect);
                    }
                    //事件:触发生成碰撞体
                    if (colliderSpawnEmitter.ExecutionEventType == ExecutionEventType.TriggerSpawnCollider)
                    {
                        var effect = new SpawnItemEffect();
                        effect.ColliderSpawnData = new ColliderSpawnData() { ColliderSpawnEmitter = colliderSpawnEmitter };
                        SkillExecutionData.ExecutionEffects.Add(effect);
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
                            SkillExecutionData.ExecutionEffects.Add(new AnimationEffect() { AnimationData = animationData });
                        }
                    }
                }
            }
        }
    }
}
