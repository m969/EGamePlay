using ECS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class EffectDecoratorComponent : EcsComponent
    {
        //public override void Awake()
        //{
        //    if (GetEntity<AbilityEffect>().EffectConfig.Decorators != null)
        //    {
        //        foreach (var effectDecorator in GetEntity<AbilityEffect>().EffectConfig.Decorators)
        //        {
        //            if (effectDecorator is DamageReduceWithTargetCountDecorator reduceWithTargetCountDecorator)
        //            {
        //                Entity.AddComponent<EffectDamageReduceWithTargetCountComponent>();
        //            }
        //        }
        //    }
        //}
    }
}