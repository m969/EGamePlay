using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 主动触发观察者
    /// </summary>
    public class TriggerObserver : Entity, ICombatObserver
    {
        public void OnTrigger(Entity source)
        {
            GetParent<AbilityEffect>().OnObserverTrigger(new TriggerContext(this, source));
        }

        public void OnTriggerWithAbilityItem(AbilityItem abilityItem, CombatEntity target)
        {
            var context = new TriggerContext()
            {
                Observer = this,
                Source = abilityItem,
                AbilityItem = abilityItem,
                Target = target,
            };
            GetParent<AbilityEffect>().OnObserverTrigger(context);
        }

        public void OnTriggerWithSkillExecution(SkillExecution skillExecution, CombatEntity target)
        {
            var context = new TriggerContext()
            {
                Observer = this,
                Source = skillExecution,
                Target = target,
            };
            GetParent<AbilityEffect>().OnObserverTrigger(context);
        }
    }
}