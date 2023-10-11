using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public class ApplyEffectEvent { public AbilityEffect AbilityEffect; }
    public enum EffectSourceType { Ability, Execution }

    public interface IEffectTriggerSystem
    {
        void OnTriggerApplyEffect(Entity effectAssign);
    }

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
        public EffectTriggerEventBind TriggerEventBind { get; set; }


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
                TriggerEventBind = AddChild<EffectTriggerEventBind>();
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
            TriggerEventBind?.EnableTriggerBind();
        }

        public void DisableEffect()
        {
            Enable = false;
            foreach (var item in Components.Values)
            {
                item.Enable = false;
            }
        }

        public EffectAssignAction CreateAssignAction(Entity targetEntity)
        {
            //Log.Debug($"TryAssignAllEffectsToTargetWithExecution {targetEntity} {AbilityEffects.Count}");
            if (OwnerEntity.EffectAssignAbility.TryMakeAction(out var action))
            {
                //Log.Debug($"AbilityEffect TryAssignEffectTo {targetEntity} {EffectConfig}");
                action.AssignTarget = targetEntity;
                action.SourceAbility = OwnerAbility;
                action.AbilityEffect = this;
            }
            return action;
        }
    }
}
