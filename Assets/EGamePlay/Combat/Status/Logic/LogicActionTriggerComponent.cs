using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat.Status
{
    /// <summary>
    /// 逻辑行动点触发组件
    /// </summary>
    public class LogicActionTriggerComponent : Component
    {
        public override void Setup()
        {
            var actionPointType = GetEntity<LogicEntity>().Effect.ActionPointType;
            GetEntity<LogicEntity>().GetParent<StatusAbility>().OwnerEntity.ListenActionPoint(actionPointType, OnActionPointTrigger);
        }

        private void OnActionPointTrigger(CombatAction combatAction)
        {
            GetEntity<LogicEntity>().ApplyEffect();
        }
    }
}