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
        private static List<SkillEntity> CombatSkills = new List<SkillEntity>();


        public static T CreateSkill<T>() where T: SkillEntity, new()
        {
            var operation = new T();
            CombatSkills.Add(operation);
            return operation;
        }
    }
}