using System;
using GameUtils;
using UnityEngine;

namespace EGamePlay.Combat
{
    public sealed class TimeState_WhenInTimeNoDamageObserver : Entity, ICombatObserver
    {
        private GameTimer NoDamageTimer { get; set; }
        private Action WhenNoDamageInTimeCallback { get; set; }


        public override void Awake(object initData)
        {
            var time = (float)initData;
            NoDamageTimer = new GameTimer(time);
            var combatEntity = GetParent<AbilityEffect>().ParentEntity;
            combatEntity.ListenActionPoint(ActionPointType.PostReceiveDamage, WhenReceiveDamage);
        }

        public override void OnDestroy()
        {
            var combatEntity = GetParent<AbilityEffect>().ParentEntity;
            combatEntity.UnListenActionPoint(ActionPointType.PostReceiveDamage, WhenReceiveDamage);
        }

        public void StartListen(Action whenNoDamageInTimeCallback)
        {
            //WhenNoDamageInTimeCallback = whenNoDamageInTimeCallback;
            NoDamageTimer.OnFinish(OnFinish);
            AddComponent<UpdateComponent>();
        }

        private void OnFinish()
        {
            OnTrigger(this);
        }

        public void OnTrigger(Entity source)
        {
            //WhenNoDamageInTimeCallback?.Invoke();
            var abilityEffect = GetParent<AbilityEffect>();
            abilityEffect.OnObserverTrigger(new TriggerContext(this, source));
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