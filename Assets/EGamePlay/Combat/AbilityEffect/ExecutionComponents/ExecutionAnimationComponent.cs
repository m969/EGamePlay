using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class ExecutionAnimationComponent : Component
    {
        public EffectExecutionAnimationComponent EffectAnimationComponent { get; set; }


        public override void Setup()
        {
            Entity.Subscribe<ExecutionEffectEvent>(OnTriggerExecutionEffect);
        }

        public void OnTriggerExecutionEffect(ExecutionEffectEvent evnt)
        {
            evnt.ExecutionEffect.GetParent<SkillExecution>().OwnerEntity.Publish(EffectAnimationComponent.AnimationData.AnimationClip);
        }
    }
}