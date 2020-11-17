using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat.Status
{
    /// <summary>
    /// 状态的生命周期组件
    /// </summary>
    public class StatusLifeTimeComponent : Component
    {
        public GameTimer LifeTimer { get; set; }


        public override void Setup()
        {
            var status = Entity as StatusAbilityEntity;
            var lifeTime = status.StatusConfigObject.Duration / 1000f;
            LifeTimer = new GameTimer(lifeTime);
            LifeTimer.OnFinish(() => { Entity.Dispose(); });
        }

        public override void Update()
        {
            if (LifeTimer.IsRunning)
            {
                LifeTimer.UpdateAsFinish(Time.deltaTime);
            }
        }
    }
}