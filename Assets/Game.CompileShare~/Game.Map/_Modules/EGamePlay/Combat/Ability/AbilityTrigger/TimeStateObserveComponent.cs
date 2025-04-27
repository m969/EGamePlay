using ECS;
using GameUtils;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace EGamePlay.Combat
{
    [LabelText("计时状态事件")]
    public enum TimeStateEventType
    {
        [LabelText("自定义计时状态事件")]
        CustomCondition = 0,
        [LabelText("当x秒内没有受伤")]
        WhenInTimeNoDamage = 3,
        [LabelText("当间隔x秒")]
        WhenIntervalTime = 4,
    }

    public sealed class TimeStateObserveComponent : EcsComponent
    {
        //public float Interval {  get; set; }
        public GameTimer Timer { get; set; }
        public Action TimeStateAction { get; set; }
        public TimeStateEventType TimeStateEventType { get; set; }


        //public override void Awake(object initData)
        //{
        //    //Log.Debug($"TimeState_TimeIntervalObserveComponent Awake {initData}");
        //    //Interval = (float)initData;
        //    //Timer = new GameTimer(Interval);
        //}

        //public void StartListen(Action whenTimeIntervalAction)
        //{
        //    Timer.OnRepeat(OnRepeat);
        //    Enable = true;
        //    //AddComponent<UpdateComponent>();
        //}

        //private void OnRepeat()
        //{
        //    GetEntity<AbilityTrigger>().OnTrigger(new TriggerContext());
        //    //OnTrigger(GetEntity<AbilityTrigger>());
        //}

        //public override void Update()
        //{
        //    if (Timer.IsRunning)
        //    {
        //        Timer.UpdateAsRepeat(Time.deltaTime);
        //    }
        //}
    }
}