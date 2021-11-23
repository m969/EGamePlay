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
        public AbilityExecution ParentExecution => GetParent<AbilityExecution>();


        public override void Awake(object initData)
        {
            AbilityEffect = initData as AbilityEffect;
            Name = AbilityEffect.Name;
            //Log.Debug($"ExecutionEffect Awake {AbilityEffect.OwnerAbility.Name} {AbilityEffect.EffectConfig}");

            foreach (var component in AbilityEffect.Components.Values)
            {
                //时间到直接应用能力给目标效果
                if (component is EffectExecutionApplyToTargetComponent applyToTargetComponent)
                {
                    var executionApplyToTargetComponent = AddComponent<ExecutionApplyToTargetComponent>();
                    executionApplyToTargetComponent.EffectApplyType = applyToTargetComponent.EffectApplyType;
                    if (applyToTargetComponent.TriggerTime > 0)
                    {
                        AddComponent<ExecutionTimeTriggerComponent>().TriggerTime = (float)applyToTargetComponent.TriggerTime;
                    }
                    else
                    {
                        ApplyEffect();
                    }
                }
                //时间到生成碰撞体，碰撞体再触发应用能力效果
                if (component is EffectExecutionSpawnItemComponent spawnItemComponent)
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
                //时间到播放动作
                if (component is EffectExecutionAnimationComponent animationComponent)
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

            if (AbilityEffect.EffectConfig is CustomEffect customEffect)
            {
                if (customEffect.CustomEffectType == "按命中目标数递减百分比伤害")
                {
                    if (Parent is AbilityItem abilityItem)
                    {
                        abilityItem.GetComponent<ExecutionEffectComponent>().DamageExecutionEffect.AddComponent<ExecutionDamageReduceWithTargetCountComponent>(AbilityEffect);
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
            //AbilityEffect.ApplyEffectToOwner();
            this.Publish(new ExecutionEffectEvent() { ExecutionEffect = this });
        }

        public void ApplyEffectTo(CombatEntity targetEntity)
        {
            AbilityEffect.ApplyEffectTo(targetEntity, this);
        }
    }
}
