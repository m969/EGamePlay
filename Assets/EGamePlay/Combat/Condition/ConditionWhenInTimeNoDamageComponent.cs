using System;
using GameUtils;
using UnityEngine;

namespace EGamePlay.Combat
{
    public sealed class ConditionWhenInTimeNoDamageComponent : Component 
    {
        private GameTimer NoDamageTimer { get; set; }
        public override bool DefaultEnable => false;


        public override void Awake(object initData)
        {
            var time = (float)initData;
            NoDamageTimer = new GameTimer(time);
            Entity.GetParent<CombatEntity>().ListenActionPoint(ActionPointType.PostReceiveDamage, WhenReceiveDamage);
        }

        public void StartListen(Action whenNoDamageInTimeCallback)
        {
            NoDamageTimer.OnFinish(whenNoDamageInTimeCallback);
            Enable = true;
            //Entity.AddComponent<UpdateComponent>();
        }

        public override void Update()
        {
            if (NoDamageTimer.IsRunning)
            {
                NoDamageTimer.UpdateAsFinish(Time.deltaTime);
            }
        }

        private void WhenReceiveDamage(Entity combatAction)
        {
            //Log.Debug($"{GetType().Name}->WhenReceiveDamage");
            NoDamageTimer.Reset();
        }
    }
}