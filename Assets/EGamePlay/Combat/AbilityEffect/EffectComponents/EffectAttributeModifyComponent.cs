using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class EffectAttributeModifyComponent : Component
    {
        public AddStatusEffect AddStatusEffect { get; set; }


        public override void Setup()
        {
            AddStatusEffect = GetEntity<AbilityEffect>().EffectConfig as AddStatusEffect;
        }

        public int GetNumericValue()
        {
            return 1;
        }
    }
}