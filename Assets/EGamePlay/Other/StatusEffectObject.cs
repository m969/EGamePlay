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
    [CreateAssetMenu(fileName = "状态特效配置", menuName = "技能|状态/状态特效配置")]
    [LabelText("状态特效配置")]
    public class StatusEffectObject : SerializedScriptableObject
    {
        [LabelText("状态ID")]
        [DelayedProperty]
        public string ID = "状态1";
        [LabelText("状态名称")]
        [DelayedProperty]
        public string Name;
    }
}