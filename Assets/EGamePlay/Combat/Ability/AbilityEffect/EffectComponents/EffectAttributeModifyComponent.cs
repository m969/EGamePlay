using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class EffectAttributeModifyComponent : Component, IEffectTriggerSystem
    {
        public override bool DefaultEnable => false;
        public AttributeModifyEffect AttributeModifyEffect { get; set; }
        public FloatModifier AttributeModifier { get; set; }
        public string ModifyValueFormula { get; set; }


        public override void Awake()
        {
            AttributeModifyEffect = GetEntity<AbilityEffect>().EffectConfig as AttributeModifyEffect;
            ModifyValueFormula = AttributeModifyEffect.NumericValue;
        }

        public override void OnEnable()
        {
            var parentEntity = GetEntity<AbilityEffect>().OwnerAbility.Parent;
            var attributeModifyEffect = AttributeModifyEffect;
            var numericValue = ModifyValueFormula;
            numericValue = numericValue.Replace("%", "");
            var expression = ExpressionHelper.ExpressionParser.EvaluateExpression(numericValue);
            var value = (float)expression.Value;
            var numericModifier = new FloatModifier() { Value = value };

            var attributeType = attributeModifyEffect.AttributeType;
            var numeric = parentEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType);
            if (attributeModifyEffect.ModifyType == ModifyType.BaseValue)
            {
                numeric.SetBase(value);
            }
            if (attributeModifyEffect.ModifyType == ModifyType.Add)
            {
                numeric.AddModifier(ModifierType.FinalAdd, numericModifier);
            }
            if (attributeModifyEffect.ModifyType == ModifyType.PercentAdd)
            {
                numeric.AddModifier(ModifierType.FinalPctAdd, numericModifier);
            }
            AttributeModifier = numericModifier;
        }

        public override void OnDisable()
        {
            var parentEntity = GetEntity<AbilityEffect>().OwnerAbility.Parent;
            var attributeType = AttributeModifyEffect.AttributeType;
            var numeric = parentEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType);
            if (AttributeModifyEffect.ModifyType == ModifyType.Add)
            {
                numeric.RemoveModifier(ModifierType.FinalAdd, AttributeModifier);
            }
            if (AttributeModifyEffect.ModifyType == ModifyType.PercentAdd)
            {
                numeric.RemoveModifier(ModifierType.FinalPctAdd, AttributeModifier);
            }
        }

        public void OnTriggerApplyEffect(Entity effectAssign)
        {

        }
    }
}