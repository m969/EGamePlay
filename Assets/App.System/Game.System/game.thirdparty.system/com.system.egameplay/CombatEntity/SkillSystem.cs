using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System;
using ET;
using System.ComponentModel;
using ECSGame;

namespace EGamePlay
{
    public class SkillSystem : AComponentSystem<CombatEntity, SkillComponent>,
        IAwake<CombatEntity, SkillComponent>
    {
        public void Awake(CombatEntity entity, SkillComponent component)
        {

        }

        public static Ability Attach(CombatEntity entity, object configObject)
        {
            var component = entity.GetComponent<SkillComponent>();
            var abilityComp = entity.GetComponent<AbilityComponent>();
            var skill = AbilitySystem.Attach(entity, configObject);
            component.NameSkills.Add(skill.Config.Name, skill);
            component.IdSkills.Add(skill.Config.Id, skill);
            return skill;
        }

        public static void Remove(CombatEntity entity, Ability skill)
        {
            var component = entity.GetComponent<SkillComponent>();
            var abilityComp = entity.GetComponent<AbilityComponent>();
            component.NameSkills.Remove(skill.Config.Name);
            component.IdSkills.Remove(skill.Config.Id);
            AbilitySystem.Remove(entity, skill);
        }
    }
}