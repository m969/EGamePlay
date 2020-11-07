using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战斗行为概念，造成伤害、承受伤害、接受治疗、接受buff等属于战斗行为表现
    /// 战斗行为随战斗行动触发，并且会携带战斗行动的引用
    /// </summary>
    public sealed class CombatAction
    {
        public Action<CombatOperation> Action { get; set; }
    }

    public enum CombatActionType
    {
        CauseDamage,
        ReceiveDamage,
        GiveCure,
        ReceiveCure,
        AssignEffect,
        ReceiveEffect,

        Max,
    }

    /// <summary>
    /// 战斗行为触发器，在这里管理所有角色战斗行为的监听、移除监听、触发流程
    /// </summary>
    public sealed class CombatActionTrigger
    {
        private Dictionary<CombatActionType, CombatAction> CombatActions { get; set; } = new Dictionary<CombatActionType, CombatAction>();


        public void Initialize()
        {
            CombatActions.Add(CombatActionType.CauseDamage, new CombatAction());
            CombatActions.Add(CombatActionType.ReceiveDamage, new CombatAction());
        }

        public void AddListener(CombatActionType actionType, Action<CombatOperation> action)
        {
            CombatActions[actionType].Action += action;
        }

        public void RemoveListener(CombatActionType actionType, Action<CombatOperation> action)
        {
            CombatActions[actionType].Action -= action;
        }

        public void TriggerAction(CombatActionType actionType, CombatOperation action)
        {
            CombatActions[actionType].Action?.Invoke(action);
        }
    }
}