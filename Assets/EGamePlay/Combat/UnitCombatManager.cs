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
        public Skill.SkillManager SkillManager = new Skill.SkillManager();
        public Status.StatusManager StatusManager = new Status.StatusManager();


        public void Start()
        {
            SkillManager.Start();
            StatusManager.Start();
        }

        public void Update()
        {
            SkillManager.Update();
            StatusManager.Update();
        }
    }
}