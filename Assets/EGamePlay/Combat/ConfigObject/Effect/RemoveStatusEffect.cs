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
    [Effect("移除状态效果", 40)]
    public class RemoveStatusEffect : Effect
    {
        public override string Label
        {
            get
            {
                if (this.RemoveStatus != null)
                {
                    return $"移除 [ {this.RemoveStatus.Name} ] 状态效果";
                }
                return "移除状态效果";
            }
        }

        [ToggleGroup("Enabled")]
        [LabelText("状态配置")]
        public StatusConfigObject RemoveStatus;
    }
}