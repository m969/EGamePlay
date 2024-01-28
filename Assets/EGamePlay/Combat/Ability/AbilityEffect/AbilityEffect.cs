using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace EGamePlay.Combat
{
    public class ApplyEffectEvent { public AbilityEffect AbilityEffect; }
    public enum EffectSourceType { Ability, Execution }

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
        }

        public ICombatObserver Observer;
        public Entity Source;
    }

    /// <summary>
    /// 能力效果，如伤害、治疗、施加状态等这些和技能数值、状态相关的效果
    /// </summary>
    public partial class AbilityEffect : Entity
    {
        public bool Enable { get; set; }
        public Entity OwnerAbility => Parent;
        public CombatEntity OwnerEntity => (OwnerAbility as IAbilityEntity).OwnerEntity;
        public CombatEntity ParentEntity => (OwnerAbility as IAbilityEntity).ParentEntity;
        public Effect EffectConfig { get; set; }
        public string ConditionParamValue { get; set; }
        public EffectSourceType EffectSourceType { get; set; }
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
            /// 施加状态效果
            if (this.EffectConfig is AddStatusEffect) AddComponent<EffectAddStatusComponent>();
            /// 自定义效果
            if (this.EffectConfig is CustomEffect) AddComponent<EffectCustomComponent>();
            /// 效果修饰
            AddComponent<EffectDecoratosComponent>();
        }

        public override void OnDestroy()
        {
            DisableEffect();
        }

        public void EnableEffect()
        {
            //Log.Debug("EnableEffect");
            Enable = true;
            foreach (var item in Components.Values)
            {
                item.Enable = true;
            }

            //var triggable = !(this.EffectConfig is ActionControlEffect) && !(this.EffectConfig is AttributeModifyEffect);
            //if (triggable)
            {
                if (EffectConfig.EffectTriggerType == EffectTriggerType.None || EffectConfig.EffectTriggerType == EffectTriggerType.Instant)
                {
                    TriggerObserver = AddChild<TriggerObserver>();
                }

                if (EffectConfig.EffectTriggerType == EffectTriggerType.Action)
                {
                    //Log.Debug("EnableEffect ActionPointObserver");
                    AddChild<ActionPointObserver>();
                }
                if (EffectConfig.EffectTriggerType == EffectTriggerType.Condition)
                {
                    var conditionType = EffectConfig.ConditionType;
                    var paramObj = ConditionParamValue;
                    if (conditionType == ConditionEventType.WhenInTimeNoDamage && float.TryParse((string)paramObj, out var time))
                    {
                        var condition = AddChild<ConditionWhenInTimeNoDamageObserver>(time);
                        condition.StartListen(null);
                    }
                    if (conditionType == ConditionEventType.WhenIntervalTime && float.TryParse((string)paramObj, out var intervalTime))
                    {
                        var condition = AddChild<ConditionTimeIntervalObserver>(intervalTime);
                        condition.StartListen(null);
                    }
                }

                if (EffectConfig.StateCheckList != null && EffectConfig.StateCheckList.Count > 0)
                {
                    foreach (var item in Children)
                    {
                        if (item is ICombatObserver)
                        {
                            item.AddComponent<EffectTargetStateCheckComponent>();
                        }
                    }
                    //if (EffectConfig.EffectTriggerType == EffectTriggerType.None || EffectConfig.EffectTriggerType == EffectTriggerType.Instant)
                    //{
                    //    EffectTrigger.AddComponent<EffectTargetStateCheckComponent>();
                    //}
                }

                /// 立即触发
                if (EffectConfig.EffectTriggerType == EffectTriggerType.Instant)
                {
                    //TriggerEffectToParent();
                    TriggerObserver.OnTrigger(ParentEntity);
                }
            }

            //TriggerEventBind?.EnableTriggerBind();
        }

        public void DisableEffect()
        {
            Enable = false;
            foreach (var item in Components.Values)
            {
                item.Enable = false;
            }
        }

        public EffectAssignAction CreateAssignAction(Entity targetEntity)
        {
            //Log.Debug($"TryAssignAllEffectsToTargetWithExecution {targetEntity} {AbilityEffects.Count}");
            if (OwnerEntity.EffectAssignAbility.TryMakeAction(out var action))
            {
                //Log.Debug($"AbilityEffect TryAssignEffectTo {targetEntity} {EffectConfig}");
                action.AssignTarget = targetEntity;
                action.SourceAbility = OwnerAbility;
                action.AbilityEffect = this;
            }
            return action;
        }

        public void TriggerEffectToParent()
        {
            TriggerEffectCheckWithTarget(ParentEntity);
        }

        /// <summary>
        /// 触发技能效果
        /// </summary>
        public void TriggerEffectCheck(SkillExecution skillExecution)
        {
            TriggerEffectCheckWithTarget(skillExecution);
        }

        /// <summary>
        /// 触发对目标施加效果
        /// </summary>
        public void TriggerEffectCheckWithTarget(Entity target)
        {
            var effectAssign = CreateAssignAction(target);
            effectAssign.AssignEffect();
        }

        public void OnObserverTrigger(TriggerContext context)
        {
            //Log.Debug("AbilityEffect OnObserverTrigger");
            var observer = context.Observer;
            var source = context.Source;
            var target = source;

            var conditionCheckResult = true;
            if (observer != null)
            {
                if (!(observer is ActionPointObserver))
                {
                    target = ParentEntity;
                }
                /// 这里是状态条件判断，状态条件判断是判断目标的状态是否满足条件，满足则触发效果
                if ((observer as Entity).TryGet(out EffectTargetStateCheckComponent component))
                {
                    conditionCheckResult = component.CheckTargetState(target);
                }
            }

            /// 条件满足则触发效果
            if (conditionCheckResult)
            {
                TriggerEffectCheckWithTarget(target);
            }
        }
    }
}
