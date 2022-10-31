using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public class ApplyEffectEvent { public AbilityEffect AbilityEffect; }
    public enum EffectSourceType { Ability, Execution }

    /// <summary>
    /// 能力效果，如伤害、治疗、施加状态等这些和技能数值、状态相关的效果
    /// </summary>
    public partial class AbilityEffect : Entity
    {
        public bool Enable { get; set; }
        public Entity OwnerAbility => Parent;
        public CombatEntity OwnerEntity => (OwnerAbility as IAbilityEntity).OwnerEntity;
        public Effect EffectConfig { get; set; }
        public EffectSourceType EffectSourceType { get; set; }


        public override void Awake(object initData)
        {
            this.EffectConfig = initData as Effect;
            Name = EffectConfig.GetType().Name;
            //Log.Debug($"AbilityEffect Awake {OwnerAbility.Name} {EffectConfig}");

            /// 行动禁制
            if (this.EffectConfig is ActionControlEffect) AddComponent<EffectActionControlComponent>();
            /// 属性修饰
            if (this.EffectConfig is AttributeModifyEffect) AddComponent<EffectAttributeModifyComponent>();

            /// 伤害效果
            if (this.EffectConfig is DamageEffect) AddComponent<EffectDamageComponent>();
            /// 治疗效果
            if (this.EffectConfig is CureEffect) AddComponent<EffectCureComponent>();
            /// 施加状态效果
            if (this.EffectConfig is AddStatusEffect) AddComponent<EffectAddStatusComponent>();
            /// 自定义效果
            if (this.EffectConfig is CustomEffect) AddComponent<EffectCustomComponent>();
            /// 效果修饰
            AddComponent<EffectDecoratosComponent>();

            var triggable = !(this.EffectConfig is ActionControlEffect) && !(this.EffectConfig is AttributeModifyEffect);
            if (triggable)
            {
                /// 立即触发
                if (EffectConfig.EffectTriggerType == EffectTriggerType.Instant) TryAssignEffectToParent();
                /// 行动点触发
                var isAction = EffectConfig.EffectTriggerType == EffectTriggerType.Action;
                if (isAction) AddComponent<EffectActionTriggerComponent>();
                /// 间隔触发
                var isInterval = EffectConfig.EffectTriggerType == EffectTriggerType.Interval && !string.IsNullOrEmpty(EffectConfig.Interval);
                if (isInterval) AddComponent<EffectIntervalTriggerComponent>();
                /// 条件触发
                var isCondition = EffectConfig.EffectTriggerType == EffectTriggerType.Condition && !string.IsNullOrEmpty(EffectConfig.ConditionParam);
                if (isCondition) AddComponent<EffectConditionTriggerComponent>();
            }
        }

        public override void OnDestroy()
        {
            DisableEffect();
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

        /// <summary>   尝试触发效果   </summary>
        public void TryTriggerEffect()
        {
            this.FireEvent(nameof(TryTriggerEffect));
        }

        /// <summary>   尝试触发效果   </summary>
        public void TryTriggerEffectWithAction(IActionExecution action)
        {
            //this.FireEvent(nameof(TryTriggerEffect));
            TryTriggerEffectToAction(action);
        }

        /// <summary>   尝试将效果赋给施术者   </summary>
        public void TryAssignEffectToOwner()
        {
            TryAssignEffectTo((OwnerAbility as IAbilityEntity).OwnerEntity);
        }

        /// <summary>   尝试将效果赋给父对象   </summary>
        public void TryAssignEffectToParent()
        {
            TryAssignEffectTo((OwnerAbility as IAbilityEntity).ParentEntity);
        }

        /// <summary>   尝试将效果赋给目标实体   </summary>
        public void TryAssignEffectTo(CombatEntity targetEntity)
        {
            if (OwnerEntity.EffectAssignAbility.TryMakeAction(out var action))
            {
                //Log.Debug($"AbilityEffect TryAssignEffectTo {targetEntity} {EffectConfig}");
                action.Target = targetEntity;
                action.SourceAbility = OwnerAbility;
                action.AbilityEffect = this;
                action.ApplyEffectAssign();
            }
        }

        /// <summary>   尝试将效果赋给目标实体   </summary>
        public void TryAssignEffectToTargetWithAbilityItem(CombatEntity targetEntity, AbilityItem abilityItem)
        {
            if (OwnerEntity.EffectAssignAbility.TryMakeAction(out var action))
            {
                //Log.Debug($"AbilityEffect ApplyEffectTo {targetEntity} {EffectConfig}");
                action.Target = targetEntity;
                action.SourceAbility = OwnerAbility;
                action.AbilityEffect = this;
                action.AbilityItem = abilityItem;
                action.ApplyEffectAssign();
            }
        }

        /// <summary>   尝试将效果赋给目标行动   </summary>
        public void TryTriggerEffectToAction(IActionExecution action)
        {
            if (OwnerEntity.EffectAssignAbility.TryMakeAction(out var assignAction))
            {
                //Log.Debug($"AbilityEffect ApplyEffectTo {targetEntity} {EffectConfig}");
                assignAction.SourceAbility = OwnerAbility;
                assignAction.AbilityEffect = this;
                assignAction.TargetAction = action;
                assignAction.ApplyEffectAssign();
            }
        }

        /// <summary>   开始赋给效果   </summary>
        public void StartAssignEffect(EffectAssignAction effectAssignAction)
        {
            this.FireEvent(nameof(StartAssignEffect), effectAssignAction);
        }
    }
}
