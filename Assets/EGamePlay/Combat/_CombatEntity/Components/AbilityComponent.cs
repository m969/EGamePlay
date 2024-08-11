using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class AbilityComponent : Component
    {
        //public Dictionary<string, Ability> NameSkills { get; set; } = new Dictionary<string, Ability>();
        public Dictionary<long, Ability> IdAbilities { get; set; } = new Dictionary<long, Ability>();
        //public Dictionary<KeyCode, Ability> InputSkills { get; set; } = new Dictionary<KeyCode, Ability>();


        /// <summary>
        /// 挂载能力，技能、被动、buff等都通过这个接口挂载
        /// </summary>
        public Ability AttachAbility(object configObject)
        {
            var ability = Entity.AddChild<Ability>(configObject);
            ability.AddComponent<AbilityLevelComponent>();
            IdAbilities.Add(ability.Id, ability);
            return ability;
        }

        public void RemoveAbility(Ability ability)
        {
            IdAbilities.Remove(ability.Id);
            ability.EndAbility();
        }
    }
}
