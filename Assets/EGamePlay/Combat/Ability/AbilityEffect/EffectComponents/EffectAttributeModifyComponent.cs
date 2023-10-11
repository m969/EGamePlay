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
        }

        public override void OnEnable()
        {
            var parentEntity = Entity.GetParent<StatusAbility>().GetParent<CombatEntity>();
            var attributeModifyEffect = AttributeModifyEffect;
            var numericValue = ModifyValueFormula;
            numericValue = numericValue.Replace("%", "");
            var expression = ExpressionHelper.ExpressionParser.EvaluateExpression(numericValue);
            var value = (float)expression.Value;
            var numericModifier = new FloatModifier() { Value = value };

            var attributeType = attributeModifyEffect.AttributeType.ToString();
            //Log.Debug($"EffectAttributeModifyComponent OnEnable {attributeType} {numericValue} {attributeModifyEffect.ModifyType}");
            if (attributeModifyEffect.ModifyType == ModifyType.Add)
            {
                parentEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).AddModifier(ModifierType.FinalAdd, numericModifier);
            }
            if (attributeModifyEffect.ModifyType == ModifyType.PercentAdd)
            {
                parentEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).AddModifier(ModifierType.FinalPctAdd, numericModifier);
            }
            AttributeModifier = numericModifier;
        }

        public override void OnDisable()
        {
            var parentEntity = Entity.GetParent<StatusAbility>().GetParent<CombatEntity>();
            var attributeType = AttributeModifyEffect.AttributeType.ToString();
            //Log.Debug($"EffectAttributeModifyComponent OnDisable {attributeType} {AttributeModifyEffect.NumericValue} {AttributeModifyEffect.ModifyType}");
            if (AttributeModifyEffect.ModifyType == ModifyType.Add)
            {
                parentEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).RemoveModifier(ModifierType.FinalAdd, AttributeModifier);
            }
            if (AttributeModifyEffect.ModifyType == ModifyType.PercentAdd)
            {
                parentEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).RemoveModifier(ModifierType.FinalPctAdd, AttributeModifier);
            }
        }

        public void OnTriggerApplyEffect(Entity effectAssign)
        {

        }
    }
}