using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System;
using ET;
using ECSGame;

namespace EGamePlay
{
    public class ExecuteClipViewSystem : AEntitySystem<ExecuteClip>,
        IAwake<ExecuteClip>,
        IEnable<ExecuteClip>,
        IDisable<ExecuteClip>,
        IUpdate<ExecuteClip>,
        IOnTriggerClip,
        IOnEndClip
    {
        public void Awake(ExecuteClip entity)
        {
            entity.Name = entity.ClipData.GetType().Name;
            var clipType = entity.ClipData.ExecuteClipType;
            /// 播放特效效果
            if (clipType == ExecuteClipType.ParticleEffect)
            {
                var animationEffect = entity.ClipData.ParticleEffectData;
                entity.AddComponent<ExecuteParticleEffectComponent>().ParticleEffectPrefab = animationEffect.ParticleEffect;
            }
        }

        public void Enable(ExecuteClip entity)
        {

        }

        public void Disable(ExecuteClip entity)
        {

        }

        public void Update(ExecuteClip entity)
        {

        }

        public void OnTriggerClip(ExecuteClip entity)
        {
            var clipType = entity.ClipData.ExecuteClipType;
            if (clipType == ExecuteClipType.Animation)
            {
                Trigger_Animation(entity);
            }
            if (clipType == ExecuteClipType.ParticleEffect)
            {
                Trigger_ParticleEffect(entity);
            }
        }

        public void OnEndClip(ExecuteClip entity)
        {
            var clipType = entity.ClipData.ExecuteClipType;
            if (clipType == ExecuteClipType.ParticleEffect)
            {
                TriggerEnd_ParticleEffect(entity);
            }
        }

        /// <summary>
        /// 生成粒子特效
        /// </summary>
        /// <param name="entity"></param>
        private static void Trigger_ParticleEffect(ExecuteClip entity)
        {
            var component = entity.GetComponent<ExecuteParticleEffectComponent>();
            var execution = entity.GetParent<AbilityExecution>();
            component.ParticleEffectObj = GameObject.Instantiate(component.ParticleEffectPrefab, execution.Position, execution.Rotation);
        }

        /// <summary>
        /// 销毁粒子特效
        /// </summary>
        /// <param name="entity"></param>
        private static void TriggerEnd_ParticleEffect(ExecuteClip entity)
        {
            var component = entity.GetComponent<ExecuteParticleEffectComponent>();
            GameObject.Destroy(component.ParticleEffectObj);
        }

        private static void Trigger_Animation(ExecuteClip entity)
        {
            var combatEntity = entity.GetParent<AbilityExecution>().OwnerEntity;
            AnimationSystem.PlayFade(combatEntity.Actor, entity.ClipData.AnimationData.AnimationClip);
        }
    }
}