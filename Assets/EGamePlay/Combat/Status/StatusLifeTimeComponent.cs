using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat.Status
{
    public class StatusLifeTimeComponent : Component
    {
        public GameTimer LifeTimer { get; set; }


        public override void Setup()
        {
            var status = Parent as StatusEntity;
            var lifeTime = status.StatusConfigObject.DurationConfig.Duration / 1000f;
            LifeTimer = new GameTimer(lifeTime);
            LifeTimer.OnFinish(() => { Parent.Destroy(); });
        }

        public override void Update()
        {
            LifeTimer.UpdateAsFinish(Time.deltaTime);
        }
    }
}