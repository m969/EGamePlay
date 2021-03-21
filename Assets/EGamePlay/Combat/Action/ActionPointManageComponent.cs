using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 行动点，一次战斗行动<see cref="CombatAction"/>会触发战斗实体一系列的行动点
    /// </summary>
    public sealed class ActionPoint
    {
        public List<Action<CombatAction>> Listeners { get; set; } = new List<Action<CombatAction>>();
    }

    [Flags]
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

        [LabelText("赋给效果")]
        AssignEffect = 1 << 7,
        [LabelText("接受效果")]
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

        Max,
    }

    /// <summary>
    /// 行动点管理器，在这里管理一个战斗实体所有行动点的添加监听、移除监听、触发流程
    /// </summary>
    public sealed class ActionPointManageComponent : Component
    {
        private Dictionary<ActionPointType, ActionPoint> ActionPoints { get; set; } = new Dictionary<ActionPointType, ActionPoint>();


        public override void Setup()
        {
            base.Setup();
        }

        public void AddListener(ActionPointType actionPointType, Action<CombatAction> action)
        {
            if (!ActionPoints.ContainsKey(actionPointType))
            {
                ActionPoints.Add(actionPointType, new ActionPoint());
            }
            ActionPoints[actionPointType].Listeners.Add(action);
        }

        public void RemoveListener(ActionPointType actionPointType, Action<CombatAction> action)
        {
            if (ActionPoints.ContainsKey(actionPointType))
            {
                ActionPoints[actionPointType].Listeners.Remove(action);
            }
        }

        public void TriggerActionPoint(ActionPointType actionPointType, CombatAction action)
        {
            if (ActionPoints.ContainsKey(actionPointType) && ActionPoints[actionPointType].Listeners.Count > 0)
            {
                for (int i = ActionPoints[actionPointType].Listeners.Count - 1; i >= 0; i--)
                {
                    var item = ActionPoints[actionPointType].Listeners[i];
                    item.Invoke(action);
                }
            }
        }
    }
}