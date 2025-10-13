using ECS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 能力触发器
    /// </summary>
    public class AbilityTrigger : EcsEntity
    {
        public Ability OwnerAbility => GetParent<Ability>();
        public CombatEntity OwnerEntity => OwnerAbility.OwnerEntity;
        public EcsEntity ParentEntity => OwnerAbility.ParentEntity;
        public TriggerConfig TriggerConfig { get; set; }
        public string ConditionParamValue { get; set; }
    }
}