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
        public CollisionExecuteData CollisionExecuteData => ExecuteClipData.CollisionExecuteData;


        public override void Awake(object initData)
        {
            ExecuteClipData = initData as ExecuteClipData;
            if (CollisionExecuteData.ActionData.ActionEventType == FireEventType.AssignEffect)
            {
                GetEntity<AbilityItem>().EffectApplyType = CollisionExecuteData.ActionData.EffectApply;
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

        public AbilityEffect[] GetAssignEffects()
        {
            var EffectApplyType = CollisionExecuteData.ActionData.EffectApply;
            var effects = GetEntity<AbilityItem>().AbilityEntity.GetComponent<AbilityEffectComponent>().AbilityEffects;
            var list = new List<AbilityEffect>();
            for (int i = 0; i < effects.Count; i++)
            {
                if (i == (int)EffectApplyType - 1 || EffectApplyType == EffectApplyType.AllEffects)
                {
                    var effect = effects[i];
                    list.Add(effect);
                }
            }
            return list.ToArray();
        }
    }
}