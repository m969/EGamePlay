using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EGamePlay.Combat.Skill
{
    public class SkillManager
    {
        public List<SkillEntity> Skills { get; set; } = new List<SkillEntity>();


        public void Start()
        {
            Skills.Add(new SkillEntity() { SkillConfigObject = Resources.Load<SkillConfigObject>("SkillConfigs/Skill_1002_炎爆") });
            foreach (var item in Skills)
            {
                item.Start();
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Skills[0].StartListen();
            }

            foreach (var item in Skills)
            {
                item.Update();
            }
        }
    }
}