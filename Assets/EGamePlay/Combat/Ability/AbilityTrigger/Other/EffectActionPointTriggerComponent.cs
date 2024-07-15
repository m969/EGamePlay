using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 行动点触发组件
    /// </summary>
    public class EffectActionPointTriggerComponent : Component
    {
        public override bool DefaultEnable { get; set; } = false;


        //public override void OnEnable()
        //{
        //    var abilityEffect = Entity.GetParent<AbilityEffect>();
        //    var actionPointType = abilityEffect.EffectConfig.ActionPointType;
        //    var combatEntity = abilityEffect.Parent.As<IAbilityEntity>().OwnerEntity;
        //    combatEntity.ListenActionPoint(actionPointType, OnActionPointTrigger);
        //}

        //public override void OnDisable()
        //{
        //    var abilityEffect = Entity.GetParent<AbilityEffect>();
        //    var actionPointType = abilityEffect.EffectConfig.ActionPointType;
        //    var combatEntity = abilityEffect.Parent.As<IAbilityEntity>().OwnerEntity;
        //    combatEntity.UnListenActionPoint(actionPointType, OnActionPointTrigger);
        //}

        ///// <summary>
        ///// 通过行动点触发效果
        ///// </summary>
        //private void OnActionPointTrigger(Entity combatAction)
        //{
        //    GetEntity<EffectTriggerEventBind>().TriggerEffectCheckWithTarget(combatAction);
        //}
    }
}