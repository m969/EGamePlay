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
    public class ExecuteTimeTriggerComponent : ECS.EcsComponent
    {
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public string TimeValueExpression { get; set; }
        public GameTimer StartTimer { get; set; }
        public GameTimer EndTimer { get; set; }
    }
}