using EGamePlay.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class ConditionTargetHPCheck : Entity, IConditionCheckSystem
    {
        public CombatEntity OwnerBattler => Parent.GetParent<AbilityEffect>().OwnerEntity;
        public string AffectCheck { get; set; }
        public bool IsInvert => AffectCheck.StartsWith("!");


        public override void Awake(object initData)
        {
            AffectCheck = initData.ToString().ToLower();
        }

        public bool CheckCondition(Entity target)
        {
            //Log.Debug($"ConditionTargetHPCheck CheckCondition");
            if (target is IActionExecute combatAction)
            {
                target = combatAction.Target;
            }
            var battler = target as CombatEntity;
            var arr = AffectCheck.Split('<', '=', '≤');
            var formula = arr[1];
            formula = formula.Replace("%", $"*0.01");
            formula = formula.Replace("TargetHPMax".ToLower(), $"{battler.GetComponent<AttributeComponent>().HealthPointMax.Value}");
            var value = ExpressionHelper.ExpressionParser.Evaluate(formula);
            var targetHP = battler.GetComponent<AttributeComponent>().HealthPoint.Value;
            if (AffectCheck.Contains("<") || AffectCheck.Contains("≤"))
            {
                //Log.Debug($"{targetHP} < {value}");
                return targetHP <= value;
            }
            if (AffectCheck.Contains("="))
            {
                return targetHP == value;
            }
            Log.Debug($"ConditionTargetHPCheck CheckCondition {battler.Name} {AffectCheck} false");
            return false;
        }
    }
}
