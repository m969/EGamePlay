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
    [Effect("施加状态")]
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
    }
}