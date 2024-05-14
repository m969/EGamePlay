using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class EffectCureComponent : Component, IEffectTriggerSystem
    {
        public CureEffect CureEffect { get; set; }
        public string CureValueProperty { get; set; }


        public override void Awake()
        {
            CureEffect = GetEntity<AbilityEffect>().EffectConfig as CureEffect;
            CureValueProperty = CureEffect.CureValueFormula;
            //Log.Debug($"EffectCureComponent {CureValueProperty}");
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
            var v1 = (int)System.Math.Ceiling((float)expression.Value);
            return v1;
        }

        public void OnTriggerApplyEffect(Entity effectAssign)
        {
            //Log.Debug($"EffectCureComponent OnTriggerApplyEffect");
            var effectAssignAction = effectAssign.As<EffectAssignAction>();
            if (GetEntity<AbilityEffect>().OwnerEntity.CureAbility.TryMakeAction(out var action))
            {
                action.SourceAssignAction = effectAssignAction;
                action.Target = effectAssignAction.Target;
                action.ApplyCure();
            }
        }
    }
}