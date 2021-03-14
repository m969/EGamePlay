using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat.Skill;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 施法行动
    /// </summary>
    public class SpellAction : CombatAction
    {
        public SkillAbility SkillAbility { get; set; }
        public SkillExecution SkillExecution { get; set; }
        public List<CombatEntity> SkillTargets { get; set; } = new List<CombatEntity>();
        public Vector3 InputPoint { get; set; }
        public float InputDirection { get; set; }


        //前置处理
        private void PreProcess()
        {

        }

        public void SpellSkill()
        {
            PreProcess();
            if (SkillExecution == null)
            {
                SkillAbility.ApplyAbilityEffectsTo(Target);
            }
            else
            {
                Hero.Instance.StopMove();

                if (SkillTargets.Count == 0)
                {
                    if (SkillExecution.InputTarget != null)
                        Hero.Instance.transform.GetChild(0).LookAt(SkillExecution.InputTarget.Position);
                    else if (SkillExecution.InputPoint != null)
                        Hero.Instance.transform.GetChild(0).LookAt(SkillExecution.InputPoint);
                    else
                        Hero.Instance.transform.GetChild(0).localEulerAngles = new Vector3(0, SkillExecution.InputDirection, 0);
                }
                else
                {
                    SkillExecution.SkillTargets.AddRange(SkillTargets);
                }

                SkillExecution.BeginExecute();
            }
            PostProcess();

            ApplyAction();
        }

        //后置处理
        private void PostProcess()
        {
            //Creator.TriggerActionPoint(ActionPointType.PostGiveCure, this);
            //Target.TriggerActionPoint(ActionPointType.PostReceiveCure, this);
        }
    }
}