using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if EGAMEPLAY_ET
using Unity.Mathematics;
#endif

#if UNITY
namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class ExecuteParticleEffectComponent : Component
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
#if EGAMEPLAY_ET
            ParticleEffectObj = GameObject.Instantiate(ParticleEffectPrefab, Entity.GetParent<SkillExecution>().OwnerEntity.Position, quaternion.LookRotation(Entity.GetParent<SkillExecution>().OwnerEntity.Rotation, math.forward()));
#else
            ParticleEffectObj = GameObject.Instantiate(ParticleEffectPrefab, Entity.GetParent<SkillExecution>().OwnerEntity.Position, Entity.GetParent<SkillExecution>().OwnerEntity.Rotation);
#endif
        }

        public void OnTriggerEnd(Entity entity)
        {
            GameObject.Destroy(ParticleEffectObj);
        }
    }
}
#endif