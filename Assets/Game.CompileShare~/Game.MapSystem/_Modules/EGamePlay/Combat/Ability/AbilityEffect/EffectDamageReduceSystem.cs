using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;

namespace EGamePlay
{
    public class EffectDamageReduceSystem : AComponentSystem<AbilityEffect, EffectDamageReduceWithTargetCountComponent>,
        IAwake<AbilityEffect, EffectDamageReduceWithTargetCountComponent>
    {
        public void Awake(AbilityEffect entity, EffectDamageReduceWithTargetCountComponent component)
        {
            var damageEffect = entity.EffectConfig as DamageEffect;
            foreach (var effectDecorator in damageEffect.Decorators)
            {
                if (effectDecorator is DamageReduceWithTargetCountDecorator reduceWithTargetCountDecorator)
                {
                    component.ReducePercent = reduceWithTargetCountDecorator.ReducePercent / 100;
                    component.MinPercent = reduceWithTargetCountDecorator.MinPercent / 100;
                }
            }
        }

        public static float GetDamagePercent(AbilityEffect entity, int TargetCounter)
        {
            var component = entity.GetComponent<EffectDamageReduceWithTargetCountComponent>();
            return System.Math.Max(component.MinPercent, 1 - component.ReducePercent * TargetCounter);
        }
    }
}