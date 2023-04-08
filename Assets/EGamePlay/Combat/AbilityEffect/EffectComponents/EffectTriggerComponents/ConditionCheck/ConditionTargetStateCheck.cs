using EGamePlay.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EGamePlay.Combat
{
    public class ConditionTargetStateCheck : Entity, IConditionCheckSystem
    {
        public CombatEntity OwnerBattler => GetParent<EffectTriggerEventBind>().GetParent<AbilityEffect>().OwnerEntity;
        public string AffectCheck { get; set; }
        public bool IsInvert => AffectCheck.StartsWith("!");


        public override void Awake(object initData)
        {
            AffectCheck = initData.ToString().ToLower();
        }

        public bool CheckCondition(Entity target)
        {
            if (target is IActionExecution combatAction)
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
