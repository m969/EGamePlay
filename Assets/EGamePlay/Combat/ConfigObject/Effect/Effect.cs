using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace EGamePlay.Combat
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class EffectAttribute : Attribute
    {
        readonly string effectType;
        readonly int order;

        public EffectAttribute(string effectType, int order)
        {
            this.effectType = effectType;
            this.order = order;
        }

        public string EffectType
        {
            get { return effectType; }
        }

        public int Order
        {
            get { return order; }
        }
    }

    [Serializable]
    public abstract class Effect
    {
        [HideInInspector]
        public bool IsSkillEffect;

        [HideInInspector]
        public virtual string Label => "Effect";

        [ToggleGroup("Enabled", "$Label")]
        public bool Enabled;

        [ToggleGroup("Enabled"), ShowIf("IsSkillEffect", true)]
        public AddSkillEffetTargetType AddSkillEffectTargetType;

        [HorizontalGroup("Enabled/Hor")]
        [ToggleGroup("Enabled"), HideIf("IsSkillEffect", true), HideLabel]
        public EffectTriggerType EffectTriggerType;

        [HorizontalGroup("Enabled/Hor")]
        [ToggleGroup("Enabled"), HideIf("IsSkillEffect", true), ShowIf("EffectTriggerType", EffectTriggerType.Condition), HideLabel]
        public ConditionType ConditionType;

        [HorizontalGroup("Enabled/Hor")]
        [ToggleGroup("Enabled"), HideIf("IsSkillEffect", true), ShowIf("EffectTriggerType", EffectTriggerType.Action), HideLabel]
        public ActionPointType ActionPointType;

        [HorizontalGroup("Enabled/Hor")]
        [ToggleGroup("Enabled"), HideIf("IsSkillEffect", true), ShowIf("EffectTriggerType", EffectTriggerType.Interval), SuffixLabel("毫秒", true), HideLabel]
        public string Interval;

        [ToggleGroup("Enabled"), HideIf("IsSkillEffect", true), LabelText("条件参数 x="), ShowIf("EffectTriggerType", EffectTriggerType.Condition)]
        public string ConditionParam;

        [ToggleGroup("Enabled"), /*HideIf("IsSkillEffect", true), */LabelText("触发概率")]
        public string TriggerProbability = "100%";


        public string IntervalValue { get; set; }
        public string ConditionParamValue { get; set; }
    }
}