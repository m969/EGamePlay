using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EGamePlay.Combat;

#if EGAMEPLAY_ET
using Unity.Mathematics;
using Vector3 = Unity.Mathematics.float3;
using Quaternion = Unity.Mathematics.float3;
using AO;
using AO.EventType;
using ET.EventType;
#else
using float3 = UnityEngine.Vector3;
#endif

namespace EGamePlay.Combat
{
    /// <summary>
    /// 状态能力执行体
    /// </summary>
    public class StatusExecution : Entity, IAbilityExecute
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public CombatEntity InputCombatEntity { get; set; }
        public Vector3 InputPoint { get; set; }
        public float InputDirection { get; set; }
        public Ability AbilityEntity { get; set; }
        public CombatEntity OwnerEntity { get; set; }


        public void BeginExecute()
        {
        }

        public void EndExecute()
        {
        }
    }
}