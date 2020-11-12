using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat.Status
{
    public partial class EffectEntity : Entity
    {
        public bool Enabled { get; private set; }
        public CombatEntity Caster { get; set; }
        public Effect Effect { get; set; }


        public override void Awake(object paramObject)
        {
            Effect = paramObject as Effect;
            switch (Effect.EffectTriggerType)
            {
                //case EffectTriggerType.Immediate:
                //    break;
                case EffectTriggerType.Condition:
                    break;
                case EffectTriggerType.Action:
                    break;
                case EffectTriggerType.Interval:
                    break;
                default:
                    break;
            }
        }

        public void ApplyEffect()
        {
            var combatEntity = Parent as CombatEntity;
            if (Effect is DamageEffect damageEffect)
            {
                var damageAction = CombatActionManager.CreateAction<DamageAction>(Caster);
                damageAction.DamageEffect = damageEffect;
                damageAction.Target = combatEntity; ;
                damageAction.DamageSource = DamageSource.Buff;
                damageAction.ApplyDamage();
            }
        }
    }
}