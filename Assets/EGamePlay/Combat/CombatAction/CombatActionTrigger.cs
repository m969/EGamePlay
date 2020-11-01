using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战斗行为触发器，在这里管理所有角色战斗行为的监听、移除监听、触发流程
    /// </summary>
    public sealed class CombatActionTrigger
    {
        public Dictionary<CombatActionType, Action<CombatAction>> CombatActions { get; set; } = new Dictionary<CombatActionType, Action<CombatAction>>();


        public void Initialize()
        {
            CombatActions.Add(CombatActionType.CauseDamage, null);
        }

        public void AddListener(CombatActionType actionType, Action<CombatAction> action)
        {
            CombatActions[actionType] += action;
        }

        public void RemoveListener(CombatActionType actionType, Action<CombatAction> action)
        {
            CombatActions[actionType] -= action;
        }

        public void CallAction(CombatActionType actionType, CombatAction action)
        {
            CombatActions[actionType]?.Invoke(action);
        }
    }
}