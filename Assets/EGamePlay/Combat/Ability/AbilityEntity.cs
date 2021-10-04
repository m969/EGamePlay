using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
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
        public virtual CombatEntity OwnerEntity { get => GetParent<CombatEntity>(); set { } }
        public CombatEntity ParentEntity => GetParent<CombatEntity>();
        public AbilityEffectComponent AbilityEffectComponent => GetComponent<AbilityEffectComponent>();
        public List<AbilityEffect> AbilityEffects => GetComponent<AbilityEffectComponent>().AbilityEffects;
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
    }
}
