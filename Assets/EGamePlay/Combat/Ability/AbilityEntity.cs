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
            Log.Debug($"{GetType().Name}->TryActivateAbility");
            ActivateAbility();
        }
        
        public virtual void ActivateAbility()
        {
            
        }

        public virtual void EndActivate()
        {
            Entity.Destroy(this);
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
                if (statusConfigObject.EnabledLogicTrigger)
                {
                    Effects = statusConfigObject.Effects;
                }
            }
            if (Effects == null)
            {
                return;
            }
            foreach (var item in Effects)
            {
                if (item is DamageEffect damageEffect)
                {
                    var action = CombatActionManager.CreateAction<DamageAction>(this.AbilityOwner);
                    action.Target = targetEntity;
                    action.DamageSource = DamageSource.Skill;
                    action.DamageEffect = damageEffect;
                    action.ApplyDamage();
                }
                else if (item is CureEffect cureEffect)
                {
                    var action = CombatActionManager.CreateAction<CureAction>(this.AbilityOwner);
                    action.Target = targetEntity;
                    action.CureEffect = cureEffect;
                    action.ApplyCure();
                }
                else
                {
                    var action = CombatActionManager.CreateAction<AssignEffectAction>(this.AbilityOwner);
                    action.Target = targetEntity;
                    action.Effect = item;
                    if (item is AddStatusEffect addStatusEffect)
                    {
                        addStatusEffect.AddStatus.Duration = addStatusEffect.Duration;
                        addStatusEffect.AddStatus.NumericValue = addStatusEffect.ParamValue;
                    }
                    action.ApplyAssignEffect();
                }
            }
        }
    }
}