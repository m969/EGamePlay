using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 能力执行接口，具体的技能执行体、行动执行体等都需要实现这个接口，执行体是实际创建能力表现、执行能力表现，触发能力效果应用的地方
    /// 执行体里可以存一些表现执行相关的临时的状态数据
    /// </summary>
    public interface IAbilityExecute
    {
        public Entity AbilityEntity { get; set; }
        public CombatEntity OwnerEntity { get; set; }


        /// 开始执行
        public void BeginExecute();

        /// 结束执行
        public void EndExecute();
    }
}