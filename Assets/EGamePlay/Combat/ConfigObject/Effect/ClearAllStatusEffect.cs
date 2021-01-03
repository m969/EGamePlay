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
    [Effect("移除所有状态效果", 50)]
    public class ClearAllStatusEffect : Effect
    {
        public override string Label => "移除所有状态效果";
    }
}