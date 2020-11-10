using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 行动点，一次战斗行动<see cref="CombatAction"/>会触发战斗实体一系列的行动点
    /// </summary>
    public sealed class ActionPoint
    {
        public List<Action<CombatAction>> Listeners { get; set; } = new List<Action<CombatAction>>();
    }

    public enum ActionPointType
    {
        PreCauseDamage,//造成伤害前
        PreReceiveDamage,//承受伤害前

        PostCauseDamage,//造成伤害后
        PostReceiveDamage,//承受伤害后

        GiveCure,
        ReceiveCure,

        AssignEffect,
        ReceiveEffect,

        Max,
    }

    /// <summary>
    /// 行动点管理器，在这里管理一个战斗实体所有行动点的添加监听、移除监听、触发流程
    /// </summary>
    public sealed class ActionPointManager
    {
        private Dictionary<ActionPointType, ActionPoint> ActionPoints { get; set; } = new Dictionary<ActionPointType, ActionPoint>();


        public void Initialize()
        {
            ActionPoints.Add(ActionPointType.PostCauseDamage, new ActionPoint());
            ActionPoints.Add(ActionPointType.PostReceiveDamage, new ActionPoint());
        }

        public void AddListener(ActionPointType actionPointType, Action<CombatAction> action)
        {
            ActionPoints[actionPointType].Listeners.Add(action);
        }

        public void RemoveListener(ActionPointType actionPointType, Action<CombatAction> action)
        {
            ActionPoints[actionPointType].Listeners.Remove(action);
        }

        public void TriggerActionPoint(ActionPointType actionPointType, CombatAction action)
        {
            foreach (var item in ActionPoints[actionPointType].Listeners)
            {
                item.Invoke(action);
            }
        }
    }
}