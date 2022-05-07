using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class ExecutionSpawnItemComponent : Component
    {
        public SpawnItemEffect SpawnItemEffect { get; set; }


        public override void Setup()
        {
            Entity.Subscribe<ExecutionEffectEvent>(OnTriggerExecutionEffect);
        }

        public void OnTriggerExecutionEffect(ExecutionEffectEvent evnt)
        {
#if !NOT_UNITY
            evnt.ExecutionEffect.GetParent<SkillExecution>().SpawnCollisionItem(SpawnItemEffect.ColliderSpawnData.ColliderSpawnEmitter);
#endif
        }
    }
}