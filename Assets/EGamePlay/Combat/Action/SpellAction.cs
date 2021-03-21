using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat.Skill;
using ET;

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
            Creator.TriggerActionPoint(ActionPointType.PreSpell, this);
        }

        public void SpellSkill()
        {
            PreProcess();
            if (SkillExecution == null)
            {
                SkillAbility.ApplyAbilityEffectsTo(Target);
                PostProcess();
                ApplyAction();
            }
            else
            {
                if (SkillTargets.Count > 0)
                {
                    SkillExecution.SkillTargets.AddRange(SkillTargets);
                }
                SkillExecution.BeginExecute();
                AddComponent<UpdateComponent>();
            }
        }

        public override void Update()
        {
            if (SkillExecution != null)
            {
                if (SkillExecution.IsDisposed)
                {
                    PostProcess();
                    ApplyAction();
                }
            }
        }

        //后置处理
        private void PostProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.PostSpell, this);
        }
    }
}