using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using EGamePlay.Combat.Ability;

namespace EGamePlay.Combat
{
    public sealed class NoDamageTimeCondition : ConditionEntity 
    {
        private GameTimer NoDamageTimer { get; set; }


        public override void Awake(object paramObject)
        {
            var time = (float)paramObject;
            NoDamageTimer = new GameTimer(time);
            GetParent<CombatEntity>().ListenActionPoint(ActionPointType.PostReceiveDamage, WhenReceiveDamage);
        }

        public async void StartListen(Action whenNoDamageInTimeCallback)
        {
            while (true)
            {
                if (IsDispose)
                {
                    break;
                }
                await Task.Delay(100);
                NoDamageTimer.UpdateAsFinish(0.1f, whenNoDamageInTimeCallback);
            }
        }

        private void WhenReceiveDamage(CombatAction combatAction)
        {
            Log.Debug($"{GetType().Name}->WhenReceiveDamage");
            NoDamageTimer.Reset();
        }
    }
}