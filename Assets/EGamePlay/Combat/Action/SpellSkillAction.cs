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
        public SkillExecution SkillAbilityExecution { get; set; }


        //前置处理
        private void PreProcess()
        {

        }

        public void SpellSkill()
        {
            PreProcess();
            if (SkillAbilityExecution == null)
            {
                SkillAbility.ApplyAbilityEffectsTo(Target);
            }
            else
            {
                Hero.Instance.StopMove();

                if (SkillAbilityExecution.InputCombatEntity != null)
                    Hero.Instance.transform.GetChild(0).LookAt(SkillAbilityExecution.InputCombatEntity.Position);
                else if (SkillAbilityExecution.InputPoint != null)
                    Hero.Instance.transform.GetChild(0).LookAt(SkillAbilityExecution.InputPoint);
                else
                    Hero.Instance.transform.GetChild(0).localEulerAngles = new Vector3(0, SkillAbilityExecution.InputDirection, 0);

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