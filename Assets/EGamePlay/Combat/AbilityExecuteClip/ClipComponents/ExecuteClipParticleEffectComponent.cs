using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class ExecuteClipParticleEffectComponent : Component
    {
        public GameObject ParticleEffectPrefab { get; set; }
        public GameObject ParticleEffectObj { get; set; }


        public override void Awake()
        {
            Entity.OnEvent(nameof(ExecuteClip.TriggerEffect), OnTriggerStart);
            Entity.OnEvent(nameof(ExecuteClip.EndEffect), OnTriggerEnd);
        }

        public void OnTriggerStart(Entity entity)
        {
            //Log.Debug("ExecutionAnimationComponent OnTriggerExecutionEffect");
            //Entity.GetParent<SkillExecution>().OwnerEntity.Publish(AnimationClip);
            ParticleEffectObj = GameObject.Instantiate(ParticleEffectPrefab, Entity.GetParent<SkillExecution>().OwnerEntity.Position, Entity.GetParent<SkillExecution>().OwnerEntity.Rotation);
        }

        public void OnTriggerEnd(Entity entity)
        {
            //Log.Debug("ExecutionAnimationComponent OnTriggerExecutionEffect");
            //Entity.GetParent<SkillExecution>().OwnerEntity.Publish(AnimationClip);
            GameObject.Destroy(ParticleEffectObj);
        }
    }
}