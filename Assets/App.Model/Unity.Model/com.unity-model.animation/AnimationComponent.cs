using ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSGame
{
    public class AnimationComponent : EcsComponent
    {
        public Animancer.AnimancerComponent AnimancerComponent;
        public AnimationClip IdleAnimation;
        public AnimationClip RunAnimation;
        public AnimationClip JumpAnimation;
        public AnimationClip AttackAnimation;
        public AnimationClip SkillAnimation;
        public AnimationClip StunAnimation;
        public AnimationClip DamageAnimation;
        public AnimationClip DeadAnimation;
        public AnimationClip[] AnimationClips;
        public float Speed { get; set; } = 1f;
    }
}
