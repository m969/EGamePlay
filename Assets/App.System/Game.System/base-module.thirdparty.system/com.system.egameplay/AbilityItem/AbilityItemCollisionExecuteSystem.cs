using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;

namespace EGamePlay
{
    public class AbilityItemCollisionExecuteSystem : AComponentSystem<AbilityItem, AbilityItemCollisionExecuteComponent>,
        IAwake<AbilityItem, AbilityItemCollisionExecuteComponent>,
        IDestroy<AbilityItem, AbilityItemCollisionExecuteComponent>
    {
        public void Awake(AbilityItem entity, AbilityItemCollisionExecuteComponent component)
        {
            //ExecuteClipData = initData as ExecuteClipData;
            var actionData = GetActionEventData(entity);
            if (actionData.ActionEventType == FireEventType.AssignEffect)
            {
                entity.ExecuteTriggerType = actionData.ExecuteTrigger;
                if (actionData.FireType == FireType.StartTrigger)
                {
                    if (actionData.EffectApplyTarget == EffectApplyTarget.Self)
                    {
                        AbilityItemSystem.OnTriggerEvent(entity, entity);
                    }
                }
            }
            if (actionData.ActionEventType == FireEventType.TriggerNewExecution)
            {

            }
        }

        public void Destroy(AbilityItem entity, AbilityItemCollisionExecuteComponent component)
        {
            var clipData = entity.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            var actionEvenData = GetActionEventData(entity);
            if (clipData.ExecuteClipType == ExecuteClipType.ItemExecute && actionEvenData.FireType == FireType.EndTrigger)
            {
                AbilityItemSystem.OnTriggerEvent(entity, null);
            }

            if (clipData.ItemData.ExecuteType == CollisionExecuteType.InHand)
            {
                AbilityExecutionSystem.EndExecute(entity.AbilityExecution);
            }
        }

        public static T GetItemEffect<T>(AbilityItem entity) where T : ItemEffect
        {
            var component = entity.GetComponent<AbilityItemCollisionExecuteComponent>();
            T effectData = null;
            foreach (var item in component.ExecuteClipData.EffectDatas)
            {
                if (item is T itemEffect)
                {
                    effectData = itemEffect;
                    break;
                }
            }
            return effectData;
        }

        /// <summary>
        /// 获取触发事件数据：优先使用 Inspector 中可见的 ActionEventEffect（触发事件），
        /// 若未启用则回退到隐藏的 ItemData.ActionData（旧配置），以兼容旧资源。
        /// </summary>
        public static ActionEventData GetActionEventData(AbilityItem entity)
        {
            var component = entity.GetComponent<AbilityItemCollisionExecuteComponent>();
            var actionEvent = GetItemEffect<ActionEventEffect>(entity);
            if (actionEvent != null && actionEvent.Enabled)
            {
                return new ActionEventData
                {
                    FireType = actionEvent.FireType,
                    ActionEventType = actionEvent.ActionEventType,
                    ExecuteTrigger = actionEvent.ExecuteTrigger,
                    EffectApplyTarget = actionEvent.EffectApplyTarget,
                    NewExecution = actionEvent.NewExecution,
                };
            }
            return component.CollisionExecuteData.ActionData;
        }

        public static AbilityEffect[] GetAssignEffects(AbilityItem entity, AbilityItemCollisionExecuteComponent component)
        {
            var triggerType = GetActionEventData(entity).ExecuteTrigger;
            var effects = entity.AbilityEntity.AbilityEffects;
            var list = new List<AbilityEffect>();
            for (int i = 0; i < effects.Count; i++)
            {
                if (i == (int)triggerType - 1 || triggerType == ExecuteTriggerType.AllTriggers)
                {
                    var effect = effects[i];
                    list.Add(effect);
                }
            }
            return list.ToArray();
        }
    }
}