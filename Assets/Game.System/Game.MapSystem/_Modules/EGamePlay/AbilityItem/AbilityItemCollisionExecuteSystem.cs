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
            if (component.CollisionExecuteData.ActionData.ActionEventType == FireEventType.AssignEffect)
            {
                entity.ExecuteTriggerType = component.CollisionExecuteData.ActionData.ExecuteTrigger;
                if (component.CollisionExecuteData.ActionData.FireType == FireType.StartTrigger)
                {
                    if (component.CollisionExecuteData.ActionData.EffectApplyTarget == EffectApplyTarget.Self)
                    {
                        AbilityItemSystem.OnTriggerEvent(entity, entity);
                    }
                }
            }
            if (component.CollisionExecuteData.ActionData.ActionEventType == FireEventType.TriggerNewExecution)
            {

            }
        }

        public void Destroy(AbilityItem entity, AbilityItemCollisionExecuteComponent component)
        {
            var clipData = entity.GetComponent<AbilityItemCollisionExecuteComponent>().ExecuteClipData;
            var actionEvenData = entity.GetComponent<AbilityItemCollisionExecuteComponent>().CollisionExecuteData.ActionData;
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

        public static AbilityEffect[] GetAssignEffects(AbilityItem entity, AbilityItemCollisionExecuteComponent component)
        {
            var triggerType = component.CollisionExecuteData.ActionData.ExecuteTrigger;
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