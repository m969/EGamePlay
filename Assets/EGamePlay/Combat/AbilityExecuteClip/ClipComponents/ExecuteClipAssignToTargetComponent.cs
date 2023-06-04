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
    public class ExecuteClipAssignToTargetComponent : Component
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
            Log.Debug($"ExecutionAssignToTargetComponent OnTriggerExecutionEffect {skillExecution.InputTarget} {EffectApplyType}");
            if (skillExecution.InputTarget != null)
            {
                var abilityEffectComponent = skillExecution.AbilityEntity.GetComponent<AbilityEffectComponent>();
                var OwnerEntity = skillExecution.OwnerEntity;
                if (EffectApplyType == EffectApplyType.AllEffects)
                {
                    var effectAssigns = abilityEffectComponent.CreateAssignActions(skillExecution.InputTarget);
                    foreach (var item in effectAssigns)
                    {
                        item.AssignEffect();
                    }
                }
                else
                {
                    var abilityEffect = abilityEffectComponent.GetEffect((int)EffectApplyType - 1);
                    var effectAssign = abilityEffect.CreateAssignAction(skillExecution.InputTarget);
                    effectAssign.AssignEffect();
                }
            }
        }
    }
}