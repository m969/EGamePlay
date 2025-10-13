using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;

namespace EGamePlay
{
    public class EffectCureSystem : AComponentSystem<AbilityEffect, EffectCureComponent>,
        IAwake<AbilityEffect, EffectCureComponent>
    {
        public void Awake(AbilityEffect entity, EffectCureComponent component)
        {
            component.CureEffect = entity.EffectConfig as CureEffect;
            component.CureValueProperty = component.CureEffect.CureValueFormula;
        }

        public static int GetCureValue(AbilityEffect entity)
        {
            return ParseValue(entity);
        }

        private static int ParseValue(AbilityEffect entity)
        {
            var expression = ExpressionHelper.ExpressionParser.EvaluateExpression(entity.GetComponent<EffectCureComponent>().CureValueProperty);
            if (expression.Parameters.ContainsKey("生命值上限"))
            {
                expression.Parameters["生命值上限"].Value = entity.OwnerEntity.GetComponent<AttributeComponent>().HealthPoint.Value;
            }
            var v1 = (int)System.Math.Ceiling((float)expression.Value);
            return v1;
        }

        public static void TriggerApply(AbilityEffect entity, EffectAssignAction effectAssign)
        {
            if (entity.OwnerEntity.CureAbility.TryMakeAction(out var action))
            {
                action.SourceAssignAction = effectAssign;
                action.Target = effectAssign.Target;
                CureActionSystem.Execute(action);
            }
        }
    }
}