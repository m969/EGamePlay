using ECS;
using EGamePlay.Combat;
using GameUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static UnityEngine.GraphicsBuffer;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 判断随机概率是否满足条件
    /// </summary>
    public class StateCheck_RandomCheck : IStateCheck
    {
        public bool CheckWith(string affectCheck, EcsEntity target)
        {
            if (target is IActionExecute combatAction)
            {
                target = combatAction.Target;
            }
            var arr = affectCheck.Split('<', '=', '≤');
            var formula = arr[1];
            formula = formula.Replace("%", $"*0.01");
            var value = ExpressionHelper.ExpressionParser.Evaluate(formula);
            var random = RandomHelper.RandomRate();
            if (affectCheck.Contains("<") || affectCheck.Contains("≤"))
            {
                return random <= value * 100;
            }
            Log.Debug($"StateCheck_RandomCheck CheckCondition {affectCheck} false");
            return false;
        }
    }
}
