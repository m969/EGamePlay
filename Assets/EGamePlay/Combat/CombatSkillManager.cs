using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay.Combat.Skill;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战斗技能管理器
    /// </summary>
    public static class CombatSkillManager
    {
        private static List<SkillAbilityEntity> CombatSkills = new List<SkillAbilityEntity>();


        public static T CreateSkill<T>() where T: SkillAbilityEntity, new()
        {
            var skill = EntityFactory.Create<T>();
            CombatSkills.Add(skill);
            return skill;
        }
    }
}