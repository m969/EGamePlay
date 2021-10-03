using System;
using GameUtils;
using UnityEngine;

namespace EGamePlay.Combat
{
    public sealed class WhenInTimeNoDamageCondition : ConditionEntity 
    {
        private GameTimer NoDamageTimer { get; set; }


        public override void Awake(object initData)
        {
            var time = (float)initData;
            NoDamageTimer = new GameTimer(time);
            GetParent<CombatEntity>().ListenActionPoint(ActionPointType.PostReceiveDamage, WhenReceiveDamage);
        }

        public void StartListen(Action whenNoDamageInTimeCallback)
        {
            NoDamageTimer.OnFinish(whenNoDamageInTimeCallback);
            AddComponent<UpdateComponent>();
        }

        public override void Update()
        {
            if (NoDamageTimer.IsRunning)
            {
                NoDamageTimer.UpdateAsFinish(Time.deltaTime);
            }
        }

        private void WhenReceiveDamage(ActionExecution combatAction)
        {
            //Log.Debug($"{GetType().Name}->WhenReceiveDamage");
            NoDamageTimer.Reset();
        }
    }
}