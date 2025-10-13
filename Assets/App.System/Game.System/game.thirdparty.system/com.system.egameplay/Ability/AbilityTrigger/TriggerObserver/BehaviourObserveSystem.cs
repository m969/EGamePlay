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
    public class BehaviourObserveSystem : AEntitySystem<AbilityTrigger>,
        IAwake<AbilityTrigger>,
        IDestroy<AbilityTrigger>
    {
        public void Awake(AbilityTrigger entity)
        {
            var combatEntity = entity.OwnerAbility.ParentEntity as CombatEntity;
            BehaviourPointSystem.AddObserver(combatEntity, entity.TriggerConfig.ActionPointType, entity);
        }

        public void Destroy(AbilityTrigger entity)
        {
            var combatEntity = entity.OwnerAbility.ParentEntity as CombatEntity;
            BehaviourPointSystem.RemoveObserver(combatEntity, entity.TriggerConfig.ActionPointType, entity);
        }

        public static void OnTrigger(AbilityTrigger entity, EcsEntity source)
        {
            AbilityTriggerSystem.OnTrigger(entity, new TriggerContext() { TriggerSource = source });
        }
    }
}