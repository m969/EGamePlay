using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public interface IEffectTriggerSystem
    {
        void OnTriggerApplyEffect(Entity effectAssign);
    }

    public struct TriggerContext
    {
        public AbilityTrigger AbilityTrigger;
        public Entity TriggerSource;
        public AbilityItem AbilityItem;
        public Entity Target;
    }

    /// <summary>
    /// 能力效果，如伤害、治疗、施加状态等这些和技能数值、状态相关的效果
    /// </summary>
    public partial class AbilityEffect : Entity
    {
        public bool Enable { get; set; }
        public Ability OwnerAbility => GetParent<Ability>();
        public CombatEntity OwnerEntity => OwnerAbility.OwnerEntity;
        public Entity ParentEntity => OwnerAbility.ParentEntity;
        public Effect EffectConfig { get; set; }


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
            /// 光盾防御效果
            if (this.EffectConfig is ShieldDefenseEffect) AddComponent<EffectShieldDefenseComponent>();
            /// 施加状态效果
            if (this.EffectConfig is AddStatusEffect) AddComponent<EffectAddStatusComponent>();
            /// 自定义效果
            if (this.EffectConfig is CustomEffect) AddComponent<EffectCustomComponent>();
            /// 效果修饰
            var decorators = this.EffectConfig.Decorators;
            if (decorators != null && decorators.Count > 0) AddComponent<EffectDecoratosComponent>();
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
    }
}
