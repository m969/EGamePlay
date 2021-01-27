using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;

namespace EGamePlay.Combat.Status
{
    /// <summary>
    /// 状态的生命周期组件
    /// </summary>
    public class StatusLifeTimeComponent : Component
    {
        public override bool Enable { get; set; } = true;
        public GameTimer LifeTimer { get; set; }


        public override void Setup()
        {
            var status = Entity as StatusAbility;
            var lifeTime = status.StatusConfigObject.Duration / 1000f;
            LifeTimer = new GameTimer(lifeTime);
        }

        public override void Update()
        {
            if (LifeTimer.IsRunning)
            {
                LifeTimer.UpdateAsFinish(Time.deltaTime, GetEntity<StatusAbility>().EndAbility);
            }
        }
    }
}