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
        public EffectAnimationComponent EffectAnimationComponent { get; set; }


        public override void Setup()
        {
            Entity.Subscribe<ExecutionEffectEvent>(ExecutionEffect);
        }

        public void ExecutionEffect(ExecutionEffectEvent evnt)
        {
            evnt.ExecutionEffect.GetParent<SkillExecution>().OwnerEntity.Publish(EffectAnimationComponent.AnimationData.AnimationClip);
        }
    }
}