using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace EGamePlay.Combat
{
    public class ApplyToTargetEffect : Effect
    {
        public override string Label => "应用效果给目标";

        public float TriggerTime { get; set; }
        public string TimeValueExpression { get; set; }
        public EffectApplyType EffectApplyType { get; set; }
    }
}