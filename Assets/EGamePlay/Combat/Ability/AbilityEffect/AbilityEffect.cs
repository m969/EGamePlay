using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public interface IEffectTriggerSystem
    {
        void OnTriggerApplyEffect(Entity effectAssign);
    }

    public struct TriggerContext
    {
        public TriggerContext(ICombatObserver observer, Entity source)
        {
            Observer = observer;
            Source = source;
            AbilityItem = null;
            Target = null;
        }

        public ICombatObserver Observer;
        public Entity Source;
        public AbilityItem AbilityItem;
        public Entity Target;
    }

    /// <summary>
    /// 能力效果，如伤害、治疗、施加状态等这些和技能数值、状态相关的效果
    /// </summary>
    public partial class AbilityEffect : Entity
    {
        public bool Enable { get; set; }
        public Entity OwnerAbility => Parent;
        public CombatEntity OwnerEntity => (OwnerAbility as IAbilityEntity).OwnerEntity;
        public Entity ParentEntity => (OwnerAbility as IAbilityEntity).ParentEntity;
        public Effect EffectConfig { get; set; }
        public string ConditionParamValue { get; set; }
        public TriggerObserver TriggerObserver { get; set; }


        public override void Awake(object initData)
        {
            this.EffectConfig = initData as Effect;
            Name = EffectConfig.GetType().Name;
            //Log.Debug($"AbilityEffect Awake {OwnerAbility.Name} {EffectConfig}");

            /// 行动禁制
            if (this.EffectConfig is ActionControlEffect) AddComponent<EffectActionControlComponent>();
            /// 属性修饰
            if (this.EffectConfig is AttributeModifyEffect) AddComponent<EffectAttributeModifyComponent>();

            /// 伤害效果
            if (this.EffectConfig is DamageEffect) AddComponent<EffectDamageComponent>();
            /// 治疗效果
            if (this.EffectConfig is CureEffect) AddComponent<EffectCureComponent>();
            /// 光盾防御效果
            if (this.EffectConfig is ShieldDefenseEffect) AddComponent<EffectShieldDefenseComponent>();
            /// 施加状态效果
            if (this.EffectConfig is AddStatusEffect) AddComponent<EffectAddStatusComponent>();
            /// 自定义效果
            if (this.EffectConfig is CustomEffect) AddComponent<EffectCustomComponent>();
            /// 效果修饰
            var decorators = this.EffectConfig.Decorators;
            if (decorators != null && decorators.Count > 0) AddComponent<EffectDecoratosComponent>();
        }

        public override void OnDestroy()
        {
            DisableEffect();
        }

        public void EnableEffect()
        {
            Enable = true;
            //if (EffectConfig.EffectTriggerType != EffectAutoTriggerType.None)
            //{
            //    foreach (var item in Components.Values)
            //    {
            //        item.Enable = true;
            //    }
            //}

            //if (EffectConfig.EffectTriggerType == EffectAutoTriggerType.None || EffectConfig.EffectTriggerType == EffectAutoTriggerType.Instant)
            //{
            //    TriggerObserver = AddChild<TriggerObserver>();
            //}

            //if (EffectConfig.EffectTriggerType == EffectAutoTriggerType.Action)
            //{
            //    //Log.Debug("EnableEffect ActionPointObserver");
            //    AddChild<ActionPointObserveComponent>();
            //}
            //if (EffectConfig.EffectTriggerType == EffectAutoTriggerType.Condition)
            //{
            //    var conditionType = EffectConfig.ConditionType;
            //    var paramObj = ConditionParamValue;
            //    if (conditionType == TimeStateEventType.WhenInTimeNoDamage && float.TryParse((string)paramObj, out var time))
            //    {
            //        var condition = AddChild<TimeState_WhenInTimeNoDamageObserveComponent>(time);
            //        condition.StartListen(null);
            //    }
            //    if (conditionType == TimeStateEventType.WhenIntervalTime && float.TryParse((string)paramObj, out var intervalTime))
            //    {
            //        var condition = AddChild<TimeState_TimeIntervalObserveComponent>(intervalTime);
            //        condition.StartListen(null);
            //    }
            //}

            //if (EffectConfig.StateCheckList != null && EffectConfig.StateCheckList.Count > 0)
            //{
            //    foreach (var item in Children)
            //    {
            //        if (item is ICombatObserver)
            //        {
            //            item.AddComponent<TriggerStateCheckComponent>();
            //        }
            //    }
            //}

            ///// 立即触发
            //if (EffectConfig.EffectTriggerType == EffectAutoTriggerType.Instant)
            //{
            //    //TriggerEffectToParent();
            //    TriggerObserver.OnTrigger(ParentEntity);
            //}
        }

        public void DisableEffect()
        {
            Enable = false;
            //foreach (var item in Components.Values)
            //{
            //    item.Enable = false;
            //}
        }

        public void OnObserverTrigger(TriggerContext context)
        {
            //Log.Debug("AbilityEffect OnObserverTrigger");
            var observer = context.Observer;
            var source = context.Source;
            Entity target = context.Target;
            if (target == null)
            {
                target = source;
            }

            var stateCheckResult = true;
            if (!(observer is ActionPointObserveComponent) && !(observer is TriggerObserver))
            {
                target = ParentEntity;
            }

            /// 这里是状态判断，状态判断是判断目标的状态是否满足条件，满足才能触发效果
            if ((observer as Entity).TryGet(out TriggerStateCheckComponent component))
            {
                stateCheckResult = component.CheckTargetState(target);
            }

            /// 条件满足则触发效果
            if (stateCheckResult)
            {
                if (OwnerEntity.EffectAssignAbility.TryMakeAction(out var effectAssign))
                {
                    effectAssign.AssignTarget = target;
                    effectAssign.SourceAbility = OwnerAbility;
                    effectAssign.AbilityEffect = this;
                    effectAssign.TriggerContext = context;
                    effectAssign.AssignEffect();
                }
            }
        }
    }
}
