using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class EffectDamageComponent : Component
    {
        public DamageEffect DamageEffect { get; set; }
        public string DamageValueProperty { get; set; }


        public override void Setup()
        {
            DamageEffect = GetEntity<AbilityEffect>().EffectConfig as DamageEffect;
            DamageValueProperty = DamageEffect.DamageValueFormula;
        }

        public int GetDamageValue()
        {
            return ParseDamage();
        }

        private int ParseDamage()
        {
            var expression = ExpressionHelper.ExpressionParser.EvaluateExpression(DamageValueProperty);
            if (expression.Parameters.ContainsKey("自身攻击力"))
            {
                expression.Parameters["自身攻击力"].Value = GetEntity<AbilityEffect>().OwnerEntity.GetComponent<AttributeComponent>().Attack.Value;
            }
            return Mathf.CeilToInt((float)expression.Value);
        }
    }
}