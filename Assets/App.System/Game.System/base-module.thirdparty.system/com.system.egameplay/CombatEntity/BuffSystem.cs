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

        public static Ability Attach(CombatEntity entity, object configObject)
        {
            var component = entity.GetComponent<BuffComponent>();
            var statusAbility = AbilitySystem.Attach(entity, configObject);
            var statusConfigID = statusAbility.Config.KeyName;
            if (!component.TypeIdBuffs.ContainsKey(statusConfigID))
            {
                component.TypeIdBuffs.Add(statusConfigID, new List<Ability>());
            }
            component.TypeIdBuffs[statusConfigID].Add(statusAbility);
            component.Buffs.Add(statusAbility);
            return statusAbility;
        }

        public static void Remove(CombatEntity entity, Ability buff)
        {
            var component = entity.GetComponent<BuffComponent>();
            //component.Publish(new RemoveStatusEvent() { Entity = entity, Status = buff, StatusId = buff.Id });
            var keyName = buff.Config.KeyName;
            component.TypeIdBuffs[keyName].Remove(buff);
            if (component.TypeIdBuffs[keyName].Count == 0)
            {
                component.TypeIdBuffs.Remove(keyName);
            }
            component.Buffs.Remove(buff);
            AbilitySystem.Remove(entity, buff);
        }

        public static bool HasBuff(CombatEntity entity, string statusTypeId)
        {
            var component = entity.GetComponent<BuffComponent>();
            return component.TypeIdBuffs.ContainsKey(statusTypeId);
        }

        public static Ability GetBuff(CombatEntity entity, string statusTypeId)
        {
            var component = entity.GetComponent<BuffComponent>();
            return component.TypeIdBuffs[statusTypeId][0];
        }

        public static void OnBuffChanged(CombatEntity entity, Ability statusAbility)
        {
            var parentEntity = statusAbility.ParentEntity;
            var component = entity.GetComponent<BuffComponent>();

            var tempActionControl = ActionControlType.None;
            foreach (var item in component.Buffs)
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
                combatEntity.Actor.GetComponent<MotionComponent>().Enable = !moveForbid;
            }
        }
    }
}