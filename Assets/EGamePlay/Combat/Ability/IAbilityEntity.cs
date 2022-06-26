using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
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

        /// 激活能力
        public void ActivateAbility();

        /// 禁用能力
        public void DeactivateAbility();

        /// 结束能力
        public void EndAbility();

        /// 创建能力执行体
        public Entity CreateExecution();
    }
}
