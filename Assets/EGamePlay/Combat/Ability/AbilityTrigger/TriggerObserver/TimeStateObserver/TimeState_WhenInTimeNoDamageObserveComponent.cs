using System;
using GameUtils;
using UnityEngine;

namespace EGamePlay.Combat
{
    public sealed class TimeState_WhenInTimeNoDamageObserveComponent : Component, ICombatObserver
    {
        public override bool DefaultEnable => false;
        private GameTimer NoDamageTimer { get; set; }
        private Action WhenNoDamageInTimeCallback { get; set; }


        public override void Awake(object initData)
        {
            var time = (float)initData;
            NoDamageTimer = new GameTimer(time);
            var combatEntity = GetEntity<AbilityTrigger>().ParentEntity;
            combatEntity.GetComponent<ActionPointComponent>().AddListener(ActionPointType.PostReceiveDamage, WhenReceiveDamage);
        }

        public override void OnDestroy()
        {
            var combatEntity = GetEntity<AbilityTrigger>().ParentEntity;
            combatEntity.GetComponent<ActionPointComponent>().RemoveListener(ActionPointType.PostReceiveDamage, WhenReceiveDamage);
        }

        public void StartListen(Action whenNoDamageInTimeCallback)
        {
            NoDamageTimer.OnFinish(OnFinish);
            Enable = true;
            //AddComponent<UpdateComponent>();
        }

        private void OnFinish()
        {
            OnTrigger(GetEntity<AbilityTrigger>());
        }

        public void OnTrigger(Entity source)
        {
            GetEntity<AbilityTrigger>().OnTrigger(new TriggerContext(this, source));
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
            NoDamageTimer.Reset();
        }
    }
}