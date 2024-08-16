using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameUtils;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class AbilityItemCollisionExecuteComponent : Component
    {
        public ExecuteClipData ExecuteClipData { get; private set; }
        public ItemExecute CollisionExecuteData => ExecuteClipData.ItemData;


        public override void Awake(object initData)
        {
            ExecuteClipData = initData as ExecuteClipData;
            if (CollisionExecuteData.ActionData.ActionEventType == FireEventType.AssignEffect)
            {
                GetEntity<AbilityItem>().ExecuteTriggerType = CollisionExecuteData.ActionData.ExecuteTrigger;
                if (CollisionExecuteData.ActionData.FireType == FireType.StartTrigger)
                {
                    if (CollisionExecuteData.ActionData.EffectApplyTarget == EffectApplyTarget.Self)
                    {
                        GetEntity<AbilityItem>().OnTriggerEvent(GetEntity<AbilityItem>());
                    }
                }
            }
            if (CollisionExecuteData.ActionData.ActionEventType == FireEventType.TriggerNewExecution)
            {

            }
        }

        public T GetItemEffect<T>() where T : ItemEffect
        {
            T effectData = null;
            foreach (var item in ExecuteClipData.EffectDatas)
            {
                if (item is T itemEffect)
                {
                    effectData = itemEffect;
                    break;
                }
            }
            return effectData;
        }

        public AbilityEffect[] GetAssignEffects()
        {
            var triggerType = CollisionExecuteData.ActionData.ExecuteTrigger;
            var effects = GetEntity<AbilityItem>().AbilityEntity.GetComponent<AbilityEffectComponent>().AbilityEffects;
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