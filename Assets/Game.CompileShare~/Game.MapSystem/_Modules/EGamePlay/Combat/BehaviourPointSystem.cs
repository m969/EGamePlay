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

        public static void AddListener(EcsEntity entity, ActionPointType actionPointType, Action<EcsEntity> action)
        {
            var component = entity.GetComponent<BehaviourPointComponent>();
            if (!component.ActionPoints.ContainsKey(actionPointType))
            {
                component.ActionPoints.Add(actionPointType, new BehaviourPoint());
            }
            AddListener(component.ActionPoints[actionPointType], action);
        }

        public static void RemoveListener(EcsEntity entity, ActionPointType actionPointType, Action<EcsEntity> action)
        {
            var component = entity.GetComponent<BehaviourPointComponent>();
            if (component.ActionPoints.ContainsKey(actionPointType))
            {
                RemoveListener(component.ActionPoints[actionPointType], action);
            }
        }

        public static void AddListener(EcsEntity entity, ApplyPointType applyPointType, Action<EcsEntity> action)
        {
            var component = entity.GetComponent<BehaviourPointComponent>();
            if (!component.ApplyPoints.ContainsKey(applyPointType))
            {
                component.ApplyPoints.Add(applyPointType, new BehaviourPoint());
            }
            AddListener(component.ApplyPoints[applyPointType], action);
        }

        public static void RemoveListener(EcsEntity entity, ApplyPointType applyPointType, Action<EcsEntity> action)
        {
            var component = entity.GetComponent<BehaviourPointComponent>();
            if (component.ApplyPoints.ContainsKey(applyPointType))
            {
                RemoveListener(component.ApplyPoints[applyPointType], action);
            }
        }

        public static void AddObserver(EcsEntity entity, ActionPointType actionPointType, EcsEntity observer)
        {
            var component = entity.GetComponent<BehaviourPointComponent>();
            if (!component.ActionPoints.ContainsKey(actionPointType))
            {
                component.ActionPoints.Add(actionPointType, new BehaviourPoint());
            }
            AddObserver(component.ActionPoints[actionPointType], observer);
        }

        public static void RemoveObserver(EcsEntity entity, ActionPointType actionPointType, EcsEntity observer)
        {
            var component = entity.GetComponent<BehaviourPointComponent>();
            if (component.ActionPoints.ContainsKey(actionPointType))
            {
                RemoveObserver(component.ActionPoints[actionPointType], observer);
            }
        }

        public static BehaviourPoint GetActionPoint(EcsEntity entity, ActionPointType actionPointType)
        {
            var component = entity.GetComponent<BehaviourPointComponent>();
            if (component.ActionPoints.TryGetValue(actionPointType, out var actionPoint)) ;
            return actionPoint;
        }

        public static void TriggerActionPoint(EcsEntity entity, ActionPointType actionPointType, EcsEntity actionExecution)
        {
            if (entity.IsDisposed) return;
            var component = entity.GetComponent<BehaviourPointComponent>();
            foreach (var item in component.ActionPoints)
            {
                if (item.Key.HasFlag(actionPointType))
                {
                    TriggerAllObservers(item.Value, actionExecution);
                }
            }
        }

        public static void TriggerApplyPoint(EcsEntity entity, ApplyPointType actionPointType, EcsEntity actionExecution)
        {
            if (entity.IsDisposed) return;
            var component = entity.GetComponent<BehaviourPointComponent>();
            foreach (var item in component.ApplyPoints)
            {
                if (item.Key.HasFlag(actionPointType))
                {
                    TriggerAllObservers(item.Value, actionExecution);
                }
            }
        }

        /// <summary>
        /// 触发Creator施效点
        /// </summary>
        public static void TriggerCreatorApplyPoint(IActionExecute actionExecute, ApplyPointType pointType)
        {
            if (actionExecute.Creator.IsDisposed) return;
            TriggerApplyPoint(actionExecute.Creator, pointType, (EcsEntity)actionExecute);
        }

        /// <summary>
        /// 触发Target施效点
        /// </summary>
        public static void TriggerTargetApplyPoint(IActionExecute actionExecute, ApplyPointType pointType)
        {
            if (actionExecute.Target.IsDisposed) return;
            TriggerApplyPoint(actionExecute.Target, pointType, (EcsEntity)actionExecute);
        }

        /// <summary>
        /// 触发Creator行动点
        /// </summary>
        public static void TriggerCreatorActionPoint(IActionExecute actionExecute, ActionPointType pointType)
        {
            if (actionExecute.Creator.IsDisposed) return;
            TriggerActionPoint(actionExecute.Creator, pointType, (EcsEntity)actionExecute);
        }

        /// <summary>
        /// 触发Target行动点
        /// </summary>
        public static void TriggerTargetActionPoint(IActionExecute actionExecute, ActionPointType pointType)
        {
            if (actionExecute.Target.IsDisposed) return;
            TriggerActionPoint(actionExecute.Target, pointType, (EcsEntity)actionExecute);
        }

        public static void AddListener(BehaviourPoint behaviourPoint, Action<EcsEntity> action)
        {
            behaviourPoint.Listeners.Add(action);
        }

        public static void RemoveListener(BehaviourPoint behaviourPoint, Action<EcsEntity> action)
        {
            behaviourPoint.Listeners.Remove(action);
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