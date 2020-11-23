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
                var damageAction = CombatActionManager.CreateAction<DamageAction>(GetParent<StatusAbilityEntity>().Caster);
                damageAction.DamageEffect = damageEffect;
                damageAction.Target = GetParent<StatusAbilityEntity>().GetParent<CombatEntity>();
                damageAction.DamageSource = DamageSource.Buff;
                damageAction.ApplyDamage();
            }
        }
    }
}