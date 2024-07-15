using GameUtils;
using System;
using UnityEngine;

namespace EGamePlay.Combat
{
    public sealed class TimeState_TimeIntervalObserveComponent : Component, ICombatObserver
    {
        public override bool DefaultEnable => false;
        private GameTimer IntervalTimer { get; set; }
        private Action WhenTimeIntervalAction { get; set; }


        public override void Awake(object initData)
        {
            var time = (float)initData;
            IntervalTimer = new GameTimer(time);
        }

        public void StartListen(Action whenTimeIntervalAction)
        {
            IntervalTimer.OnRepeat(OnRepeat);
            Enable = true;
            //AddComponent<UpdateComponent>();
        }

        private void OnRepeat()
        {
            OnTrigger(GetEntity<AbilityTrigger>());
        }

        public void OnTrigger(Entity source)
        {
            GetEntity<AbilityTrigger>().OnTrigger(new TriggerContext(this, source));
        }

        public override void Update()
        {
            if (IntervalTimer.IsRunning)
            {
                IntervalTimer.UpdateAsRepeat(Time.deltaTime);
            }
        }
    }
}