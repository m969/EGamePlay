using ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class EffectDamageComponent : EcsComponent
    {
        public DamageEffect DamageEffect { get; set; }
        public string DamageValueFormula { get; set; }
    }
}