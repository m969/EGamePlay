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

    public class AnimationEffect : Effect
    {
        public override string Label => "播放动作";

        public AnimationData AnimationData { get; set; }
    }
}