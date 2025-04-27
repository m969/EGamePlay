using ECS;
using ET;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class EffectAddBuffComponent : EcsComponent/*, IEffectTriggerSystem*/
    {
        public AddStatusEffect AddStatusEffect { get; set; }
        public float Duration { get; set; }
        public string NumericValueProperty { get; set; }


        //public override void Awake()
        //{
        //    AddStatusEffect = GetEntity<AbilityEffect>().EffectConfig as AddStatusEffect;
        //    Duration = AddStatusEffect.Duration;
        //}

        //public int GetNumericValue()
        //{
        //    return 1;
        //}

        //public void OnTriggerApplyEffect(Entity effectAssign)
        //{
        //    //Log.Debug($"EffectCureComponent OnAssignEffect");
        //    var effectAssignAction = effectAssign.As<EffectAssignAction>();
        //    if (GetEntity<AbilityEffect>().OwnerEntity.AddStatusAbility.TryMakeAction(out var action))
        //    {
        //        action.SourceAssignAction = effectAssignAction;
        //        action.Target = effectAssignAction.Target;
        //        action.SourceAbility = effectAssignAction.SourceAbility;
        //        action.ApplyAddStatus();
        //    }
        //}

        //public IActionExecute GetActionExecution()
        //{
        //    if (GetEntity<AbilityEffect>().OwnerEntity.AddStatusAbility.TryMakeAction(out var action))
        //    {
        //        return action;
        //    }
        //    return null;
        //}
    }
}