using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public struct TriggerContext
    {
        public AbilityTrigger AbilityTrigger;
        public EcsEntity TriggerSource;
        public AbilityItem AbilityItem;
        public EcsEntity Target;
    }

    /// <summary>
    /// 能力效果，如伤害、治疗、施加状态等这些和技能数值、状态相关的效果
    /// </summary>
    public partial class AbilityEffect : EcsEntity
    {
        public string Name { get; set; }
        public Ability OwnerAbility => GetParent<Ability>();
        public CombatEntity OwnerEntity => OwnerAbility.OwnerEntity;
        public EcsEntity ParentEntity => OwnerAbility.ParentEntity;
        public Effect EffectConfig { get; set; }
    }
}
