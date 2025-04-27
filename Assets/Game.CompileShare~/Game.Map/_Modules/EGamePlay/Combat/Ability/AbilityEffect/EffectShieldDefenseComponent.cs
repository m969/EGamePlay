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
    public class EffectShieldDefenseComponent : EcsComponent/*, IEffectTriggerSystem*/
    {
        public ShieldDefenseEffect ShieldDefenseEffect { get; set; }


        //public override void Awake()
        //{
        //    ShieldDefenseEffect = GetEntity<AbilityEffect>().EffectConfig as ShieldDefenseEffect;
        //}

        //public void OnTriggerApplyEffect(Entity effectAssign)
        //{
        //    var effectAssignAction = effectAssign.As<EffectAssignAction>();
        //    var target = effectAssignAction.Target;
        //    target.AddComponent<AbilityItemShieldComponent>();
        //}
    }
}