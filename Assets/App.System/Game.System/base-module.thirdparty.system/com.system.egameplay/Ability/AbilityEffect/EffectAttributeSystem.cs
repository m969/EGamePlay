using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;

namespace EGamePlay
{
    public class EffectAttributeSystem : AComponentSystem<AbilityEffect, EffectAttributeModifyComponent>,
        IAwake<AbilityEffect, EffectAttributeModifyComponent>,
        IEnable<AbilityEffect, EffectAttributeModifyComponent>,
        IDisable<AbilityEffect, EffectAttributeModifyComponent>
    {
        public void Awake(AbilityEffect entity, EffectAttributeModifyComponent component)
        {
            component.AttributeModifyEffect = entity.EffectConfig as AttributeModifyEffect;
            component.ModifyValueFormula = component.AttributeModifyEffect.NumericValue;
        }

        public void Enable(AbilityEffect entity, EffectAttributeModifyComponent component)
        {
            //Log.Debug($"EffectAttributeSystem Enable {component.AttributeModifyEffect.AttributeType}");
            var parentEntity = entity.OwnerAbility.Parent;
            var attributeModifyEffect = component.AttributeModifyEffect;
            var numericValue = component.ModifyValueFormula;
            numericValue = numericValue.Replace("%", "");
            var expression = ExpressionHelper.ExpressionParser.EvaluateExpression(numericValue);
            var value = (float)expression.Value;
            var numericModifier = new FloatModifier() { Value = value };

            var attributeType = attributeModifyEffect.AttributeType;
            var numeric = AttributeSystem.GetNumeric(parentEntity as CombatEntity, attributeType);
            if (attributeModifyEffect.ModifyType == ModifyType.BaseValue)
            {
                NumericSystem.SetBase(numeric, value);
            }
            if (attributeModifyEffect.ModifyType == ModifyType.Add)
            {
                NumericSystem.AddModifier(numeric, ModifierType.FinalAdd, numericModifier);
            }
            if (attributeModifyEffect.ModifyType == ModifyType.PercentAdd)
            {
                NumericSystem.AddModifier(numeric, ModifierType.FinalPctAdd, numericModifier);
            }
            component.AttributeModifier = numericModifier;
        }

        public void Disable(AbilityEffect entity, EffectAttributeModifyComponent component)
        {
            //Log.Debug($"EffectAttributeModifyComponent OnDisable {AttributeModifyEffect.AttributeType}");
            var parentEntity = entity.OwnerAbility.Parent;
            var attributeType = component.AttributeModifyEffect.AttributeType;
            //var numeric = parentEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType);
            var numeric = AttributeSystem.GetNumeric(parentEntity as CombatEntity, attributeType);
            if (component.AttributeModifyEffect.ModifyType == ModifyType.Add)
            {
                NumericSystem.RemoveModifier(numeric, ModifierType.FinalAdd, component.AttributeModifier);
            }
            if (component.AttributeModifyEffect.ModifyType == ModifyType.PercentAdd)
            {
                NumericSystem.RemoveModifier(numeric, ModifierType.FinalPctAdd, component.AttributeModifier);
            }
        }

        public static void TriggerApply(AbilityEffect entity, EffectAssignAction effectAssign)
        {
            entity.GetComponent<EffectAttributeModifyComponent>().Enable = true;
        }
    }
}