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
                    var executionEffects = GetEntity<ExecutionEffect>().ParentExecution.ExecutionEffects;
                    if (EffectApplyType == EffectApplyType.Effects)
                    {
                        foreach (var executionEffect in executionEffects)
                        {
                            if (executionEffect.Name == "SpawnItem") continue;
                            if (executionEffect.Name == "Animation") continue;
                            if (executionEffect.Name == "ApplyToTarget") continue;
                            executionEffect.ApplyEffectTo(skillExecution.InputTarget);
                        }
                    }
                    ExecutionEffect eecutionEffect2 = null;
                    if (EffectApplyType == EffectApplyType.Effect1)
                    {
                        eecutionEffect2 = executionEffects[0];
                        eecutionEffect2.ApplyEffectTo(skillExecution.InputTarget);
                    }
                    if (EffectApplyType == EffectApplyType.Effect2)
                    {
                        eecutionEffect2 = executionEffects[1];
                        eecutionEffect2.ApplyEffectTo(skillExecution.InputTarget);
                    }
                    if (EffectApplyType == EffectApplyType.Effect3)
                    {
                        eecutionEffect2 = executionEffects[2];
                        eecutionEffect2.ApplyEffectTo(skillExecution.InputTarget);
                    }
                }
            }
#endif
        }
    }
}