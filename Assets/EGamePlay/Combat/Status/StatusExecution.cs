using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EGamePlay.Combat;
#if EGAMEPLAY_ET
using Vector3 = Unity.Mathematics.float3;
#endif

namespace EGamePlay.Combat
{
    /// <summary>
    /// 状态能力执行体
    /// </summary>
    public abstract class StatusExecution : Entity, IAbilityExecute
    {
        public CombatEntity InputCombatEntity { get; set; }
        public Vector3 InputPoint { get; set; }
        public float InputDirection { get; set; }
        public Entity AbilityEntity { get; set; }
        public CombatEntity OwnerEntity { get; set; }


        public void BeginExecute()
        {
        }

        public void EndExecute()
        {
        }
    }
}