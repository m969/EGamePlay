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
    public class ExecuteAssignEffectToTargetComponent : Component
    {
        public override bool DefaultEnable { get; set; } = false;
        public EffectApplyType EffectApplyType { get; set; }


        public override void Awake()
        {
            Entity.Subscribe<ExecuteEffectEvent>(OnTriggerExecuteEffect);
        }

        public void OnTriggerExecuteEffect(ExecuteEffectEvent evnt)
        {
            var skillExecution = Entity.GetParent<SkillExecution>();
            //Log.Debug($"ExecutionAssignToTargetComponent OnTriggerExecutionEffect {skillExecution.InputTarget} {EffectApplyType}");
            if (skillExecution.InputTarget != null)
            {
                var abilityEffectComponent = skillExecution.AbilityEntity.GetComponent<AbilityEffectComponent>();
                var OwnerEntity = skillExecution.OwnerEntity;
                var effects = abilityEffectComponent.AbilityEffects;
                for (int i = 0; i < effects.Count; i++)
                {
                    if (i == (int)EffectApplyType - 1 || EffectApplyType == EffectApplyType.AllEffects)
                    {
                        var effect = effects[i];
                        effect.TriggerObserver.OnTriggerWithSkillExecution(skillExecution, skillExecution.InputTarget);
                    }
                }
            }
        }
    }
}