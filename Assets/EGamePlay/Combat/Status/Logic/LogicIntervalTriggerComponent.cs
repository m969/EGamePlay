using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameUtils;

namespace EGamePlay.Combat.Status
{
    /// <summary>
    /// 逻辑间隔触发组件
    /// </summary>
    public class LogicIntervalTriggerComponent : Component
    {
        public GameTimer IntervalTimer { get; set; }


        public override void Setup()
        {
            //Log.Debug(GetEntity<LogicEntity>().Effect.Interval);
            var expression = ExpressionHelper.ExpressionParser.EvaluateExpression(GetEntity<LogicEntity>().Effect.Interval);
            if (expression.Parameters.ContainsKey("技能等级"))
            {
                expression.Parameters["技能等级"].Value = GetEntity<LogicEntity>().GetParent<StatusAbility>().Level;
            }
            var interval = (int)expression.Value / 1000f;
            IntervalTimer = new GameTimer(interval);
        }

        public override void Update()
        {
            IntervalTimer.UpdateAsRepeat(Time.deltaTime, GetEntity<LogicEntity>().ApplyEffect);
        }
    }
}