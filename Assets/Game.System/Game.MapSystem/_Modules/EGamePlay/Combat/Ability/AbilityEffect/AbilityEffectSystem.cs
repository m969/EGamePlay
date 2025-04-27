using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System;
using ET;

namespace EGamePlay
{
    public class AbilityEffectSystem : AEntitySystem<AbilityEffect>,
        IAwake<AbilityEffect>,
        IEnable<AbilityEffect>,
        IDisable<AbilityEffect>,
        IDestroy<AbilityEffect>
    {
        public void Awake(AbilityEffect entity)
        {
        }

        public void Enable(AbilityEffect entity)
        {

        }

        public void Disable(AbilityEffect entity)
        {

        }

        public void Destroy(AbilityEffect entity)
        {
            entity.Enable = false;
        }

        public static AbilityEffect Create(Ability ability, Effect effectConfig)
        {
            var abilityEffect = ability.AddChild<AbilityEffect>();
            abilityEffect.EffectConfig = effectConfig;
            abilityEffect.Name = effectConfig.GetType().Name;

            /// 行动禁制
            if (effectConfig is ActionControlEffect) abilityEffect.AddComponent<EffectActionControlComponent>();
            /// 属性修饰
            if (effectConfig is AttributeModifyEffect) abilityEffect.AddComponent<EffectAttributeModifyComponent>();

            /// 伤害效果
            if (effectConfig is DamageEffect) abilityEffect.AddComponent<EffectDamageComponent>();
            /// 治疗效果
            if (effectConfig is CureEffect) abilityEffect.AddComponent<EffectCureComponent>();
            /// 光盾防御效果
            if (effectConfig is ShieldDefenseEffect) abilityEffect.AddComponent<EffectShieldDefenseComponent>();
            /// 施加状态效果
            if (effectConfig is AddStatusEffect) abilityEffect.AddComponent<EffectAddBuffComponent>();
            /// 效果修饰
            var decorators = abilityEffect.EffectConfig.Decorators;
            if (decorators != null && decorators.Count > 0) abilityEffect.AddComponent<EffectDecoratorComponent>();
            return abilityEffect;
        }

        public static void TriggerApply(EffectAssignAction effectAssign)
        {
            //ConsoleLog.Debug("AbilityEffectSystem OnTriggerApplyEffect");
            var abilityEffect = effectAssign.AbilityEffect;
            foreach (var item in effectAssign.AbilityEffect.Components.Values)
            {
                if (item is EffectActionControlComponent) EffectActionControlSystem.TriggerApply(abilityEffect, effectAssign);
                if (item is EffectAddBuffComponent) EffectAddBuffSystem.TriggerApply(abilityEffect, effectAssign);
                if (item is EffectAttributeModifyComponent) EffectAttributeSystem.TriggerApply(abilityEffect, effectAssign);
                if (item is EffectCureComponent) EffectCureSystem.TriggerApply(abilityEffect, effectAssign);
                if (item is EffectDamageComponent) EffectDamageSystem.TriggerApply(abilityEffect, effectAssign);
                if (item is EffectShieldDefenseComponent) EffectShieldSystem.TriggerApply(abilityEffect, effectAssign);
            }
        }
    }
}