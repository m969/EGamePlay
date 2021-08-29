using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;

namespace EGamePlay
{
    /// <summary>
    /// 生命周期组件
    /// </summary>
    public class LifeTimeComponent : Component
    {
        public override bool Enable { get; set; } = true;
        public GameTimer LifeTimer { get; set; }


        public override void Setup(object initData)
        {
            LifeTimer = new GameTimer((float)initData);
        }

        public override void Update()
        {
            if (LifeTimer.IsRunning)
            {
                LifeTimer.UpdateAsFinish(Time.deltaTime, DestroyEntity);
            }
        }

        private void DestroyEntity()
        {
            Entity.Destroy(Entity);
        }
    }
}