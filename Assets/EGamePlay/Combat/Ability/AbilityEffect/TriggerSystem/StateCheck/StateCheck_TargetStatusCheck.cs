using EGamePlay.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 目标状态能力条件判断，判断目标是否有某个状态能力
    /// </summary>
    public class StateCheck_TargetStatusCheck : Entity, IStateCheck
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
            var battler = target as CombatEntity;
            if (battler.HasStatus(""))
            {
                Log.Debug($"ConditionTargetStateCheck CheckCondition {battler.Name} true");
                return true;
            }
            Log.Debug($"ConditionTargetStateCheck CheckCondition {battler.Name} {AffectCheck} false");
            return false;
        }
    }
}
