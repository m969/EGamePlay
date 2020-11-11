using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat.Skill
{
    public abstract class AbilityEntity : Entity
    {
        public CombatEntity SpellCaster { get; set; }
        public CombatEntity SkillTarget { get; set; }
        public SkillConfigObject SkillConfigObject { get; set; }


        public void TryActivateAbility()
        {

        }

        public void ActivateAbility()
        {

        }

        public void EndAbility()
        {

        }

        public void ApplyAbilityEffect()
        {
            foreach (var item in SkillConfigObject.Effects)
            {
                if (item is DamageEffect damageEffect)
                {
                    var operation = CombatActionManager.CreateAction<DamageAction>(this.SpellCaster);
                    operation.Target = SkillTarget;
                    operation.DamageSource = DamageSource.Skill;
                    operation.DamageEffect = damageEffect;
                    operation.ApplyDamage();
                }
                else if (item is CureEffect cureEffect)
                {
                    var operation = CombatActionManager.CreateAction<CureAction>(this.SpellCaster);
                    operation.Target = SkillTarget;
                    operation.CureEffect = cureEffect;
                    operation.ApplyCure();
                }
                else
                {
                    var operation = CombatActionManager.CreateAction<AssignEffectAction>(this.SpellCaster);
                    operation.Target = SkillTarget;
                    operation.Effect = item;
                    if (item is AddStatusEffect addStatusEffect)
                    {
                        addStatusEffect.AddStatus.Duration = addStatusEffect.Duration;
                    }
                    operation.ApplyAssignEffect();
                }
            }
        }
    }
}