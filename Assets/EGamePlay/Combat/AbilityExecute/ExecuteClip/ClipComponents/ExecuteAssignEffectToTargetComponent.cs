using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameUtils;
using static UnityEngine.GraphicsBuffer;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 执行体应用目标效果组件
    /// </summary>
    public class ExecuteAssignEffectToTargetComponent : Component
    {
        public override bool DefaultEnable { get; set; } = false;
        public ExecuteTriggerType ExecuteTriggerType { get; set; }


        public override void Awake()
        {
            Entity.Subscribe<ExecuteEffectEvent>(OnTriggerExecuteEffect);
        }

        public void OnTriggerExecuteEffect(ExecuteEffectEvent evnt)
        {
            var skillExecution = Entity.GetParent<AbilityExecution>();
            //Log.Debug($"ExecutionAssignToTargetComponent OnTriggerExecutionEffect {skillExecution.InputTarget} {EffectApplyType}");
            if (skillExecution.InputTarget != null)
            {
                var abilityTriggerComp = skillExecution.AbilityEntity.GetComponent<AbilityTriggerComponent>();
                var OwnerEntity = skillExecution.OwnerEntity;
                var effects = abilityTriggerComp.AbilityTriggers;
                for (int i = 0; i < effects.Count; i++)
                {
                    if (i == (int)ExecuteTriggerType - 1 || ExecuteTriggerType == ExecuteTriggerType.AllTriggers)
                    {
                        var effect = effects[i];
                        var context = new TriggerContext()
                        {
                            AbilityTrigger = effect,
                            TriggerSource = skillExecution,
                            Target = skillExecution.InputTarget,
                        };
                        effect.OnTrigger(context);
                    }
                }
            }
        }
    }
}