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
    [Effect("施加状态", 30)]
    public class AddStatusEffect : Effect
    {
        public override string Label
        {
            get
            {
                if (this.AddStatus != null)
                {
                    return $"施加 [ {this.AddStatus.Name} ] 状态";
                }
                return "施加状态";
            }
        }

        [ToggleGroup("Enabled")]
        public StatusConfigObject AddStatus;

        [ToggleGroup("Enabled"), LabelText("持续时间"), SuffixLabel("毫秒", true)]
        public uint Duration;

        //[ToggleGroup("Enabled"), LabelText("参数")]
        //public string ParamValue;

        [HideReferenceObjectPicker]
        [ToggleGroup("Enabled")]
        [LabelText("参数列表")]
        public Dictionary<string, string> Params = new Dictionary<string, string>();
    }
}