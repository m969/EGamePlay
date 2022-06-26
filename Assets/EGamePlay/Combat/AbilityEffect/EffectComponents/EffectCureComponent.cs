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


        public override void Awake()
        {
            CureEffect = GetEntity<AbilityEffect>().EffectConfig as CureEffect;
            CureValueProperty = CureEffect.CureValueFormula;
            Entity.OnEvent(nameof(AbilityEffect.StartAssignEffect), OnAssignEffect);
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

        private void OnAssignEffect(Entity entity)
        {
            //Log.Debug($"EffectCureComponent OnAssignEffect");
            var effectAssignAction = entity.As<EffectAssignAction>();
            if (GetEntity<AbilityEffect>().OwnerEntity.CureAbility.TryMakeAction(out var action))
            {
                effectAssignAction.FillDatasToAction(action);
                action.ApplyCure();
            }
        }
    }
}