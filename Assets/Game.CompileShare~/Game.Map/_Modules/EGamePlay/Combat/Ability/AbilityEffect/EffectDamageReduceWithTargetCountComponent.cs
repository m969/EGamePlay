using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameUtils;
using ECS;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class EffectDamageReduceWithTargetCountComponent : EcsComponent
    {
        public float ReducePercent { get; set; }
        public float MinPercent { get; set; }


        //public override void Awake()
        //{
        //    var damageEffect = (Entity as AbilityEffect).EffectConfig as DamageEffect;
        //    foreach (var effectDecorator in damageEffect.Decorators)
        //    {
        //        if (effectDecorator is DamageReduceWithTargetCountDecorator reduceWithTargetCountDecorator)
        //        {
        //            ReducePercent = reduceWithTargetCountDecorator.ReducePercent / 100;
        //            MinPercent = reduceWithTargetCountDecorator.MinPercent / 100;
        //        }
        //    }
        //}

        //public float GetDamagePercent(int TargetCounter)
        //{
        //    return System.Math.Max(MinPercent, 1 - ReducePercent * TargetCounter);
        //}
    }
}