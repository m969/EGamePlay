using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 行动点，一次战斗行动<see cref="IActionExecute"/>会触发战斗实体一系列的行动点<see cref="ActionPoint"/>
    /// </summary>
    public sealed class ActionPoint
    {
        public List<Action<Entity>> Listeners { get; set; } = new List<Action<Entity>>();
        public List<ActionPointObserver> Observers { get; set; } = new List<ActionPointObserver>();


        public void AddListener(Action<Entity> action)
        {
            Listeners.Add(action);
        }

        public void RemoveListener(Action<Entity> action)
        {
            Listeners.Remove(action);
        }

        public void AddObserver(ActionPointObserver action)
        {
            Observers.Add(action);
        }

        public void RemoveObserver(ActionPointObserver action)
        {
            Observers.Remove(action);
        }

        public void TriggerAllObservers(Entity actionExecution)
        {
            if (Listeners.Count > 0)
            {
                for (int i = Listeners.Count - 1; i >= 0; i--)
                {
                    var item = Listeners[i];
                    item.Invoke(actionExecution);
                }
            }
            if (Observers.Count > 0)
            {
                for (int i = Observers.Count - 1; i >= 0; i--)
                {
                    var item = Observers[i];
                    item.OnTrigger(actionExecution);
                }
            }
        }
    }

    [Flags]
    [LabelText("行动点类型")]
    public enum ActionPointType
    {
        [LabelText("（空）")]
        None = 0,

        [LabelText("造成伤害前")]
        PreCauseDamage = 1 << 1,
        [LabelText("承受伤害前")]
        PreReceiveDamage = 1 << 2,

        [LabelText("造成伤害后")]
        PostCauseDamage = 1 << 3,
        [LabelText("承受伤害后")]
        PostReceiveDamage = 1 << 4,

        [LabelText("给予治疗后")]
        PostGiveCure = 1 << 5,
        [LabelText("接受治疗后")]
        PostReceiveCure = 1 << 6,

        [LabelText("赋给技能效果")]
        AssignEffect = 1 << 7,
        [LabelText("接受技能效果")]
        ReceiveEffect = 1 << 8,

        [LabelText("赋加状态后")]
        PostGiveStatus = 1 << 9,
        [LabelText("承受状态后")]
        PostReceiveStatus = 1 << 10,

        [LabelText("给予普攻前")]
        PreGiveAttack = 1 << 11,
        [LabelText("给予普攻后")]
        PostGiveAttack = 1 << 12,

        [LabelText("遭受普攻前")]
        PreReceiveAttack = 1 << 13,
        [LabelText("遭受普攻后")]
        PostReceiveAttack = 1 << 14,

        [LabelText("起跳前")]
        PreJumpTo= 1 << 15,
        [LabelText("起跳后")]
        PostJumpTo = 1 << 16,

        [LabelText("施法前")]
        PreSpell = 1 << 17,
        [LabelText("施法后")]
        PostSpell = 1 << 18,

        [LabelText("赋给普攻效果前")]
        PreGiveAttackEffect = 1 << 19,
        [LabelText("赋给普攻效果后")]
        PostGiveAttackEffect = 1 << 20,
        [LabelText("承受普攻效果前")]
        PreReceiveAttackEffect = 1 << 21,
        [LabelText("承受普攻效果后")]
        PostReceiveAttackEffect = 1 << 22,

        Max,
    }

    /// <summary>
    /// 行动点管理器，在这里管理一个战斗实体所有行动点的添加监听、移除监听、触发流程
    /// </summary>
    public sealed class ActionPointComponent : Component
    {
        private Dictionary<ActionPointType, ActionPoint> ActionPoints { get; set; } = new Dictionary<ActionPointType, ActionPoint>();


        public void AddListener(ActionPointType actionPointType, Action<Entity> action)
        {
            if (!ActionPoints.ContainsKey(actionPointType))
            {
                ActionPoints.Add(actionPointType, new ActionPoint());
            }
            ActionPoints[actionPointType].AddListener(action);
        }

        public void RemoveListener(ActionPointType actionPointType, Action<Entity> action)
        {
            if (ActionPoints.ContainsKey(actionPointType))
            {
                ActionPoints[actionPointType].RemoveListener(action);
            }
        }

        public void AddObserver(ActionPointType actionPointType, ActionPointObserver action)
        {
            if (!ActionPoints.ContainsKey(actionPointType))
            {
                ActionPoints.Add(actionPointType, new ActionPoint());
            }
            ActionPoints[actionPointType].AddObserver(action);
            //Log.Debug($"AddObserver {actionPointType} {ActionPoints[actionPointType].Observers.Count}");
        }

        public void RemoveObserver(ActionPointType actionPointType, ActionPointObserver action)
        {
            if (ActionPoints.ContainsKey(actionPointType))
            {
                ActionPoints[actionPointType].RemoveObserver(action);
            }
        }

        public ActionPoint GetActionPoint(ActionPointType actionPointType)
        {
            if (ActionPoints.TryGetValue(actionPointType, out var actionPoint)) ;
            return actionPoint;
        }

        public void TriggerActionPoint(ActionPointType actionPointType, Entity actionExecution)
        {
            //Log.Debug($"TriggerActionPoint {actionPointType}");
            foreach (var item in ActionPoints)
            {
                if (item.Key.HasFlag(actionPointType))
                {
                    //Log.Debug($"TriggerActionPoint {actionPointType} {item.Value.Observers.Count}");
                    item.Value.TriggerAllObservers(actionExecution);
                }
            }
        }
    }
}