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
    [Effect("移除状态", 40)]
    public class RemoveStatusEffect : Effect
    {
        public override string Label
        {
            get
            {
                if (this.RemoveStatus != null)
                {
                    return $"移除 [ {this.RemoveStatus.Name} ] 状态";
                }
                return "移除状态";
            }
        }

        [ToggleGroup("Enabled")]
        public StatusConfigObject RemoveStatus;
    }
}