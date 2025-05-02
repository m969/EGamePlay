using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System;
using ET;
using System.ComponentModel;
using ECSGame;

namespace EGamePlay
{
    public class BehaviourPointSystem : AComponentSystem<CombatEntity, BehaviourPointComponent>,
        IAwake<CombatEntity, BehaviourPointComponent>
    {
        public void Awake(CombatEntity entity, BehaviourPointComponent component)
        {

        }

        public static void AddObserver(CombatEntity entity, ActionPointType actionPointType, EcsEntity observer)
        {
            var component = entity.GetComponent<BehaviourPointComponent>();
            if (!component.ActionPoints.ContainsKey(actionPointType))
            {
                component.ActionPoints.Add(actionPointType, new BehaviourPoint());
            }
            AddObserver(component.ActionPoints[actionPointType], observer);
        }

        public static void RemoveObserver(CombatEntity entity, ActionPointType actionPointType, EcsEntity observer)
        {
            var component = entity.GetComponent<BehaviourPointComponent>();
            if (component.ActionPoints.ContainsKey(actionPointType))
            {
                RemoveObserver(component.ActionPoints[actionPointType], observer);
            }
        }

        public static BehaviourPoint GetActionPoint(CombatEntity entity, ActionPointType actionPointType)
        {
            var component = entity.GetComponent<BehaviourPointComponent>();
            if (component.ActionPoints.TryGetValue(actionPointType, out var actionPoint)) ;
            return actionPoint;
        }

        public static void TriggerActionPoint(EcsEntity entity, ActionPointType actionPointType, EcsEntity actionExecution)
        {
            if (entity.IsDisposed) return;
            var component = entity.GetComponent<BehaviourPointComponent>();
            if (component == null) return;
            foreach (var item in component.ActionPoints)
            {
                if (item.Key.HasFlag(actionPointType))
                {
                    TriggerAllObservers(item.Value, actionExecution);
                }
            }
        }

        public static void AddObserver(BehaviourPoint behaviourPoint, EcsEntity observer)
        {
            behaviourPoint.Observers.Add(observer);
        }

        public static void RemoveObserver(BehaviourPoint behaviourPoint, EcsEntity observer)
        {
            behaviourPoint.Observers.Remove(observer);
        }

        public static void TriggerAllObservers(BehaviourPoint behaviourPoint, EcsEntity actionExecution)
        {
            if (behaviourPoint.Listeners.Count > 0)
            {
                for (int i = behaviourPoint.Listeners.Count - 1; i >= 0; i--)
                {
                    var item = behaviourPoint.Listeners[i];
                    item.Invoke(actionExecution);
                }
            }
            if (behaviourPoint.Observers.Count > 0)
            {
                for (int i = behaviourPoint.Observers.Count - 1; i >= 0; i--)
                {
                    var item = behaviourPoint.Observers[i];
                    BehaviourObserveSystem.OnTrigger(item as AbilityTrigger, actionExecution);
                }
            }
        }
    }
}