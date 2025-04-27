using System;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using Sirenix.OdinInspector;

namespace EGamePlay.Combat
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class EffectDecorateAttribute : Attribute
    {
        private string label;
        public string Label { get { return label; } }
        private int order;
        public int Order { get { return order; } }
        public EffectDecorateAttribute(string label, int order)
        {
            this.label = label.Trim();
            this.order = order;
        }
    }

    /// <summary>
    /// 效果修饰
    /// </summary>
    [Serializable]
    public abstract class EffectDecorator
    {
        [HideInInspector]
        public virtual string Label => "Effect";

        [ToggleGroup("Enabled", "$Label")]
        public bool Enabled;
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable, EffectDecorate("按命中目标数递减百分比伤害", 10)]
    public class DamageReduceWithTargetCountDecorator : EffectDecorator
    {
        [HideInInspector]
        public override string Label => "按命中目标数递减百分比伤害";

        [ToggleGroup("Enabled"), LabelText("递减百分比")]
        public float ReducePercent;
        [ToggleGroup("Enabled"), LabelText("伤害下限百分比")]
        public float MinPercent;
    }

    [Serializable, EffectDecorate("当赋给效果后触发新的效果", 20)]
    public class TriggerNewEffectWhenAssignEffectDecorator : EffectDecorator
    {
        [HideInInspector]
        public override string Label => "当赋给效果后触发新的效果";

        [ToggleGroup("Enabled")]
        [LabelText("触发效果")]
        public ExecuteTriggerType ExecuteTriggerType;
    }
}