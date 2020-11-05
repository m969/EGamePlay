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
            var config = Resources.Load<SkillConfigObject>("SkillConfigs/Skill_1002_炎爆");
            SkillConfigs.Add(1002, config);
            //var skill1002 = Entity.Create<SkillEntity>();
            //skill1002.SkillConfigObject = Resources.Load<SkillConfigObject>("SkillConfigs/Skill_1002_炎爆");
            //Skills.Add(skill1002);
            //foreach (var item in Skills)
            //{
            //    item.SpellCaster = Parent as CombatEntity;
            //    item.Start();
            //}
        }

        public void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    Skills[0].StartListen();
            //}

            foreach (var item in Skills)
            {
                item.Update();
            }
        }

        public T CreateSkill<T>() where T : SkillEntity, new()
        {
            var skill = Entity.Create<T>();
            return skill;
        }
    }
}