using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System;
using ET;

namespace EGamePlay
{
    public class TimeStateObserveSystem : AComponentSystem<AbilityTrigger, TimeStateObserveComponent>,
        IAwake<AbilityTrigger, TimeStateObserveComponent>,
        IDestroy<AbilityTrigger, TimeStateObserveComponent>
    {
        public void Awake(AbilityTrigger entity, TimeStateObserveComponent component)
        {
        }

        public void Destroy(AbilityTrigger entity, TimeStateObserveComponent component)
        {
        }

        public static void Update(AbilityTrigger entity, TimeStateObserveComponent component)
        {
            var timeStateEvent = component.TimeStateEventType;
            var timer = component.Timer;
            if (!timer.IsRunning)
            {
                return;
            }

            if (timeStateEvent == TimeStateEventType.WhenIntervalTime)
            {
                timer.UpdateAsRepeat(Time.deltaTime);
            }
            if (timeStateEvent == TimeStateEventType.WhenInTimeNoDamage)
            {
                timer.UpdateAsFinish(Time.deltaTime);
            }
        }

        public static void StartListen(AbilityTrigger entity, TimeStateEventType timeStateEvent)
        {
            var component = entity.GetComponent<TimeStateObserveComponent>();
            component.TimeStateEventType = timeStateEvent;
            if (timeStateEvent == TimeStateEventType.WhenIntervalTime)
            {
                component.Timer.OnRepeat(() => OnTimeState(entity));
            }
            if (timeStateEvent == TimeStateEventType.WhenInTimeNoDamage)
            {
                component.Timer.OnFinish(() => OnTimeState(entity));
            }

            component.Enable = true;
        }

        public static void OnTimeState(AbilityTrigger entity)
        {
            AbilityTriggerSystem.OnTrigger(entity, new TriggerContext());
        }

        public static void WhenReceiveDamage(AbilityTrigger entity, EcsEntity combatAction)
        {
            var component = entity.GetComponent<TimeStateObserveComponent>();
            component.Timer.Reset();
        }
    }
}