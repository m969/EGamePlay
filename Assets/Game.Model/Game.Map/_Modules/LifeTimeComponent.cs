using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;

namespace EGamePlay
{
    /// <summary>
    /// 生命周期组件
    /// </summary>
    public class LifeTimeComponent : EcsComponent
    {
        public float Duration { get; set; }
        public GameTimer LifeTimer { get; set; }
    }
}