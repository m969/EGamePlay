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


        ///// <summary>
        ///// 挂载能力，技能、被动、buff等都通过这个接口挂载
        ///// </summary>
        //public Ability AttachAbility(object configObject)
        //{
        //    var ability = Entity.AddChild<Ability>(x=>x.ConfigObject = configObject as AbilityConfigObject);
        //    ability.AddComponent<AbilityLevelComponent>();
        //    IdAbilities.Add(ability.Id, ability);
        //    return ability;
        //}

        //public void RemoveAbility(Ability ability)
        //{
        //    IdAbilities.Remove(ability.Id);
        //    ability.EndAbility();
        //}
    }
}
