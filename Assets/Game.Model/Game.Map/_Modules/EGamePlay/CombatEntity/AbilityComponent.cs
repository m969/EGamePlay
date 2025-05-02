using ECS;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class AbilityComponent : EcsComponent
    {
        public Dictionary<long, Ability> IdAbilities { get; set; } = new Dictionary<long, Ability>();
    }
}
