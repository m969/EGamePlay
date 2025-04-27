using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System;
using ET;

namespace EGamePlay
{
    public class AbilityDurationSystem : AComponentSystem<Ability, AbilityDurationComponent>,
        IAwake<Ability, AbilityDurationComponent>
    {
        public void Awake(Ability entity, AbilityDurationComponent component)
        {
            var lifeTime = component.Duration;
            component.LifeTimer = new GameTimer(lifeTime);
        }

        public void Update(Ability entity, AbilityDurationComponent component)
        {
            if (component.LifeTimer.IsRunning)
            {
                component.LifeTimer.UpdateAsFinish(Time.deltaTime, () => OnLifeTimeFinish(entity));
            }
        }

        public static void OnLifeTimeFinish(Ability ability)
        {
            var config = ability.Config;
            if (config.Type == "Skill")
            {
                SkillSystem.RemoveSkill(ability.ParentEntity as CombatEntity, ability);
            }
            if (config.Type == "Buff")
            {
                BuffSystem.RemoveBuff(ability.ParentEntity as CombatEntity, ability);
            }
        }
    }
}