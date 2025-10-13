using ECS;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class SkillComponent : EcsComponent
	{
        public Dictionary<string, Ability> NameSkills { get; set; } = new Dictionary<string, Ability>();
        public Dictionary<int, Ability> IdSkills { get; set; } = new Dictionary<int, Ability>();
        public Dictionary<KeyCode, Ability> InputSkills { get; set; } = new Dictionary<KeyCode, Ability>();
    }
}
