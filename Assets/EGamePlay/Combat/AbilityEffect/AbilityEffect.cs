using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public class ApplyEffectEvent { public AbilityEffect AbilityEffect; }
    public enum EffectSourceType { Ability, Execution }

    /// <summary>
    /// 能力效果
    /// </summary>
    public partial class AbilityEffect : Entity
    {
        public bool Enable { get; set; }
        public AbilityEntity OwnerAbility => GetParent<AbilityEntity>();
        public CombatEntity OwnerEntity => OwnerAbility.OwnerEntity;
        public Effect EffectConfig { get; set; }
        public EffectSourceType EffectSourceType { get; set; }


        public override void Awake(object initData)
        {
            this.EffectConfig = initData as Effect;
            Name = EffectConfig.GetType().Name;
            //Log.Debug($"AbilityEffect Awake {OwnerAbility.Name} {EffectConfig}");

            //伤害效果
            if (this.EffectConfig is DamageEffect damageEffect)
            {
                AddComponent<EffectDamageComponent>();
            }
            //治疗效果
            if (this.EffectConfig is CureEffect cureEffect)
            {
                AddComponent<EffectCureComponent>();
            }
            //施加状态效果
            if (this.EffectConfig is AddStatusEffect addStatusEffect)
            {
                AddComponent<EffectAddStatusComponent>();
            }
            //自定义效果
            if (this.EffectConfig is CustomEffect customEffect)
            {
                if (customEffect.CustomEffectType == "按命中目标数递减百分比伤害")
                {

                }
            }

            //立即触发
            if (EffectConfig.EffectTriggerType == EffectTriggerType.Instant)
            {
                ApplyEffectToParent();
            }
            //行动点触发
            if (EffectConfig.EffectTriggerType == EffectTriggerType.Action)
            {
                AddComponent<EffectActionTriggerComponent>();
            }
            //间隔触发
            if (EffectConfig.EffectTriggerType == EffectTriggerType.Interval)
            {
                Log.Debug($"AbilityEffect Awake EffectConfig.Interval={EffectConfig.Interval}");
                if (!string.IsNullOrEmpty(EffectConfig.Interval))
                {
                    AddComponent<EffectIntervalTriggerComponent>();
                }
            }
            //条件触发
            if (EffectConfig.EffectTriggerType == EffectTriggerType.Condition)
            {
                if (!string.IsNullOrEmpty(EffectConfig.ConditionParam))
                {
                    AddComponent<EffectConditionTriggerComponent>();
                }
            }
        }

        public void EnableEffect()
        {
            Enable = true;
            foreach (var item in Components.Values)
            {
                item.Enable = true;
            }
        }

        public void DisableEffect()
        {
            Enable = false;
            foreach (var item in Components.Values)
            {
                item.Enable = false;
            }
        }

        //public void ApplyEffect()
        //{
        //    Publish(new ApplyEffectEvent() { AbilityEffect = this });
        //}

        public void ApplyEffectToOwner()
        {
            ApplyEffectTo(OwnerAbility.OwnerEntity);
        }

        public void ApplyEffectToParent()
        {
            ApplyEffectTo(OwnerAbility.ParentEntity);
        }

        private void ApplyEffectTo(CombatEntity targetEntity)
        {
            if (OwnerEntity.EffectAssignAbility.TryMakeAction(out var action))
            {
                //Log.Debug($"AbilityEffect ApplyEffectTo {targetEntity} {EffectConfig}");
                action.Target = targetEntity;
                action.SourceAbility = OwnerAbility;
                action.AbilityEffect = this;
                action.ApplyEffectAssign();
            }
        }

        public void ApplyEffectTo(CombatEntity targetEntity, ExecutionEffect executionEffect)
        {
            if (OwnerEntity.EffectAssignAbility.TryMakeAction(out var action))
            {
                //Log.Debug($"AbilityEffect ApplyEffectTo {targetEntity} {EffectConfig}");
                action.Target = targetEntity;
                action.SourceAbility = OwnerAbility;
                action.AbilityEffect = this;
                action.ExecutionEffect = executionEffect;
                action.ApplyEffectAssign();
            }
        }
    }
}
