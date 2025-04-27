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
            entity.Name = entity.EffectConfig.GetType().Name;

            /// 行动禁制
            if (entity.EffectConfig is ActionControlEffect) entity.AddComponent<EffectActionControlComponent>();
            /// 属性修饰
            if (entity.EffectConfig is AttributeModifyEffect) entity.AddComponent<EffectAttributeModifyComponent>();

            /// 伤害效果
            if (entity.EffectConfig is DamageEffect) entity.AddComponent<EffectDamageComponent>();
            /// 治疗效果
            if (entity.EffectConfig is CureEffect) entity.AddComponent<EffectCureComponent>();
            /// 光盾防御效果
            if (entity.EffectConfig is ShieldDefenseEffect) entity.AddComponent<EffectShieldDefenseComponent>();
            /// 施加状态效果
            if (entity.EffectConfig is AddStatusEffect) entity.AddComponent<EffectAddBuffComponent>();
            /// 自定义效果
            //if (this.EffectConfig is CustomEffect) AddComponent<EffectCustomComponent>();
            /// 效果修饰
            var decorators = entity.EffectConfig.Decorators;
            if (decorators != null && decorators.Count > 0) entity.AddComponent<EffectDecoratorComponent>();
        }

        public void Enable(AbilityEffect entity)
        {
            //foreach (var item in entity.Components.Values)
            //{
            //    item.Enable = true;
            //}
        }

        public void Disable(AbilityEffect entity)
        {
            //foreach (var item in entity.Components.Values)
            //{
            //    item.Enable = false;
            //}
        }

        public void Destroy(AbilityEffect entity)
        {
            entity.Enable = false;
        }

        public static void OnTriggerApplyEffect(EffectAssignAction effectAssign)
        {
            foreach (var item in effectAssign.AbilityEffect.Components.Values)
            {

            }
        }
    }
}