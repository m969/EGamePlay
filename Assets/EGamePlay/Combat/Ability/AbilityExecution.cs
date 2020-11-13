using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace EGamePlay.Combat.Ability
{
    /// <summary>
    /// 能力执行体，能力执行体是实际创建处理能力表现，应用能力效果的地方
    /// </summary>
    public abstract class AbilityExecution : Entity
    {
        public AbilityEntity AbilityEntity { get; set; }
        public CombatEntity AbilityExecutionTarget { get; set; }
        public CombatEntity InputCombatEntity { get; set; }
        public Vector3 InputPoint { get; set; }
        public float InputDirection { get; set; }


        public override void Awake(object paramObject)
        {
            AbilityEntity = paramObject as AbilityEntity;
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
            
        }
        //////////////////////////
    }
}