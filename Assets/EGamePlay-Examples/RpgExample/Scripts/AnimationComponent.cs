using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimationComponent : MonoBehaviour
{
    public Animancer.AnimancerComponent AnimancerComponent;
    public AnimationClip IdleAnimation;
    public AnimationClip RunAnimation;
    public AnimationClip JumpAnimation;
    public AnimationClip AttackAnimation;
    public AnimationClip SkillAnimation;
    public AnimationClip StunAnimation;
    
    
    private void Start()
    {
        AnimancerComponent.Animator.fireEvents = false;
        AnimancerComponent.States.CreateIfNew(IdleAnimation);
        AnimancerComponent.States.CreateIfNew(RunAnimation);
        AnimancerComponent.States.CreateIfNew(JumpAnimation);
        AnimancerComponent.States.CreateIfNew(AttackAnimation);
        AnimancerComponent.States.CreateIfNew(SkillAnimation);
        AnimancerComponent.States.CreateIfNew(StunAnimation);
    }

    public void Play(AnimationClip clip)
    {
        AnimancerComponent.Play(clip);
    }
    
    public void PlayFade(AnimationClip clip)
    {
        AnimancerComponent.Play(clip, 0.25f);
    }
}
