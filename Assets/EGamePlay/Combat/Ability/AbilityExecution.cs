using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 能力执行体，能力执行体是实际创建、执行能力表现，触发能力效果应用的地方
    /// 这里可以存一些表现执行相关的临时的状态数据
    /// </summary>
    public interface IAbilityExecution
    {
        public Entity AbilityEntity { get; set; }
        public CombatEntity OwnerEntity { get; set; }
        //public AbilityEffectComponent AbilityEffectComponent => AbilityEntity.GetComponent<AbilityEffectComponent>();
        //public List<AbilityEffect> AbilityEffects => Get<AbilityEffectComponent>().AbilityEffects;
        //public List<ExecutionEffect> ExecutionEffects => Get<ExecutionEffectComponent>().ExecutionEffects;


        //public override void Awake(object initData)
        //{
        //    AbilityEntity = initData as Entity;
        //}

        //开始执行
        public void BeginExecute();
        //{

        //}

        //结束执行
        public void EndExecute();
        //{
        //    Destroy(this);
        //}

        //public T GetAbility<T>() where T : Entity
        //{
        //    return AbilityEntity as T;
        //}
    }
}