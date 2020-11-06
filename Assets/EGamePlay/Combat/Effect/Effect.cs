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

        public EffectAttribute(string effectType)
        {
            this.effectType = effectType;
        }

        public string EffectType
        {
            get { return effectType; }
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

        [ToggleGroup("Enabled"), ShowIf("IsSkillEffect", true)]
        public AddSkillEffetTargetType AddSkillEffectTargetType;

        //[GUIColor(0, 1, 0)]
        [ToggleGroup("Enabled"), HideIf("IsSkillEffect", true)]
        public EffectTriggerType EffectTriggerType;

        [ToggleGroup("Enabled"), LabelText("间隔时间"), ShowIf("EffectTriggerType", EffectTriggerType.Interval), SuffixLabel("毫秒", true)]
        public uint Interval;
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