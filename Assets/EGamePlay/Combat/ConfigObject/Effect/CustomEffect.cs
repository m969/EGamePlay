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
    [Effect("自定义效果", 1000)]
    public class CustomEffect : Effect
    {
        public override string Label => "自定义效果";

        [ToggleGroup("Enabled"), LabelText("自定义效果")]
        public string CustomEffectType;

        [ToggleGroup("Enabled"), LabelText("参数列表")]
        public Dictionary<string, string> Params = new Dictionary<string, string>();
    }
}