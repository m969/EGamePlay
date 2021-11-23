using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 技能施法组件
    /// </summary>
    public class SpellComponent : EGamePlay.Component
    {
        private CombatEntity CombatEntity => GetEntity<CombatEntity>();
        public override bool DefaultEnable { get; set; } = true;


        public override void Setup()
        {

        }

        public void SpellWithTarget(SkillAbility spellSkill, CombatEntity targetEntity)
        {
            if (CombatEntity.CurrentSkillExecution != null)
                return;

            //Log.Debug($"OnInputTarget {combatEntity}");
            if (CombatEntity.SpellAbility.TryMakeAction(out var action))
            {
                action.SkillAbility = spellSkill;
                action.SkillExecution = spellSkill.CreateExecution() as SkillExecution;
                action.SkillTargets.Add(targetEntity);
                action.SkillExecution.InputTarget = targetEntity;
                action.SpellSkill();
            }
        }

        public void SpellWithPoint(SkillAbility spellSkill, Vector3 point)
        {
            if (CombatEntity.CurrentSkillExecution != null)
                return;

            //Log.Debug($"OnInputPoint {point}");
            if (CombatEntity.SpellAbility.TryMakeAction(out var action))
            {
                action.SkillAbility = spellSkill;
                action.SkillExecution = spellSkill.CreateExecution() as SkillExecution;
                action.SkillExecution.InputPoint = point;
                action.SpellSkill();
            }
        }

        public void SpellWithDirect(SkillAbility spellSkill, float direction, Vector3 point)
        {
            if (CombatEntity.CurrentSkillExecution != null)
                return;

            //Log.Debug($"OnInputDirect {direction}");
            if (CombatEntity.SpellAbility.TryMakeAction(out var action))
            {
                action.SkillAbility = spellSkill;
                action.SkillExecution = spellSkill.CreateExecution() as SkillExecution;
                action.SkillExecution.InputPoint = point;
                action.SkillExecution.InputDirection = direction;
                action.SpellSkill();
            }
        }
    }
}