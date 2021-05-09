using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat.Status
{
    public partial class LogicEntity : Entity
    {
        public Effect Effect { get; set; }


        public override void Awake(object initData)
        {
            Effect = initData as Effect;
        }

        public void ApplyEffect()
        {
            //Log.Debug("LogicEntity ApplyEffect");
            if (Effect is DamageEffect damageEffect)
            {
                var caster = GetParent<StatusAbility>().Caster;
                if (caster.DamageActionAbility.TryCreateAction(out var damageAction))
                {
                    damageAction.DamageEffect = damageEffect;
                    damageAction.Target = GetParent<StatusAbility>().GetParent<CombatEntity>();
                    damageAction.DamageSource = DamageSource.Buff;
                    damageAction.ApplyDamage();
                }
            }
            else
            {
                GetParent<StatusAbility>().ApplyEffectTo(GetParent<StatusAbility>().OwnerEntity, Effect);
            }
        }
    }
}