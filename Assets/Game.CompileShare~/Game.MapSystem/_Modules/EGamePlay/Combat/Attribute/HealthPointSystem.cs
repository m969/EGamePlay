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
    public class HealthPointSystem : AComponentSystem<EcsEntity, HealthPointComponent>,
        IAwake<EcsEntity, HealthPointComponent>
    {
        public void Awake(EcsEntity entity, HealthPointComponent component)
        {

        }

        public static void Reset(EcsEntity entity)
        {
            var component = entity.GetComponent<HealthPointComponent>();
            component.HealthPointNumeric = entity.GetComponent<AttributeComponent>().HealthPoint;
            component.HealthPointMaxNumeric = entity.GetComponent<AttributeComponent>().HealthPointMax;
            NumericSystem.SetBase(component.HealthPointNumeric, component.HealthPointMaxNumeric.Value);
            //component.HealthPointNumeric.SetBase(HealthPointMaxNumeric.Value);
        }

        public static void SetMaxValue(EcsEntity entity, int value)
        {
            var component = entity.GetComponent<HealthPointComponent>();
            NumericSystem.SetBase(component.HealthPointMaxNumeric, value);
            //HealthPointMaxNumeric.SetBase(value);
        }

        public static void Minus(EcsEntity entity, int value)
        {
            var component = entity.GetComponent<HealthPointComponent>();
            NumericSystem.MinusBase(component.HealthPointNumeric, value);
            //HealthPointNumeric.MinusBase(value);
        }

        public static void Add(EcsEntity entity, int value)
        {
            var component = entity.GetComponent<HealthPointComponent>();
            if (value + component.Value > component.MaxValue)
            {
                NumericSystem.SetBase(component.HealthPointNumeric, component.MaxValue);
                //HealthPointNumeric.SetBase(component.MaxValue);
                return;
            }
            NumericSystem.SetBase(component.HealthPointNumeric, value);
            //HealthPointNumeric.AddBase(value);
        }

        public static void SetDie(EcsEntity entity)
        {
            var component = entity.GetComponent<HealthPointComponent>();
            NumericSystem.MinusBase(component.HealthPointNumeric, component.Value);
            //HealthPointNumeric.MinusBase(component.Value);
        }

        public static float ToPercent(EcsEntity entity)
        {
            var component = entity.GetComponent<HealthPointComponent>();
            return (float)component.Value / component.MaxValue;
        }

        public static int GetPercentHealth(EcsEntity entity, float pct)
        {
            var component = entity.GetComponent<HealthPointComponent>();
            return (int)(component.MaxValue * pct);
        }

        public static bool IsFull(EcsEntity entity)
        {
            var component = entity.GetComponent<HealthPointComponent>();
            return component.Value == component.MaxValue;
        }

        public static void ReceiveDamage(EcsEntity entity, IActionExecute combatAction)
        {
            var component = entity.GetComponent<HealthPointComponent>();
            var damageAction = combatAction as DamageAction;
            Minus(entity, damageAction.DamageValue);
        }

        public static void ReceiveCure(EcsEntity entity, IActionExecute combatAction)
        {
            var component = entity.GetComponent<HealthPointComponent>();
            var cureAction = combatAction as CureAction;
            Add(entity, cureAction.CureValue);
        }

        public static bool CheckDead(EcsEntity entity)
        {
            var component = entity.GetComponent<HealthPointComponent>();
            return component.Value <= 0;
        }
    }
}