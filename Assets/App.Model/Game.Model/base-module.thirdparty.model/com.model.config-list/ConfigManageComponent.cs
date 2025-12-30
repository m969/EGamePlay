using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ET;
using System;
using System.Reflection;
using ECS;

#if !EGAMEPLAY_ET
namespace EGamePlay.Combat
{
    public class ConfigManageComponent : EcsComponent
    {
        public ReferenceCollector ConfigsCollector { get; set; }
        public Dictionary<Type, object> TypeConfigCategarys { get; set; } = new Dictionary<Type, object>();
    }
}
#endif