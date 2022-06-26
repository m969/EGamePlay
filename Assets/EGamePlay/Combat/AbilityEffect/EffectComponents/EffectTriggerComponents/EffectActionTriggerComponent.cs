using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 行动点触发组件
    /// </summary>
    public class EffectActionTriggerComponent : Component
    {
        public override bool DefaultEnable { get; set; } = false;


        public override void OnEnable()
        {
            var actionPointType = GetEntity<AbilityEffect>().EffectConfig.ActionPointType;
            GetEntity<AbilityEffect>().GetParent<StatusAbility>().OwnerEntity.ListenActionPoint(actionPointType, OnActionPointTrigger);
        }

        private void OnActionPointTrigger(Entity combatAction)
        {
            GetEntity<AbilityEffect>().TryTriggerEffect();
        }
    }
}