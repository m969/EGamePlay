using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EGamePlay.Combat.Ability;

namespace EGamePlay.Combat.Skill
{
    /// <summary>
    /// 技能执行体
    /// </summary>
    public abstract class SkillAbilityExecution : AbilityExecution
    {
        public CombatEntity InputCombatEntity { get; set; }
        public Vector3 InputPoint { get; set; }
        public float InputDirection { get; set; }


        public override void BeginExecute()
        {
            base.BeginExecute();
            GetAbility<SkillAbility>().Spelling = true;
        }

        public override void EndExecute()
        {
            base.EndExecute();
            GetAbility<SkillAbility>().Spelling = false;
        }
    }
}