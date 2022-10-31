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
            }
            if (CollisionExecuteData.ActionData.ActionEventType == FireEventType.TriggerNewExecution)
            {
                
            }
        }
    }
}