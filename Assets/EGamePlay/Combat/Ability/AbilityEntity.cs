using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat.Ability
{
    public enum PassiveAbilityExcutionType
    {

    }

    public class AbilityEntity : Entity
    {
        public CombatEntity SpellCaster { get; set; }
        public CombatEntity SkillTarget { get; set; }
        public SkillConfigObject SkillConfigObject { get; set; }
        public CombatEntity InputCombatEntity { get; set; }
        public Vector3 InputPoint { get; set; }
        public float InputDirection { get; set; }


        public override void Awake(object paramObject)
        {
            SkillConfigObject = paramObject as SkillConfigObject;
            if (SkillConfigObject.SkillSpellType == SkillSpellType.Passive)
            {
                TryActivateAbility();
            }
        }

        public void TryActivateAbility()
        {
            ActivateAbility();
        }
        //////////////////////////
        public virtual void ActivateAbility()
        {
            
        }
        /////////////
        // 技能表现
        /////////////
        public virtual void EndAbility()
        {

        }
        //////////////////////////
        public virtual void ApplyAbilityEffect()
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