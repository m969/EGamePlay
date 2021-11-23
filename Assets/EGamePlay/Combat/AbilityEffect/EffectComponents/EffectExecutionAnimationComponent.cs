using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    public class AnimationData
    {
        public bool HasStart;
        public bool HasEnded;
        public float StartTime;
        public float EndTime;
        public float Duration;
        public AnimationClip AnimationClip;
    }

    /// <summary>
    /// 
    /// </summary>
    public class EffectExecutionAnimationComponent : Component
    {
        public AnimationData AnimationData { get; set; }
    }
}