using System;
using GameUtils;

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

        public async void StartListen(Action whenNoDamageInTimeCallback)
        {
            while (true)
            {
                if (IsDisposed)
                {
                    break;
                }
                await ET.TimerComponent.Instance.WaitAsync(100);
                NoDamageTimer.UpdateAsFinish(0.1f, whenNoDamageInTimeCallback);
            }
        }

        private void WhenReceiveDamage(ActionExecution combatAction)
        {
            //Log.Debug($"{GetType().Name}->WhenReceiveDamage");
            NoDamageTimer.Reset();
        }
    }
}