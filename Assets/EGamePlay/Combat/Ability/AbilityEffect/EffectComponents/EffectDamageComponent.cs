using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class EffectDamageComponent : Component, IEffectTriggerSystem
    {
        public DamageEffect DamageEffect { get; set; }
        public string DamageValueFormula { get; set; }


        public override void Awake()
        {
            //Log.Debug($"EffectDamageComponent Setup");
            DamageEffect = GetEntity<AbilityEffect>().EffectConfig as DamageEffect;
            DamageValueFormula = DamageEffect.DamageValueFormula;
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
            return (int)System.Math.Ceiling((float)expression.Value);
        }

        public void OnTriggerApplyEffect(Entity effectAssign)
        {
            var effectAssignAction = effectAssign.As<EffectAssignAction>();
            if (GetEntity<AbilityEffect>().OwnerEntity.DamageAbility.TryMakeAction(out var damageAction))
            {
                damageAction.SourceAssignAction = effectAssignAction;
                damageAction.Target = effectAssignAction.Target;
                damageAction.DamageSource = DamageSource.Skill;
                damageAction.ApplyDamage();
            }
        }
    }
}