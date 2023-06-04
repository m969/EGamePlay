using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameUtils;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 时间触发组件
    /// </summary>
    public class ExecuteClipTimeTriggerComponent : Component
    {
        public override bool DefaultEnable { get; set; } = false;
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public string TimeValueExpression { get; set; }
        public GameTimer StartTimer { get; set; }
        public GameTimer EndTimer { get; set; }


        public override void Update()
        {
            if (StartTimer != null && StartTimer.IsFinished == false)
            {
                StartTimer.UpdateAsFinish(Time.deltaTime, GetEntity<ExecuteClip>().TriggerEffect);
            }
            if (EndTimer != null && EndTimer.IsFinished == false)
            {
                EndTimer.UpdateAsFinish(Time.deltaTime, GetEntity<ExecuteClip>().EndEffect);
            }
        }

        public override void OnEnable()
        {
            //Log.Debug($"ExecutionTimeTriggerComponent OnEnable {TimeValueExpression} {StartTime} {EndTime}");

            if (!string.IsNullOrEmpty(TimeValueExpression))
            {
                var expression = ExpressionHelper.TryEvaluate(TimeValueExpression);
                StartTime = (int)expression.Value / 1000f;
                StartTimer = new GameTimer(StartTime);
            }
            else if (StartTime > 0)
            {
                StartTimer = new GameTimer(StartTime);
            }
            else
            {
                GetEntity<ExecuteClip>().TriggerEffect();
            }

            if (EndTime > 0)
            {
                EndTimer = new GameTimer(EndTime);
            }
        }
    }
}