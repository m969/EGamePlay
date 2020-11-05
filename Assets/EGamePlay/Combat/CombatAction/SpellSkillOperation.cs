using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat.Skill;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 施放技能行动
    /// </summary>
    public class SpellSkillOperation : CombatOperation
    {
        public SkillEntity SkillEntity { get; set; }


        private void BeforeSpell()
        {
            SkillEntity.SkillTarget = Target;
        }

        public void SpellSkill()
        {
            BeforeSpell();
            SkillEntity.AssignSkillEffect();
            AfterSpell();
        }

        private void AfterSpell()
        {

        }
    }
}