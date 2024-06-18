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
    public AnimationClip DamageAnimation;
    public AnimationClip DeadAnimation;
    public AnimationClip[] AnimationClips;
    public float Speed { get; set; } = 1f;
    
    
    private void Start()
    {
        AnimancerComponent.Animator.fireEvents = false;
        AnimancerComponent.States.CreateIfNew(IdleAnimation);
        AnimancerComponent.States.CreateIfNew(RunAnimation);
        AnimancerComponent.States.CreateIfNew(JumpAnimation);
        AnimancerComponent.States.CreateIfNew(AttackAnimation);
        AnimancerComponent.States.CreateIfNew(SkillAnimation);
        AnimancerComponent.States.CreateIfNew(StunAnimation);
        foreach (var item in AnimationClips)
        {
            AnimancerComponent.States.CreateIfNew(item);
        }
    }

    public void Play(AnimationClip clip)
    {
        var state = AnimancerComponent.States.GetOrCreate(clip);
        state.Speed = Speed;
        AnimancerComponent.Play(state);
    }
    
    public void PlayFade(AnimationClip clip)
    {
        var state = AnimancerComponent.States.GetOrCreate(clip);
        state.Speed = Speed;
        AnimancerComponent.Play(state, 0.25f);
    }

    public void TryPlayFade(AnimationClip clip)
    {
        var state = AnimancerComponent.States.GetOrCreate(clip);
        state.Speed = Speed;
        if (AnimancerComponent.IsPlaying(clip))
        {
            return;
        }
        AnimancerComponent.Play(state, 0.25f);
    }
}
