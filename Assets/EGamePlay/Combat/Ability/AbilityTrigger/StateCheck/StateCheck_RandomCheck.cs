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
    public class StateCheck_RandomCheck : Entity, IStateCheck
    {
        public CombatEntity OwnerBattler => Parent.GetParent<AbilityEffect>().OwnerEntity;
        public string AffectCheck { get; set; }
        public bool IsInvert => AffectCheck.StartsWith("!");


        public override void Awake(object initData)
        {
            AffectCheck = initData.ToString().ToLower();
        }

        public bool CheckWith(Entity target)
        {
            if (target is IActionExecute combatAction)
            {
                target = combatAction.Target;
            }
            var arr = AffectCheck.Split('<', '=', '≤');
            var formula = arr[1];
            formula = formula.Replace("%", $"*0.01");
            var value = ExpressionHelper.ExpressionParser.Evaluate(formula);
            var random = RandomHelper.RandomRate();
            if (AffectCheck.Contains("<") || AffectCheck.Contains("≤"))
            {
                return random <= value * 100;
            }
            Log.Debug($"StateCheck_RandomCheck CheckCondition {AffectCheck} false");
            return false;
        }
    }
}
