using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace EGamePlay.Combat.Ability
{
    /// <summary>
    /// 能力执行体，能力执行体是实际创建处理能力表现，应用能力效果的地方
    /// 这里会存一些表现执行相关的临时的状态数据
    /// </summary>
    public abstract class AbilityExecution : Entity
    {
        public AbilityEntity AbilityEntity { get; set; }
        public CombatEntity InputCombatEntity { get; set; }
        public Vector3 InputPoint { get; set; }
        public float InputDirection { get; set; }


        public override void Awake(object initData)
        {
            AbilityEntity = initData as AbilityEntity;
        }

        //////////////////////////
        public virtual void BeginExecute()
        {

        }
        /////////////
        // 能力表现
        /////////////
        public virtual void EndExecute()
        {
            Destroy(this);
        }
        //////////////////////////
    }
}