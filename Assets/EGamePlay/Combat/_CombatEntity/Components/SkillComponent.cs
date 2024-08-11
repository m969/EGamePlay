using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class SkillComponent : Component
	{
        public Dictionary<string, Ability> NameSkills { get; set; } = new Dictionary<string, Ability>();
        public Dictionary<int, Ability> IdSkills { get; set; } = new Dictionary<int, Ability>();
        public Dictionary<KeyCode, Ability> InputSkills { get; set; } = new Dictionary<KeyCode, Ability>();

        public Ability AttachSkill(object configObject)
        {
            var abilityComp = Entity.GetComponent<AbilityComponent>();
            var skill = abilityComp.AttachAbility(configObject);
            NameSkills.Add(skill.Config.Name, skill);
            IdSkills.Add(skill.Config.Id, skill);
            return skill;
        }

        public void RemoveSkill(Ability skill)
        {
            var abilityComp = Entity.GetComponent<AbilityComponent>();
            NameSkills.Remove(skill.Config.Name);
            IdSkills.Remove(skill.Config.Id);
            abilityComp.RemoveAbility(skill);
        }
    }
}
