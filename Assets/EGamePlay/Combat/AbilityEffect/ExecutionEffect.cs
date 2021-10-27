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

            foreach (var component in AbilityEffect.Components.Values)
            {
                //时间到直接应用能力效果
                if (component is EffectTimeTriggerComponent timeTriggerComponent)
                {
                    if (timeTriggerComponent.TriggerTime > 0)
                    {
                        AddComponent<ExecutionTimeTriggerComponent>().TriggerTime = (float)timeTriggerComponent.TriggerTime;
                    }
                    else
                    {
                        ApplyEffect();
                    }
                }
                //时间到生成碰撞体，碰撞体再触发应用能力效果
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
                //时间到播放动作
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
            if (ParentExecution is SkillExecution skillExecution)
            {
                if (skillExecution.InputTarget != null)
                {
                    ApplyEffectTo(skillExecution.InputTarget);
                }
            }
            this.Publish(new ExecutionEffectEvent() { ExecutionEffect = this });
        }

        public void ApplyEffectTo(CombatEntity targetEntity)
        {
            if (AbilityEffect.OwnerEntity.EffectAssignAbility.TryCreateAction(out var action))
            {
                //Log.Debug($"ExecutionEffect ApplyEffectTo {targetEntity} {AbilityEffect.EffectConfig}");
                action.Target = targetEntity;
                action.SourceAbility = AbilityEffect.OwnerAbility;
                action.AbilityEffect = AbilityEffect;
                action.ExecutionEffect = this;
                action.ApplyEffectAssign();
            }
        }
    }
}
