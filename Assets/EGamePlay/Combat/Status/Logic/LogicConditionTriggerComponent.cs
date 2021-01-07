using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat.Status
{
    /// <summary>
    /// 逻辑条件触发组件
    /// </summary>
    public class LogicConditionTriggerComponent : Component
    {
        public override void Setup()
        {
            var conditionType = GetEntity<LogicEntity>().Effect.ConditionType;
            var conditionParam = GetEntity<LogicEntity>().Effect.ConditionParam;
            Entity.GetParent<StatusAbility>().OwnerEntity.ListenerCondition(conditionType, OnConditionTrigger, conditionParam);
        }

        private void OnConditionTrigger()
        {
            GetEntity<LogicEntity>().ApplyEffect();
        }
    }
}