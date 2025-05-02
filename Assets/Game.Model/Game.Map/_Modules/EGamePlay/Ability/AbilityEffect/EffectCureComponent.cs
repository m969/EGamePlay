using ECS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class EffectCureComponent : EcsComponent
    {
        public CureEffect CureEffect { get; set; }
        public string CureValueProperty { get; set; }
    }
}