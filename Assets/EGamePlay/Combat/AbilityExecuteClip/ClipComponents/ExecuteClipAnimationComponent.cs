using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class ExecuteClipAnimationComponent : Component
    {
        public AnimationClip AnimationClip { get; set; }


        public override void Awake()
        {
            Entity.OnEvent(nameof(ExecuteClip.TriggerEffect), OnTriggerExecutionEffect);
        }

        public void OnTriggerExecutionEffect(Entity entity)
        {
            //Log.Debug("ExecutionAnimationComponent OnTriggerExecutionEffect");
            Entity.GetParent<SkillExecution>().OwnerEntity.Publish(AnimationClip);
        }
    }
}