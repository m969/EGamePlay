using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 行动点概念，一次战斗行动会触发一系列造成伤害、承受伤害、给予治疗、接受治疗等行动点
    /// 行动点随战斗行动触发，并且会携带战斗行动的引用
    /// </summary>
    public sealed class ActionPoint
    {
        //public Action<CombatOperation> Action { get; set; }
        public List<Action<CombatAction>> Actions { get; set; } = new List<Action<CombatAction>>();
    }

    public enum ActionPointType
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
    /// 行动点事件管理器，在这里管理战斗实体所有行动点事件的监听、移除监听、触发流程
    /// </summary>
    public sealed class ActionPointEventManager
    {
        private Dictionary<ActionPointType, ActionPoint> ActionPoints { get; set; } = new Dictionary<ActionPointType, ActionPoint>();


        public void Initialize()
        {
            ActionPoints.Add(ActionPointType.CauseDamage, new ActionPoint());
            ActionPoints.Add(ActionPointType.ReceiveDamage, new ActionPoint());
        }

        public void AddListener(ActionPointType actionType, Action<CombatAction> action)
        {
            //ActionPoints[actionType].Action += action;
            ActionPoints[actionType].Actions.Add(action);
        }

        public void RemoveListener(ActionPointType actionType, Action<CombatAction> action)
        {
            //ActionPoints[actionType].Action -= action;
            ActionPoints[actionType].Actions.Remove(action);
        }

        public void TriggerAction(ActionPointType actionType, CombatAction action)
        {
            //ActionPoints[actionType].Action?.Invoke(action);
            foreach (var item in ActionPoints[actionType].Actions)
            {
                item.Invoke(action);
            }
        }
    }
}