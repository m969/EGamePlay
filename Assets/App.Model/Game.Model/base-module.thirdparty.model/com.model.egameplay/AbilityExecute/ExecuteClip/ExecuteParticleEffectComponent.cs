using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if EGAMEPLAY_ET
using Unity.Mathematics;
#endif

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class ExecuteParticleEffectComponent : ECS.EcsComponent
    {
        public GameObject ParticleEffectPrefab { get; set; }
        public GameObject ParticleEffectObj { get; set; }
    }
}
