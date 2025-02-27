using System;
using GameUtils;
using UnityEngine;

namespace EGamePlay.Combat
{
    public sealed class TimeState_WhenInTimeNoDamageObserveComponent : Component
    {
        public override bool DefaultEnable => false;
        private GameTimer NoDamageTimer { get; set; }
        private Action WhenNoDamageInTimeCallback { get; set; }


        public override void Awake(object initData)
        {
            var time = (float)initData;
            NoDamageTimer = new GameTimer(time);
            var combatEntity = GetEntity<AbilityTrigger>().ParentEntity;
            combatEntity.GetComponent<BehaviourPointComponent>().AddListener(ActionPointType.PostSufferDamage, WhenReceiveDamage);
        }

        public override void OnDestroy()
        {
            var combatEntity = GetEntity<AbilityTrigger>().ParentEntity;
            combatEntity.GetComponent<BehaviourPointComponent>().RemoveListener(ActionPointType.PostSufferDamage, WhenReceiveDamage);
        }

        public void StartListen(Action whenNoDamageInTimeCallback)
        {
            NoDamageTimer.OnFinish(OnFinish);
            Enable = true;
            //AddComponent<UpdateComponent>();
        }

        private void OnFinish()
        {
            GetEntity<AbilityTrigger>().OnTrigger(new TriggerContext());
            //OnTrigger(GetEntity<AbilityTrigger>());
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