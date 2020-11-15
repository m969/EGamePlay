using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.IO;
using Sirenix.Utilities.Editor;
using System.Linq;
using UnityEditor;
using System.Reflection;

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

    public abstract class Effect
    {
        [HideInInspector]
        public bool IsSkillEffect;

        [HideInInspector]
        public virtual string Label => "Effect";

        [ToggleGroup("Enabled", "$Label")]
        public bool Enabled;

        //[FoldoutGroup("Enabled/触发机制")]
        [ToggleGroup("Enabled"), ShowIf("IsSkillEffect", true)]
        public AddSkillEffetTargetType AddSkillEffectTargetType;

        [HorizontalGroup("Enabled/Hor")]
        //[FoldoutGroup("Enabled/触发机制")]
        [ToggleGroup("Enabled"), HideIf("IsSkillEffect", true), HideLabel]
        public EffectTriggerType EffectTriggerType;

        [HorizontalGroup("Enabled/Hor")]
        //[FoldoutGroup("Enabled/触发机制")]
        [ToggleGroup("Enabled"), HideIf("IsSkillEffect", true), ShowIf("EffectTriggerType", EffectTriggerType.Condition), HideLabel]
        public ConditionType ConditionType;

        [HorizontalGroup("Enabled/Hor")]
        //[FoldoutGroup("Enabled/触发机制")]
        [ToggleGroup("Enabled"), HideIf("IsSkillEffect", true), ShowIf("EffectTriggerType", EffectTriggerType.Action), HideLabel]
        public ActionPointType ActionPointType;

        [HorizontalGroup("Enabled/Hor")]
        //[FoldoutGroup("Enabled/触发机制")]
        [ToggleGroup("Enabled"), HideIf("IsSkillEffect", true), ShowIf("EffectTriggerType", EffectTriggerType.Interval), SuffixLabel("毫秒", true), HideLabel]
        public uint Interval;

        //[FoldoutGroup("Enabled/触发机制")]
        [ToggleGroup("Enabled"), HideIf("IsSkillEffect", true), LabelText("条件参数 x="), ShowIf("EffectTriggerType", EffectTriggerType.Condition)]
        public string ConditionParam;
    }

    public abstract class OnceEffect : Effect
    {

    }

    public abstract class RepeatedEffect : Effect
    {

    }

    public abstract class TriggerEffect : Effect
    {

    }
}