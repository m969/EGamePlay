using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameUtils;
using ECS;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class EffectDamageReduceWithTargetCountComponent : EcsComponent
    {
        public float ReducePercent { get; set; }
        public float MinPercent { get; set; }
    }
}