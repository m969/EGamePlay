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
    public class NumericSystem : AEntitySystem<FloatNumeric>,
        IAwake<FloatNumeric>
    {
        public void Awake(FloatNumeric entity)
        {
            entity.baseValue = entity.add = entity.pctAdd = entity.finalAdd = entity.finalPctAdd = 0f;
            entity.TypeModifierCollections.Add(((int)ModifierType.Add), new FloatModifierCollection());
            entity.TypeModifierCollections.Add(((int)ModifierType.PctAdd), new FloatModifierCollection());
            entity.TypeModifierCollections.Add(((int)ModifierType.FinalAdd), new FloatModifierCollection());
            entity.TypeModifierCollections.Add(((int)ModifierType.FinalPctAdd), new FloatModifierCollection());
        }

        public static float SetBase(FloatNumeric entity, float value)
        {
            entity.baseValue = value;
            Update(entity);
            return entity.baseValue;
        }

        public static float AddBase(FloatNumeric entity, float value)
        {
            entity.baseValue += value;
            Update(entity);
            return entity.baseValue;
        }

        public static float MinusBase(FloatNumeric entity, float value)
        {
            entity.baseValue -= value;
            if (entity.baseValue < 0) entity.baseValue = 0;
            Update(entity);
            return entity.baseValue;
        }

        public static void AddModifier(FloatNumeric entity, ModifierType modifierType, FloatModifier modifier)
        {
            var value = entity.TypeModifierCollections[((int)modifierType)].AddModifier(modifier);
            if (modifierType == ModifierType.Add) entity.add = value;
            if (modifierType == ModifierType.PctAdd) entity.pctAdd = value;
            if (modifierType == ModifierType.FinalAdd) entity.finalAdd = value;
            if (modifierType == ModifierType.FinalPctAdd) entity.finalPctAdd = value;
            Update(entity);
        }

        public static void RemoveModifier(FloatNumeric entity, ModifierType modifierType, FloatModifier modifier)
        {
            var value = entity.TypeModifierCollections[((int)modifierType)].RemoveModifier(modifier);
            if (modifierType == ModifierType.Add) entity.add = value;
            if (modifierType == ModifierType.PctAdd) entity.pctAdd = value;
            if (modifierType == ModifierType.FinalAdd) entity.finalAdd = value;
            if (modifierType == ModifierType.FinalPctAdd) entity.finalPctAdd = value;
            Update(entity);
        }

        public static void Update(FloatNumeric entity)
        {
            var value1 = entity.baseValue;
            var value2 = (value1 + entity.add) * (100 + entity.pctAdd) / 100f;
            var value3 = (value2 + entity.finalAdd) * (100 + entity.finalPctAdd) / 100f;
            entity.Value = value3;
            AttributeSystem.OnUpdate(entity.Parent, entity);
        }
    }
}