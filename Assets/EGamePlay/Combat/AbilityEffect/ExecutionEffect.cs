using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public class ExecutionEffectEvent
    {
        public ExecutionEffect ExecutionEffect;
    }

    /// <summary>
    /// 执行体效果
    /// </summary>
    public partial class ExecutionEffect : Entity
    {
        public AbilityEffect AbilityEffect { get; set; }


        public override void Awake(object initData)
        {
            AbilityEffect = initData as AbilityEffect;

            foreach (var component in AbilityEffect.Components.Values)
            {
                if (component is EffectSpawnItemComponent spawnItemComponent)
                {
                    AddComponent<ExecutionSpawnItemComponent>().EffectSpawnItemComponent = spawnItemComponent;
                    if (spawnItemComponent.ColliderSpawnData.ColliderSpawnEmitter.time > 0)
                    {
                        AddComponent<ExecutionTimeTriggerComponent>().TriggerTime = (float)spawnItemComponent.ColliderSpawnData.ColliderSpawnEmitter.time;
                    }
                    else
                    {
                        ApplyEffect();
                    }
                }
                if (component is EffectAnimationComponent animationComponent)
                {
                    AddComponent<ExecutionAnimationComponent>().EffectAnimationComponent = animationComponent;
                    if (animationComponent.AnimationData.StartTime > 0)
                    {
                        AddComponent<ExecutionTimeTriggerComponent>().TriggerTime = (float)animationComponent.AnimationData.StartTime;
                    }
                    else
                    {
                        ApplyEffect();
                    }
                }
            }

            foreach (var item in Components.Values)
            {
                item.Enable = true;
            }
        }

        public void ApplyEffect()
        {
            //Log.Debug($"ExecutionEffect ApplyEffect");
            AbilityEffect.ApplyEffect();
            Publish(new ExecutionEffectEvent() { ExecutionEffect = this });
        }
    }
}
