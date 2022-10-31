using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameUtils;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 执行体应用目标效果组件
    /// </summary>
    public class ExecutionAssignToTargetComponent : Component
    {
        public override bool DefaultEnable { get; set; } = false;
        public EffectApplyType EffectApplyType { get; set; }


        public override void Awake()
        {
            Entity.Subscribe<ExecuteEffectEvent>(OnTriggerExecuteEffect);
        }

        public void OnTriggerExecuteEffect(ExecuteEffectEvent evnt)
        {
#if !NOT_UNITY
            var ParentExecution = Entity.GetParent<SkillExecution>();
            Log.Debug($"ExecutionAssignToTargetComponent OnTriggerExecutionEffect {ParentExecution.InputTarget} {EffectApplyType}");
            if (ParentExecution.InputTarget != null)
            {
                if (EffectApplyType == EffectApplyType.AllEffects)
                {
                    ParentExecution.AbilityEntity.Get<AbilityEffectComponent>().TryAssignAllEffectsToTargetWithExecution(ParentExecution.InputTarget, ParentExecution);
                }
                else
                {
                    ParentExecution.AbilityEntity.Get<AbilityEffectComponent>().TryAssignEffectByIndex(ParentExecution.InputTarget, (int)EffectApplyType - 1);
                }
            }
#endif
        }
    }
}