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

        public static void Update(Ability entity, AbilityDurationComponent component)
        {
            if (component.LifeTimer.IsRunning)
            {
                component.LifeTimer.UpdateAsFinish(Time.deltaTime, () => OnTimeEnd(entity));
            }
        }

        public static void OnTimeEnd(Ability ability)
        {
            var config = ability.Config;
            if (config.Type == "Skill")
            {
                SkillSystem.Remove(ability.ParentEntity as CombatEntity, ability);
            }
            if (config.Type == "Buff")
            {
                BuffSystem.Remove(ability.ParentEntity as CombatEntity, ability);
            }
        }
    }
}