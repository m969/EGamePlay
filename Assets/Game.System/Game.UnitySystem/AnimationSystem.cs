using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using Animancer;
using System.ComponentModel;

namespace ECSGame
{
    public class AnimationSystem : AComponentSystem<EcsEntity, AnimationComponent>,
IAwake<EcsEntity, AnimationComponent>
    {
        public void Awake(EcsEntity entity, AnimationComponent component)
        {
            var modelTrans = entity.GetComponent<ModelViewComponent>().ModelTrans;
            GetClips(entity, modelTrans.GetComponent<EGamePlay.AnimationComponent>());
        }

        public void GetClips(EcsEntity entity, EGamePlay.AnimationComponent animationComponent)
        {
            var component = entity.GetComponent<ECSGame.AnimationComponent>();
            component.AnimancerComponent = animationComponent.AnimancerComponent;
            component.IdleAnimation = animationComponent.IdleAnimation;
            component.RunAnimation = animationComponent.RunAnimation;
            component.JumpAnimation = animationComponent.JumpAnimation;
            component.AttackAnimation = animationComponent.AttackAnimation;
            component.SkillAnimation = animationComponent.SkillAnimation;
            component.StunAnimation = animationComponent.StunAnimation;
            component.AnimationClips = animationComponent.AnimationClips;

            var modelTrans = entity.GetComponent<ModelViewComponent>().ModelTrans;
            var animancerComponent = modelTrans.GetComponentInChildren<NamedAnimancerComponent>();
            animancerComponent.Animator.fireEvents = false;
            animancerComponent.States.CreateIfNew(component.IdleAnimation);
            animancerComponent.States.CreateIfNew(component.RunAnimation);
            animancerComponent.States.CreateIfNew(component.JumpAnimation);
            animancerComponent.States.CreateIfNew(component.AttackAnimation);
            animancerComponent.States.CreateIfNew(component.SkillAnimation);
            animancerComponent.States.CreateIfNew(component.StunAnimation);
            foreach (var item in component.AnimationClips)
            {
                animancerComponent.States.CreateIfNew(item);
            }
        }

        public static void Play(EcsEntity entity, AnimationClip clip)
        {
            var component = entity.GetComponent<AnimationComponent>();
            var state = component.AnimancerComponent.States.GetOrCreate(clip);
            state.Speed = component.Speed;
            component.AnimancerComponent.Play(state);
        }

        public static void PlayFade(EcsEntity entity, AnimationClip clip)
        {
            var component = entity.GetComponent<AnimationComponent>();
            var state = component.AnimancerComponent.States.GetOrCreate(clip);
            state.Speed = component.Speed;
            component.AnimancerComponent.Play(state, 0.25f);
        }

        public static void TryPlayFade(EcsEntity entity, AnimationClip clip)
        {
            var component = entity.GetComponent<AnimationComponent>();
            var state = component.AnimancerComponent.States.GetOrCreate(clip);
            state.Speed = component.Speed;
            if (component.AnimancerComponent.IsPlaying(clip))
            {
                return;
            }
            component.AnimancerComponent.Play(state, 0.25f);
        }
    }
}
