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
    public class ItemEffect : System.Object
#endif
    {
        [HideInInspector]
        public virtual string Label => "Event";

        [ToggleGroup("Enabled", "$Label")]
        public bool Enabled;

        [ToggleGroup("Enabled"), LabelText("执行点")]
        public ItemTriggerType TriggerType = ItemTriggerType.CollisionTrigger;

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
