using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.IO;
using Sirenix.Utilities.Editor;

namespace EGamePlay.Combat
{
    public enum PropertyType
    {
        Int32,Int64,String,Float,
    }
    [InlineProperty]
    public struct PropertyData
    {
        [HorizontalGroup/*(LabelWidth = 60)*/]
        //[LabelText("属性类型")]
        [HideLabel]
        public PropertyType PropertyType;

        [HorizontalGroup/*(LabelWidth = 30)*/]
        //[LabelText("属性名")]
        [HideLabel]
        public string Name;

    }

    [CreateAssetMenu(fileName = "类设计配置")]
    [LabelText("类设计配置")]
    public class ClassDesignObject : SerializedScriptableObject
    {
        [DictionaryDrawerSettings(KeyLabel = "属性", ValueLabel = "属性设置", DisplayMode = DictionaryDisplayOptions.OneLine)]
        public Dictionary<string, PropertyData> Properties = new Dictionary<string, PropertyData>();
    }
}