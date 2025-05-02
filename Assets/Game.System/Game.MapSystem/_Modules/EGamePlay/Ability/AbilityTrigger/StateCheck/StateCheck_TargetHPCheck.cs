using ECS;
using EGamePlay.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 判断目标当前生命值是否满足条件
    /// </summary>
    public class StateCheck_TargetHPCheck : IStateCheck
    {
        public bool CheckWith(string affectCheck, EcsEntity target)
        {
            //Log.Debug($"ConditionTargetHPCheck CheckCondition");
            if (target is IActionExecute combatAction)
            {
                target = combatAction.Target;
            }
            if (target is CombatEntity battler)
            {
                var arr = affectCheck.Split('<', '=', '≤');
                var formula = arr[1];
                formula = formula.Replace("%", $"*0.01");
                formula = formula.Replace("TargetHPMax".ToLower(), $"{battler.GetComponent<AttributeComponent>().HealthPointMax.Value}");
                var value = ExpressionHelper.ExpressionParser.Evaluate(formula);
                var targetHP = battler.GetComponent<AttributeComponent>().HealthPoint.Value;
                if (affectCheck.Contains("<") || affectCheck.Contains("≤"))
                {
                    return targetHP <= value;
                }
                if (affectCheck.Contains("="))
                {
                    return targetHP == value;
                }
            }
            Log.Debug($"ConditionTargetHPCheck CheckCondition {affectCheck} false");
            return false;
        }
    }
}
