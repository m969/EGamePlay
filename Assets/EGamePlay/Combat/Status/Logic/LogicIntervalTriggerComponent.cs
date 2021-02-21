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
        public override bool Enable { get; set; } = true;
        public GameTimer IntervalTimer { get; set; }


        public override void Setup()
        {
            //Log.Debug(GetEntity<LogicEntity>().Effect.Interval);
            var expression = ExpressionHelper.ExpressionParser.EvaluateExpression(GetEntity<LogicEntity>().Effect.IntervalValue);
            if (expression.Parameters.ContainsKey("技能等级"))
            {
                expression.Parameters["技能等级"].Value = GetEntity<LogicEntity>().GetParent<StatusAbility>().Level;
            }
#if EGAMEPLAY_EXCEL
            var interval = (float)expression.Value;
            if (expression.Value > 10)
            {
                interval = (int)expression.Value / 1000f;
            }
            IntervalTimer = new GameTimer(interval);
#else
            var interval = (int)expression.Value / 1000f;
            IntervalTimer = new GameTimer(interval);
#endif
        }

        public override void Update()
        {
            if (IntervalTimer != null)
            {
                IntervalTimer.UpdateAsRepeat(Time.deltaTime, GetEntity<LogicEntity>().ApplyEffect);
            }
        }
    }
}