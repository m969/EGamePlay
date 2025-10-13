using ECS;
using System.Collections.Generic;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class BuffComponent : EcsComponent
    {
        public List<Ability> Buffs { get; set; } = new List<Ability>();
        public Dictionary<string, List<Ability>> TypeIdBuffs { get; set; } = new Dictionary<string, List<Ability>>();
    }
}
