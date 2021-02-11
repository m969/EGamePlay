using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EGamePlay.Combat.Ability;

namespace EGamePlay.Combat.Skill
{
    /// <summary>
    /// 状态能力执行体
    /// </summary>
    public abstract class StatusExecution : AbilityExecution
    {
        public CombatEntity InputCombatEntity { get; set; }
        public Vector3 InputPoint { get; set; }
        public float InputDirection { get; set; }
    }
}