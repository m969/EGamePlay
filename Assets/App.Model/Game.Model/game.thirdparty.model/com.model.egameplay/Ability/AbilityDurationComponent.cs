using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 能力持续时间组件
    /// </summary>
    public class AbilityDurationComponent : EcsComponent
    {
        public GameTimer LifeTimer { get; set; }
        public float Duration { get; set; }
    }
}