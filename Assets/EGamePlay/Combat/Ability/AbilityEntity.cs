using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat.Ability
{
    /// <summary>
    /// 能力实体，存储着某个英雄某个能力的数据和状态
    /// </summary>
    public abstract class AbilityEntity : Entity
    {
        public CombatEntity AbilityOwner { get; set; }
        public object ConfigObject { get; set; }


        public override void Awake(object paramObject)
        {
            ConfigObject = paramObject;
            this.AbilityOwner = Parent as CombatEntity;
        }

        public virtual void TryActivateAbility()
        {
            Log.Debug($"{GetType().Name} TryActivateAbility");
            ActivateAbility();
        }
        
        public virtual void ActivateAbility()
        {
            
        }

        public virtual void EndAbility()
        {

        }

        public virtual AbilityExecution CreateAbilityExecution()
        {
            return null;
        }
        
        public virtual void ApplyAbilityEffect(CombatEntity targetEntity)
        {
            List<Effect> Effects = null;
            if (ConfigObject is SkillConfigObject skillConfigObject)
            {
                Effects = skillConfigObject.Effects;
            }
            if (ConfigObject is StatusConfigObject statusConfigObject)
            {
                Effects = statusConfigObject.Effects;
            }
            foreach (var item in Effects)
            {
                if (item is DamageEffect damageEffect)
                {
                    var operation = CombatActionManager.CreateAction<DamageAction>(this.AbilityOwner);
                    operation.Target = targetEntity;
                    operation.DamageSource = DamageSource.Skill;
                    operation.DamageEffect = damageEffect;
                    operation.ApplyDamage();
                }
                else if (item is CureEffect cureEffect)
                {
                    var operation = CombatActionManager.CreateAction<CureAction>(this.AbilityOwner);
                    operation.Target = targetEntity;
                    operation.CureEffect = cureEffect;
                    operation.ApplyCure();
                }
                else
                {
                    var operation = CombatActionManager.CreateAction<AssignEffectAction>(this.AbilityOwner);
                    operation.Target = targetEntity;
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