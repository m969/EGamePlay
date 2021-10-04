using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class EffectCureComponent : Component
    {
        public CureEffect CureEffect { get; set; }
        public string CureValueProperty { get; set; }


        public override void Setup()
        {
            CureEffect = GetEntity<AbilityEffect>().EffectConfig as CureEffect;
            CureValueProperty = CureEffect.CureValueFormula;
        }

        public int GetCureValue()
        {
            return ParseValue();
        }

        private int ParseValue()
        {
            var expression = ExpressionHelper.ExpressionParser.EvaluateExpression(CureValueProperty);
            if (expression.Parameters.ContainsKey("生命值上限"))
            {
                expression.Parameters["生命值上限"].Value = GetEntity<AbilityEffect>().OwnerEntity.GetComponent<AttributeComponent>().HealthPoint.Value;
            }
            return Mathf.CeilToInt((float)expression.Value);
        }
    }
}