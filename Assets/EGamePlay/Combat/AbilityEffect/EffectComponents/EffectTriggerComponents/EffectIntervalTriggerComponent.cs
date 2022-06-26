using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameUtils;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 间隔触发组件
    /// </summary>
    public class EffectIntervalTriggerComponent : Component
    {
        public override bool DefaultEnable { get; set; } = false;
        public string IntervalValue { get; set; }
        public GameTimer IntervalTimer { get; set; }


        public override void Update()
        {
            if (IntervalTimer != null)
            {
                IntervalTimer.UpdateAsRepeat(Time.deltaTime, GetEntity<AbilityEffect>().TryAssignEffectToParent);
            }
        }

        public override void OnEnable()
        {
            //Log.Debug(GetEntity<LogicEntity>().Effect.Interval);
            var intervalExpression = IntervalValue;
            var expression = ExpressionHelper.TryEvaluate(intervalExpression);
            if (expression.Parameters.ContainsKey("技能等级"))
            {
                expression.Parameters["技能等级"].Value = GetEntity<AbilityEffect>().GetParent<StatusAbility>().Get<AbilityLevelComponent>().Level;
            }
#if EGAMEPLAY_EXCEL
            var interval = (float)expression.Value;
            if (interval > 10)
            {
                interval = interval / 1000f;
            }
            IntervalTimer = new GameTimer(interval);
#else
            var interval = (int)expression.Value / 1000f;
            IntervalTimer = new GameTimer(interval);
#endif
        }
    }
}