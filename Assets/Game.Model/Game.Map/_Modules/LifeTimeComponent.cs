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
        //public override bool DefaultEnable { get; set; } = true;
        public float Duration { get; set; }
        public GameTimer LifeTimer { get; set; }


        //public override void Awake(object initData)
        //{
        //    LifeTimer = new GameTimer((float)initData);
        //}

        //public override void Update()
        //{
        //    if (LifeTimer.IsRunning)
        //    {
        //        LifeTimer.UpdateAsFinish(Time.deltaTime, DestroyEntity);
        //    }
        //}

        //private void DestroyEntity()
        //{
        //    Entity.Destroy(Entity);
        //}
    }
}