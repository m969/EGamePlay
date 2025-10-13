using ECS;
using ET;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class EffectAddBuffComponent : EcsComponent
    {
        public AddStatusEffect AddStatusEffect { get; set; }
        public float Duration { get; set; }
        public string NumericValueProperty { get; set; }
    }
}