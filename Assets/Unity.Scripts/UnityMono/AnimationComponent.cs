using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Animancer;
using EGamePlay.Combat;

namespace EGamePlay
{
    public class AnimationComponent : MonoBehaviour
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


        private void Start()
        {

        }
    }
}
