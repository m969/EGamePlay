using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 单位战斗管理器
    /// </summary>
    public sealed class UnitCombatManager
    {
        public Skill.CombatSkillComponent SkillManager = new Skill.CombatSkillComponent();
        public Status.StatusManager StatusManager = new Status.StatusManager();


        public void Start()
        {
        }

        public void Update()
        {
        }
    }
}