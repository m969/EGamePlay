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
    public class SpellSkillAction : CombatAction
    {
        public int SkillID { get; set; }
        public SkillAbilityEntity SkillEntity { get; set; }


        private void BeforeSpell()
        {
            //SkillEntity.SkillTarget = Target;
            //SkillEntity.Start();
        }

        public void SpellSkill()
        {
            BeforeSpell();
            //SkillEntity.StartRun();
            SkillEntity.ApplyAbilityEffect(Target);
            //SkillEntity.EndRun();
            AfterSpell();
        }

        private void AfterSpell()
        {

        }
    }
}