using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat.Ability
{
    public abstract class AbilityEntity<T> : AbilityEntity where T : AbilityExecution
    {
        public virtual new T CreateExecution() 
        {
            return base.CreateExecution() as T;
        }
    }

    /// <summary>
    /// 能力实体，存储着某个英雄某个能力的数据和状态
    /// </summary>
    public abstract class AbilityEntity : Entity
    {
        public CombatEntity OwnerEntity { get => GetParent<CombatEntity>(); }
        public bool Enable { get; set; } = true;
        public object ConfigObject { get; set; }
        public int Level { get; set; } = 1;


        public override void Awake(object initData)
        {
            ConfigObject = initData;
        }

        //尝试激活能力
        public virtual void TryActivateAbility()
        {
            //Log.Debug($"{GetType().Name}->TryActivateAbility");
            ActivateAbility();
        }
        
        //激活能力
        public virtual void ActivateAbility()
        {
            
        }

        //禁用能力
        public virtual void DeactivateAbility()
        {

        }

        //结束能力
        public virtual void EndAbility()
        {
            Destroy(this);
        }

        //创建能力执行体
        public virtual AbilityExecution CreateExecution()
        {
            return null;
        }
        
        public void ApplyEffectTo(CombatEntity targetEntity, Effect effectItem)
        {
            try
            {
                if (effectItem is DamageEffect damageEffect)
                {
                    if (string.IsNullOrEmpty(damageEffect.DamageValueProperty)) damageEffect.DamageValueProperty = damageEffect.DamageValueFormula;
                    if (OwnerEntity.DamageActionAbility.TryCreateAction(out var action))
                    {
                        action.Target = targetEntity;
                        action.DamageSource = DamageSource.Skill;
                        action.DamageEffect = damageEffect;
                        action.ApplyDamage();
                    }
                }
                else if (effectItem is CureEffect cureEffect)
                {
                    if (string.IsNullOrEmpty(cureEffect.CureValueProperty)) cureEffect.CureValueProperty = cureEffect.CureValueFormula;
                    if (OwnerEntity.CureActionAbility.TryCreateAction(out var action))
                    {
                        action.Target = targetEntity;
                        action.CureEffect = cureEffect;
                        action.ApplyCure();
                    }
                }
                else
                {
                    if (OwnerEntity.AssignEffectActionAbility.TryCreateAction(out var action))
                    {
                        action.Target = targetEntity;
                        action.SourceAbility = this;
                        action.Effect = effectItem;
                        if (effectItem is AddStatusEffect addStatusEffect)
                        {
                            var statusConfig = addStatusEffect.AddStatus;
                            statusConfig.Duration = addStatusEffect.Duration;
                            if (addStatusEffect.Params != null && statusConfig.Effects != null)
                            {
                                if (statusConfig.EnabledAttributeModify)
                                {
                                    statusConfig.NumericValueProperty = statusConfig.NumericValue;
                                    foreach (var item3 in addStatusEffect.Params)
                                    {
                                        if (!string.IsNullOrEmpty(statusConfig.NumericValueProperty))
                                        {
                                            statusConfig.NumericValueProperty = statusConfig.NumericValueProperty.Replace(item3.Key, item3.Value);
                                        }
                                    }
                                }
                                if (statusConfig.EnabledLogicTrigger)
                                {
                                    foreach (var item6 in statusConfig.Effects)
                                    {
                                        item6.IntervalValue = item6.Interval;
                                        item6.ConditionParamValue = item6.ConditionParam;
                                        foreach (var item3 in addStatusEffect.Params)
                                        {
                                            if (!string.IsNullOrEmpty(item6.IntervalValue))
                                            {
                                                item6.IntervalValue = item6.IntervalValue.Replace(item3.Key, item3.Value);
                                            }
                                            if (!string.IsNullOrEmpty(item6.ConditionParamValue))
                                            {
                                                item6.ConditionParamValue = item6.ConditionParamValue.Replace(item3.Key, item3.Value);
                                            }
                                        }
                                        if (item6 is DamageEffect damage)
                                        {
                                            damage.DamageValueProperty = damage.DamageValueFormula;
                                            foreach (var item4 in addStatusEffect.Params)
                                            {
                                                if (!string.IsNullOrEmpty(damage.DamageValueProperty))
                                                {
                                                    damage.DamageValueProperty = damage.DamageValueProperty.Replace(item4.Key, item4.Value);
                                                }
                                            }
                                        }
                                        else if (item6 is CureEffect cure)
                                        {
                                            cure.CureValueProperty = cure.CureValueFormula;
                                            foreach (var item5 in addStatusEffect.Params)
                                            {
                                                if (!string.IsNullOrEmpty(cure.CureValueProperty))
                                                {
                                                    cure.CureValueProperty = cure.CureValueProperty.Replace(item5.Key, item5.Value);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        action.ApplyAssignEffect();
                    }
                }
            }
            catch (System.Exception e)
            {
                Log.Error(e);
            }
        }

        //应用能力效果
        public virtual void ApplyAbilityEffectsTo(CombatEntity targetEntity)
        {

        }
    }
}