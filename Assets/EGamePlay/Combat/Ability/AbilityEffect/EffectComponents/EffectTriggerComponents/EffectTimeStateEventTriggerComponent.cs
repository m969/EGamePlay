using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 计时状态事件触发组件，这个组件实现当自身对应的状态满足条件时，触发对应效果
    /// 触发事件分两种，第一种是按行为动作来触发，第二种是随着计时累积状态满足条件触发
    /// 第一种可以通过监听行动点触发，即ActionPoint&
    /// 第二种只能通过每帧检测来触发，即Update&
    /// </summary>
    public class EffectTimeStateEventTriggerComponent : Component
    {
        public override bool DefaultEnable { get; set; } = false;
        public string ConditionParamValue { get; set; }


        //public override void OnEnable()
        //{
        //    var conditionType = Entity.GetParent<AbilityEffect>().EffectConfig.ConditionType;
        //    var combatEntity = Entity.GetParent<AbilityEffect>().Parent.As<IAbilityEntity>().ParentEntity;
        //    combatEntity.ListenCondition(conditionType, OnConditionTrigger, ConditionParamValue);
        //}

        //public override void OnDisable()
        //{
        //    var conditionType = Entity.GetParent<AbilityEffect>().EffectConfig.ConditionType;
        //    var combatEntity = Entity.GetParent<AbilityEffect>().Parent.As<IAbilityEntity>().ParentEntity;
        //    combatEntity.UnListenCondition(conditionType, OnConditionTrigger);
        //}

        ///// <summary>
        ///// 条件事件触发效果
        ///// </summary>
        //private void OnConditionTrigger()
        //{
        //    GetEntity<EffectTriggerEventBind>().TriggerEffectToParent();
        //}
    }
}