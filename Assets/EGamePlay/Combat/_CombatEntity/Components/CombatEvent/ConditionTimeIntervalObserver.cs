using GameUtils;
using System;
using UnityEngine;

namespace EGamePlay.Combat
{
    public sealed class ConditionTimeIntervalObserver : Entity, ICombatObserver
    {
        private GameTimer IntervalTimer { get; set; }
        private Action WhenTimeIntervalAction { get; set; }


        public override void Awake(object initData)
        {
            var time = (float)initData;
            IntervalTimer = new GameTimer(time);
        }

        public void StartListen(Action whenTimeIntervalAction)
        {
            //WhenTimeIntervalAction = whenTimeIntervalAction;
            IntervalTimer.OnRepeat(OnRepeat);
            AddComponent<UpdateComponent>();
        }

        private void OnRepeat()
        {
            OnTrigger(this);
        }

        public void OnTrigger(Entity source)
        {
            //WhenTimeIntervalAction?.Invoke();
            var abilityEffect = GetParent<AbilityEffect>();
            abilityEffect.OnObserverTrigger(new TriggerContext(this, source));
        }

        public override void Update()
        {
            if (IntervalTimer.IsRunning)
            {
                IntervalTimer.UpdateAsRepeat(Time.deltaTime);
            }
        }

        //private void WhenInterval()
        //{
        //    Log.Error("ConditionTimeInterval WhenInterval");
        //}
    }
}