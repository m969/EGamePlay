using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public static class AbilityEntityHelper
    {
        public static void Test(this IAbilityEntity abilityEntity)
        {
            
        }
    }

    public interface IAbilityEntity<T> : IAbilityEntity where T : IAbilityExecution
    {
        public new T CreateExecution();
        //{
        //    return base.CreateExecution() as T;
        //}
    }

    /// <summary>
    /// 能力实体，存储着某个英雄某个能力的数据和状态
    /// </summary>
    public interface IAbilityEntity
    {
        public CombatEntity OwnerEntity { get; set; }
        public CombatEntity ParentEntity { get; }
        public bool Enable { get; set; }


        /// 尝试激活能力
        public void TryActivateAbility();
        //{
        //    ActivateAbility();
        //}

        /// 激活能力
        public void ActivateAbility();
        //{
        //    FireEvent(nameof(ActivateAbility));
        //}

        /// 禁用能力
        public void DeactivateAbility();
        //{

        //}

        /// 结束能力
        public void EndAbility();
        //{
        //    Destroy(this);
        //}

        /// 创建能力执行体
        public Entity CreateExecution();
        //{
        //    return null;
        //}
    }
}
