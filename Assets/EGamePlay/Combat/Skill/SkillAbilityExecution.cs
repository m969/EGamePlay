using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace EGamePlay.Combat.Ability
{
    /// <summary>
    /// 技能执行体
    /// </summary>
    public abstract class SkillAbilityExecution : AbilityExecution
    {
        public CombatEntity InputCombatEntity { get; set; }
        public Vector3 InputPoint { get; set; }
        public float InputDirection { get; set; }
    }
}