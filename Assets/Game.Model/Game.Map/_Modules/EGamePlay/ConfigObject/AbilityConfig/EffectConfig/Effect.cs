using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Reflection;

#if EGAMEPLAY_ET
using Unity.Mathematics;
using Vector3 = Unity.Mathematics.float3;
using Quaternion = Unity.Mathematics.quaternion;
using JsonIgnore = MongoDB.Bson.Serialization.Attributes.BsonIgnoreAttribute;
#endif


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
#if UNITY
    public abstract class Effect
#else
    public class Effect : System.Object
#endif
    {
        [HideInInspector]
        public bool IsSkillEffect;

        public bool HideTriggerType
        {
            get
            {
                return false;
            }
        }

        [HideInInspector]
        public virtual string Label => "Effect";

        [ToggleGroup("Enabled", "$Label")]
        public bool Enabled;

        [ShowIf("@this.Decorators != null && this.Decorators.Count > 0")]
        [ToggleGroup("Enabled"), LabelText("效果修饰"), PropertyOrder(100)]
        [HideReferenceObjectPicker, ListDrawerSettings(DraggableItems = false)/*, TypeFilter("GetFilteredTypeList")*/]
        /// Effect是直接效果，效果修饰是基于直接效果的辅助效果
        public List<EffectDecorator> Decorators = new List<EffectDecorator>();

        [ToggleGroup("Enabled")]
        [HorizontalGroup("Enabled/Hor2", PaddingLeft = 20, PaddingRight = 20)]
        [HideLabel, OnValueChanged("AddEffect"), ValueDropdown("EffectTypeSelect"), PropertyOrder(101), JsonIgnore]
        public string EffectTypeName = EffectTypeNameStr;

        public const string EffectTypeNameStr = "(添加修饰)";

        public IEnumerable<string> EffectTypeSelect()
        {
            var types = typeof(EffectDecorator).Assembly.GetTypes()
                .Where(x => !x.IsAbstract)
                .Where(x => typeof(EffectDecorator).IsAssignableFrom(x))
                .Where(x => x.GetCustomAttribute<EffectDecorateAttribute>() != null)
                .OrderBy(x => x.GetCustomAttribute<EffectDecorateAttribute>().Order)
                .Select(x => x.GetCustomAttribute<EffectDecorateAttribute>().Label);
            var results = types.ToList();
            results.Insert(0, EffectTypeNameStr);
            return results;
        }

        private void AddEffect()
        {
            if (EffectTypeName != EffectTypeNameStr)
            {
                var effectType = typeof(EffectDecorator).Assembly.GetTypes()
                    .Where(x => !x.IsAbstract)
                    .Where(x => typeof(EffectDecorator).IsAssignableFrom(x))
                    .Where(x => x.GetCustomAttribute<EffectDecorateAttribute>() != null)
                    .Where(x => x.GetCustomAttribute<EffectDecorateAttribute>().Label == EffectTypeName)
                    .FirstOrDefault();

                if (effectType != null)
                {
                    var effect = Activator.CreateInstance(effectType) as EffectDecorator;
                    effect.Enabled = true;
                    if (Decorators == null) Decorators = new List<EffectDecorator>();
                    Decorators.Add(effect);
                }

                EffectTypeName = EffectTypeNameStr;
            }
        }

#if UNITY_EDITOR
        private void DrawSpace()
        {
            GUILayout.Space(20);
        }

        private bool TriggerFoldout;
        private void BeginBox()
        {
        }

        private void EndBox()
        {
        }
#endif
    }
}
