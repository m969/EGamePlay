using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 状态的生命周期组件
    /// </summary>
    public class StatusLifeTimeComponent : Component
    {
        public override bool DefaultEnable { get; set; } = true;
        public GameTimer LifeTimer { get; set; }


        public override void Awake()
        {
            var lifeTime = GetEntity<StatusAbility>().GetDuration() / 1000f;
            LifeTimer = new GameTimer(lifeTime);
        }

        public override void Update()
        {
            if (LifeTimer.IsRunning)
            {
                LifeTimer.UpdateAsFinish(Time.deltaTime, OnLifeTimeFinish);
            }
        }

        private void OnLifeTimeFinish()
        {
            GetEntity<StatusAbility>().EndAbility();
        }
    }
}