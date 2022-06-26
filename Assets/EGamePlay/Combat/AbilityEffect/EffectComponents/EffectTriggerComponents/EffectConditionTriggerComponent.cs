using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 条件触发组件
    /// </summary>
    public class EffectConditionTriggerComponent : Component
    {
        public override bool DefaultEnable { get; set; } = false;
        public string ConditionParamValue { get; set; }


        public override void OnEnable()
        {
            var conditionType = GetEntity<AbilityEffect>().EffectConfig.ConditionType;
            var conditionParam = ConditionParamValue;
            Entity.GetParent<StatusAbility>().OwnerEntity.ListenerCondition(conditionType, OnConditionTrigger, conditionParam);
        }

        private void OnConditionTrigger()
        {
            GetEntity<AbilityEffect>().TryAssignEffectToOwner();
        }
    }
}