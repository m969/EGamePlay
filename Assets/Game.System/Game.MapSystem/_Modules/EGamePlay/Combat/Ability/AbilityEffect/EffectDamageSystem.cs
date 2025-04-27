using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;

namespace EGamePlay
{
    public class EffectDamageSystem : AComponentSystem<AbilityEffect, EffectDamageComponent>,
        IAwake<AbilityEffect, EffectDamageComponent>
    {
        public void Awake(AbilityEffect entity, EffectDamageComponent component)
        {
            component.DamageEffect = entity.EffectConfig as DamageEffect;
            component.DamageValueFormula = component.DamageEffect.DamageValueFormula;
        }

        public static int GetDamageValue(AbilityEffect entity)
        {
            return ParseDamage(entity);
        }

        private static int ParseDamage(AbilityEffect entity)
        {
            var component = entity.GetComponent<EffectDamageComponent>();
            var expression = ExpressionHelper.ExpressionParser.EvaluateExpression(component.DamageValueFormula);
            if (expression.Parameters.ContainsKey("自身攻击力"))
            {
                expression.Parameters["自身攻击力"].Value = entity.OwnerEntity.GetComponent<AttributeComponent>().Attack.Value;
            }
            return (int)System.Math.Ceiling((float)expression.Value);
        }

        public static void TriggerApply(AbilityEffect entity, EffectAssignAction effectAssign)
        {
            var component = entity.GetComponent<EffectDamageComponent>();
            if (entity.OwnerEntity.DamageAbility.TryMakeAction(out var damageAction))
            {
                damageAction.SourceAssignAction = effectAssign;
                damageAction.Target = effectAssign.Target;
                damageAction.DamageSource = DamageSource.Skill;
                DamageActionSystem.Execute(damageAction);
            }
        }
    }
}