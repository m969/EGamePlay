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
        public string DamageValueFormula { get; set; }


        public override void Setup()
        {
            //Log.Debug($"EffectDamageComponent Setup");
            DamageEffect = GetEntity<AbilityEffect>().EffectConfig as DamageEffect;
            DamageValueFormula = DamageEffect.DamageValueFormula;
            Entity.OnEvent(nameof(AbilityEffect.StartAssignEffect), OnAssignEffect);
        }

        public int GetDamageValue()
        {
            return ParseDamage();
        }

        private int ParseDamage()
        {
            var expression = ExpressionHelper.ExpressionParser.EvaluateExpression(DamageValueFormula);
            if (expression.Parameters.ContainsKey("自身攻击力"))
            {
                expression.Parameters["自身攻击力"].Value = GetEntity<AbilityEffect>().OwnerEntity.GetComponent<AttributeComponent>().Attack.Value;
            }
            return Mathf.CeilToInt((float)expression.Value);
        }

        private void OnAssignEffect(Entity entity)
        {
            //Log.Debug($"EffectDamageComponent OnAssignEffect");
            var effectAssignAction = entity.As<EffectAssignAction>();
            if (GetEntity<AbilityEffect>().OwnerEntity.DamageAbility.TryMakeAction(out var damageAction))
            {
                effectAssignAction.FillDatasToAction(damageAction);
                damageAction.DamageSource = DamageSource.Skill;
                damageAction.ApplyDamage();
            }
        }
    }
}