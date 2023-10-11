using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameUtils;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class EffectDamageReduceWithTargetCountComponent : Component
    {
        //public int TargetCounter { get; set; }
        public float ReducePercent { get; set; }
        public float MinPercent { get; set; }


        public override void Awake()
        {
            var damageEffect = (Entity as AbilityEffect).EffectConfig as DamageEffect;
            //ReducePercent = float.Parse(customEffect.Params["递减百分比"]);
            //MinPercent = float.Parse(customEffect.Params["伤害下限限制"]);
            foreach (var effectDecorator in damageEffect.Decorators)
            {
                if (effectDecorator is DamageReduceWithTargetCountDecorator reduceWithTargetCountDecorator)
                {
                    ReducePercent = reduceWithTargetCountDecorator.ReducePercent / 100;
                    MinPercent = reduceWithTargetCountDecorator.MinPercent / 100;
                    //Log.Debug($"{ReducePercent} {MinPercent}");
                }
            }
        }

        //public void AddOneTarget()
        //{
        //    TargetCounter++;
        //}

        public float GetDamagePercent(int TargetCounter)
        {
            return System.Math.Max(MinPercent, 1 - ReducePercent * TargetCounter);
        }
    }
}