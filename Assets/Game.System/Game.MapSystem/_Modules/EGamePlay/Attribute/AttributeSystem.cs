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
    public interface IOnAttributeUpdate
    {
        void OnAttributeUpdate(EcsEntity entity, FloatNumeric numeric);
    }

    public class AttributeSystem : AComponentSystem<CombatEntity, AttributeComponent>,
        IAwake<CombatEntity, AttributeComponent>
    {
        public void Awake(CombatEntity entity, AttributeComponent component)
        {

        }

        public static void InitializeCharacter(CombatEntity entity)
        {
            AddNumeric(entity, AttributeType.HealthPointMax, 99_999);
            AddNumeric(entity, AttributeType.HealthPoint, 99_999);
            AddNumeric(entity, AttributeType.MoveSpeed, 1);
            AddNumeric(entity, AttributeType.Attack, 1000);
            AddNumeric(entity, AttributeType.Defense, 300);
            AddNumeric(entity, AttributeType.CriticalProbability, 0.5f);
            AddNumeric(entity, AttributeType.CauseDamage, 1);
        }

        public static void InitializeAbilityItem(EcsEntity entity)
        {
            AddNumeric(entity, AttributeType.HealthPointMax, 9000);
            AddNumeric(entity, AttributeType.HealthPoint, 9000);
            AddNumeric(entity, AttributeType.Defense, 300);
        }

        public static FloatNumeric AddNumeric(EcsEntity entity, AttributeType attributeType, float baseValue)
        {
            var component = entity.GetComponent<AttributeComponent>();
            var numeric = entity.AddChild<FloatNumeric>();
            numeric.Name = attributeType.ToString();
            numeric.AttributeType = attributeType;
            NumericSystem.SetBase(numeric, baseValue);
            component.attributeNameNumerics.Add(attributeType.ToString(), numeric);
            return numeric;
        }

        public static FloatNumeric GetNumeric(EcsEntity entity, AttributeType attributeType)
        {
            var component = entity.GetComponent<AttributeComponent>();
            return component.attributeNameNumerics[attributeType.ToString()];
        }

        public static FloatNumeric GetNumeric(EcsEntity entity, string attributeName)
        {
            var component = entity.GetComponent<AttributeComponent>();
            return component.attributeNameNumerics[attributeName];
        }

        public static void OnUpdate(EcsEntity entity, FloatNumeric numeric)
        {
            var component = entity.GetComponent<AttributeComponent>();
            EventSystem.Dispatch<IOnAttributeUpdate>(entity, system => { system.OnAttributeUpdate(entity, numeric); });
        }
    }
}