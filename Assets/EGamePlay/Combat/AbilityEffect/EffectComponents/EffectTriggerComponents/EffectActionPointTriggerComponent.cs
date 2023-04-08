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


        public override void OnEnable()
        {
            var actionPointType = Entity.GetParent<AbilityEffect>().EffectConfig.ActionPointType;
            Entity.GetParent<AbilityEffect>().Parent.As<IAbilityEntity>().OwnerEntity.ListenActionPoint(actionPointType, OnActionPointTrigger);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            var actionPointType = Entity.GetParent<AbilityEffect>().EffectConfig.ActionPointType;
            Entity.GetParent<AbilityEffect>().Parent.As<IAbilityEntity>().OwnerEntity.UnListenActionPoint(actionPointType, OnActionPointTrigger);
        }

        private void OnActionPointTrigger(Entity combatAction)
        {
            GetEntity<EffectTriggerEventBind>().TriggerEffectCheckWithTarget(combatAction);
        }
    }
}