using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            var interval = int.Parse(GetEntity<LogicEntity>().Effect.Interval) / 1000f;
            IntervalTimer = new GameTimer(interval);
        }

        public override void Update()
        {
            IntervalTimer.UpdateAsRepeat(Time.deltaTime, GetEntity<LogicEntity>().ApplyEffect);
        }
    }
}