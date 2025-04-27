//using ECS;
//using GameUtils;
//using System;
//using UnityEngine;

//namespace EGamePlay.Combat
//{
//    public sealed class TimeState_TimeIntervalObserveComponent : EcsComponent
//    {
//        //public override bool DefaultEnable => false;
//        public float Interval {  get; set; }
//        private GameTimer IntervalTimer { get; set; }
//        private Action WhenTimeIntervalAction { get; set; }


//        public override void Awake(object initData)
//        {
//            //Log.Debug($"TimeState_TimeIntervalObserveComponent Awake {initData}");
//            //Interval = (float)initData;
//            IntervalTimer = new GameTimer(Interval);
//        }

//        public void StartListen(Action whenTimeIntervalAction)
//        {
//            IntervalTimer.OnRepeat(OnRepeat);
//            Enable = true;
//            //AddComponent<UpdateComponent>();
//        }

//        private void OnRepeat()
//        {
//            GetEntity<AbilityTrigger>().OnTrigger(new TriggerContext());
//            //OnTrigger(GetEntity<AbilityTrigger>());
//        }

//        public override void Update()
//        {
//            if (IntervalTimer.IsRunning)
//            {
//                IntervalTimer.UpdateAsRepeat(Time.deltaTime);
//            }
//        }
//    }
//}