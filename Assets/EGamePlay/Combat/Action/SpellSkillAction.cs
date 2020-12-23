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
        public SkillAbility SkillAbility { get; set; }
        public SkillAbilityExecution SkillAbilityExecution { get; set; }


        //前置处理
        private void PreProcess()
        {

        }

        public void SpellSkill()
        {
            PreProcess();
            if (SkillAbilityExecution == null)
            {
                SkillAbility.ApplyAbilityEffect(Target);
            }
            else
            {
                SkillAbilityExecution.BeginExecute();
            }
            PostProcess();
        }

        //后置处理
        private void PostProcess()
        {
            //Creator.TriggerActionPoint(ActionPointType.PostGiveCure, this);
            //Target.TriggerActionPoint(ActionPointType.PostReceiveCure, this);
        }
    }
}