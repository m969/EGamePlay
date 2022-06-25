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
    public class ExecutionApplyToTargetComponent : Component
    {
        public override bool DefaultEnable { get; set; } = false;
        public float TriggerTime { get; set; }
        public string TimeValueExpression { get; set; }
        public EffectApplyType EffectApplyType { get; set; }


        public override void Setup()
        {
            Entity.Subscribe<ExecutionEffectEvent>(OnTriggerExecutionEffect);
        }

        public void OnTriggerExecutionEffect(ExecutionEffectEvent evnt)
        {
            //Log.Debug($"ExecutionApplyToTargetComponent OnTriggerExecutionEffect");
#if !NOT_UNITY
            var ParentExecution = evnt.ExecutionEffect.GetParent<SkillExecution>();
            if (ParentExecution is SkillExecution skillExecution)
            {
                if (skillExecution.InputTarget != null)
                {
                    if (EffectApplyType == EffectApplyType.AllEffects)
                    {
                        GetEntity<ExecutionEffect>().ParentExecution.AbilityEntity.Get<AbilityEffectComponent>().TryAssignAllEffectsToTargetWithExecution(skillExecution.InputTarget, skillExecution);
                    }
                    else
                    {
                        GetEntity<ExecutionEffect>().ParentExecution.AbilityEntity.Get<AbilityEffectComponent>().TryAssignEffectByIndex(skillExecution.InputTarget, (int)EffectApplyType - 1);
                    }
                }
            }
#endif
        }
    }
}