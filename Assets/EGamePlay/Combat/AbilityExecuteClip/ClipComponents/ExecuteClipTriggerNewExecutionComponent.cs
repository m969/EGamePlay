using ET;
using GameUtils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class ExecuteClipTriggerNewExecutionComponent : Component
    {
        public ActionEventData ActionEventData { get; set; }


        public override void Awake()
        {
            Entity.Subscribe<ExecuteEffectEvent>(OnTriggerExecutionEffect);
        }

        public void OnTriggerExecutionEffect(ExecuteEffectEvent evnt)
        {
            var executionObject = AssetUtils.Load<ExecutionObject>(ActionEventData.NewExecution);
            if (executionObject == null)
            {
                return;
            }
            var sourceExecution = Entity.GetParent<SkillExecution>();
            var execution = sourceExecution.OwnerEntity.AddChild<SkillExecution>(sourceExecution.SkillAbility);
            execution.ExecutionObject = executionObject;
            execution.InputTarget = sourceExecution.InputTarget;
            execution.InputPoint = sourceExecution.InputPoint;
            execution.LoadExecutionEffects();
            execution.BeginExecute();
            if (executionObject != null)
            {
                execution.AddComponent<UpdateComponent>();
            }
        }
    }
}