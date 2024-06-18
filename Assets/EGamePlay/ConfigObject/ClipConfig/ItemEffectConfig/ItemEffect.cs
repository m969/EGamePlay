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
    sealed class ExecuteEffectAttribute : Attribute
    {
        readonly string effectType;
        readonly int order;

        public ExecuteEffectAttribute(string effectType, int order)
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
    public abstract class ItemEffect
#else
    public class ItemEffect : ET.Object
#endif
    {
        [HideInInspector]
        public virtual string Label => "Effect";

        [ToggleGroup("Enabled", "$Label")]
        public bool Enabled;

        //[OnInspectorGUI("BeginBox", append: false)]
        //[FoldoutGroup("Enabled/TriggerType", GroupName = "触发机制")]
        //[ToggleGroup("Enabled"), HideIf("HideTriggerType", true), LabelText("触发事件")]
        //public EffectTriggerType EffectTriggerType;

        //[FoldoutGroup("Enabled/TriggerType")]
        //[ToggleGroup("Enabled"), ShowIf("EffectTriggerType", EffectTriggerType.Action)]
        //public ActionPointType ActionPointType;

        //[FoldoutGroup("Enabled/TriggerType")]
        //[ToggleGroup("Enabled"), ShowIf("EffectTriggerType", EffectTriggerType.Condition)]
        //public TimeStateEventType ConditionType;

        //[FoldoutGroup("Enabled/TriggerType")]
        //[ToggleGroup("Enabled"), LabelText("x="), ShowIf("EffectTriggerType", EffectTriggerType.Condition)]
        //public string ConditionParam;

        //[FoldoutGroup("Enabled/TriggerType")]
        //[ToggleGroup("Enabled"), LabelText("状态判断"), HideInInspector]
        //public Dictionary<StateCheckType, string> StateChecks = new Dictionary<StateCheckType, string>();

        //[FoldoutGroup("Enabled/TriggerType")]
        //[ToggleGroup("Enabled"), LabelText("状态判断")]
        //public List<string> StateCheckList = new List<string>();

        //[OnInspectorGUI("EndBox", append: true)]
        //[FoldoutGroup("Enabled/TriggerType")]
        //[ToggleGroup("Enabled"), LabelText("触发概率")]
        //public string TriggerProbability = "100%";

        //[ToggleGroup("Enabled"), ShowIf("IsSkillEffect", true)]
        //public AddSkillEffetTargetType AddSkillEffectTargetType;

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
