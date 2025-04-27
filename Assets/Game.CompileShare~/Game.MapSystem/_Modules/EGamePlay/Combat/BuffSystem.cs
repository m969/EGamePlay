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
    public class BuffSystem : AComponentSystem<CombatEntity, BuffComponent>,
        IAwake<CombatEntity, BuffComponent>
    {
        public void Awake(CombatEntity entity, BuffComponent component)
        {

        }

        public static Ability AttachStatus(CombatEntity entity, object configObject)
        {
            var component = entity.GetComponent<BuffComponent>();
            var statusAbility = AbilitySystem.AttachAbility(entity, configObject);
            var statusConfigID = statusAbility.Config.KeyName;
            if (!component.TypeIdStatuses.ContainsKey(statusConfigID))
            {
                component.TypeIdStatuses.Add(statusConfigID, new List<Ability>());
            }
            component.TypeIdStatuses[statusConfigID].Add(statusAbility);
            component.Statuses.Add(statusAbility);
            return statusAbility;
        }

        public static void RemoveBuff(CombatEntity entity, Ability buff)
        {
            var component = entity.GetComponent<BuffComponent>();
            //component.Publish(new RemoveStatusEvent() { Entity = entity, Status = buff, StatusId = buff.Id });
            var keyName = buff.Config.KeyName;
            component.TypeIdStatuses[keyName].Remove(buff);
            if (component.TypeIdStatuses[keyName].Count == 0)
            {
                component.TypeIdStatuses.Remove(keyName);
            }
            component.Statuses.Remove(buff);
            AbilitySystem.RemoveAbility(entity, buff);
        }

        public static bool HasStatus(CombatEntity entity, string statusTypeId)
        {
            var component = entity.GetComponent<BuffComponent>();
            return component.TypeIdStatuses.ContainsKey(statusTypeId);
        }

        public static Ability GetStatus(CombatEntity entity, string statusTypeId)
        {
            var component = entity.GetComponent<BuffComponent>();
            return component.TypeIdStatuses[statusTypeId][0];
        }

        public static void OnStatusesChanged(CombatEntity entity, Ability statusAbility)
        {
            var parentEntity = statusAbility.ParentEntity;
            var component = entity.GetComponent<BuffComponent>();

            var tempActionControl = ActionControlType.None;
            foreach (var item in component.Statuses)
            {
                if (!item.Enable)
                {
                    continue;
                }
                foreach (var effect in item.AbilityEffects)
                {
                    if (effect.Enable && effect.GetComponent<EffectActionControlComponent>() is { } actionControlComponent)
                    {
                        tempActionControl = tempActionControl | actionControlComponent.ActionControlEffect.ActionControlType;
                    }
                }
            }

            if (parentEntity is CombatEntity combatEntity)
            {
                combatEntity.ActionControlType = tempActionControl;
                var moveForbid = combatEntity.ActionControlType.HasFlag(ActionControlType.MoveForbid);
                combatEntity.GetComponent<MotionComponent>().Enable = !moveForbid;
            }
        }
    }
}