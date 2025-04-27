using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;

namespace EGamePlay
{
    public class EffectDecoratorSystem : AComponentSystem<AbilityEffect, EffectDecoratorComponent>,
        IAwake<AbilityEffect, EffectDecoratorComponent>
    {
        public void Awake(AbilityEffect entity, EffectDecoratorComponent component)
        {
            if (entity.EffectConfig.Decorators != null)
            {
                foreach (var effectDecorator in entity.EffectConfig.Decorators)
                {
                    if (effectDecorator is DamageReduceWithTargetCountDecorator reduceWithTargetCountDecorator)
                    {
                        entity.AddComponent<EffectDamageReduceWithTargetCountComponent>();
                    }
                }
            }
        }
    }
}