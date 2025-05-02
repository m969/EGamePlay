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
    public class EffectShieldDefenseComponent : EcsComponent
    {
        public ShieldDefenseEffect ShieldDefenseEffect { get; set; }
    }
}