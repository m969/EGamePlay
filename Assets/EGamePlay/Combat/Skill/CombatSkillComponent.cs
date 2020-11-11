using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EGamePlay.Combat.Skill
{
    public sealed class CombatSkillComponent : Component
    {
        public List<SkillEntity> Skills { get; set; } = new List<SkillEntity>();
        public Dictionary<int, SkillConfigObject> SkillConfigs { get; set; } = new Dictionary<int, SkillConfigObject>();


        public override void Setup()
        {

        }
    }
}