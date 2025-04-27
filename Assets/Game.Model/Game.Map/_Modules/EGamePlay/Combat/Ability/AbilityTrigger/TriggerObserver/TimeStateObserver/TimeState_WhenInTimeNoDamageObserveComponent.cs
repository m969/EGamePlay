//using System;
//using ECS;
//using GameUtils;
//using UnityEngine;

//namespace EGamePlay.Combat
//{
//    public sealed class TimeState_WhenInTimeNoDamageObserveComponent : EcsComponent
//    {
//        //public override bool DefaultEnable => false;
//        public float Time {  get; set; }
//        private GameTimer NoDamageTimer { get; set; }
//        private Action WhenNoDamageInTimeCallback { get; set; }


//        public override void Awake(object initData)
//        {
//            NoDamageTimer = new GameTimer(Time);
//            var combatEntity = GetEntity<AbilityTrigger>().ParentEntity;
//            combatEntity.GetComponent<BehaviourPointComponent>().AddListener(ActionPointType.PostSufferDamage, WhenReceiveDamage);
//        }

//        public override void OnDestroy()
//        {
//            var combatEntity = GetEntity<AbilityTrigger>().ParentEntity;
//            combatEntity.GetComponent<BehaviourPointComponent>().RemoveListener(ActionPointType.PostSufferDamage, WhenReceiveDamage);
//        }

//        public override void Update()
//        {
//            if (NoDamageTimer.IsRunning)
//            {
//                NoDamageTimer.UpdateAsFinish(Time.deltaTime);
//            }
//        }

//        public void StartListen(Action whenNoDamageInTimeCallback)
//        {
//            NoDamageTimer.OnFinish(OnFinish);
//            Enable = true;
//            //AddComponent<UpdateComponent>();
//        }

//        private void OnFinish()
//        {
//            GetEntity<AbilityTrigger>().OnTrigger(new TriggerContext());
//            //OnTrigger(GetEntity<AbilityTrigger>());
//        }

//        private void WhenReceiveDamage(Entity combatAction)
//        {
//            NoDamageTimer.Reset(); 
//        }
//    }
//}