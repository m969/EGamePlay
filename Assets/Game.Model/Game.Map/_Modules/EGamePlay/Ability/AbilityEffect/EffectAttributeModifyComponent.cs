using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class EffectAttributeModifyComponent : ECS.EcsComponent
    {
        public AttributeModifyEffect AttributeModifyEffect { get; set; }
        public FloatModifier AttributeModifier { get; set; }
        public string ModifyValueFormula { get; set; }
    }
}