using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameUtils;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 执行体应用目标效果能力效果组件
    /// </summary>
    public class EffectExecutionApplyToTargetComponent : Component
    {
        public override bool DefaultEnable { get; set; } = false;
        public float TriggerTime { get; set; }
        public string TimeValueExpression { get; set; }
        public EffectApplyType EffectApplyType { get; set; }
        public List<AbilityEffect> ApplyAbilityEffects { get; set; } = new List<AbilityEffect>();
    }
}