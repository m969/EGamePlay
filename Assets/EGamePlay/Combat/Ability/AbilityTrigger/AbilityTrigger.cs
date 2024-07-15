using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 能力触发器
    /// </summary>
    public class AbilityTrigger : Entity
    {
        public bool Enable { get; set; }
        public Entity OwnerAbility => Parent;
        public CombatEntity OwnerEntity => (OwnerAbility as IAbilityEntity).OwnerEntity;
        public Entity ParentEntity => (OwnerAbility as IAbilityEntity).ParentEntity;
        public TriggerConfig TriggerConfig { get; set; }


        public override void Awake(object initData)
        {
            this.TriggerConfig = initData as TriggerConfig;

            if (TriggerConfig.TriggerType == EffectTriggerType.ExecuteTrigger)
            {
                //TriggerObserver = AddChild<TriggerObserver>();
            }

            if (TriggerConfig.TriggerType == EffectTriggerType.AutoTrigger)
            {
                if (TriggerConfig.AutoTriggerType == EffectAutoTriggerType.Action)
                {
                    AddComponent<ActionPointObserveComponent>();
                }
                if (TriggerConfig.AutoTriggerType == EffectAutoTriggerType.Condition)
                {
                    var conditionType = TriggerConfig.ConditionType;
                    var paramObj = TriggerConfig.ConditionParam;
                    if (conditionType == TimeStateEventType.WhenInTimeNoDamage && float.TryParse((string)paramObj, out var time))
                    {
                        var condition = AddComponent<TimeState_WhenInTimeNoDamageObserveComponent>(time);
                        condition.StartListen(null);
                    }
                    if (conditionType == TimeStateEventType.WhenIntervalTime && float.TryParse((string)paramObj, out var intervalTime))
                    {
                        var condition = AddComponent<TimeState_TimeIntervalObserveComponent>(intervalTime);
                        condition.StartListen(null);
                    }
                }
            }

            if (TriggerConfig.StateCheckList != null && TriggerConfig.StateCheckList.Count > 0)
            {
                AddComponent<TriggerStateCheckComponent>();
                //foreach (var item in Children)
                //{
                //    if (item is ICombatObserver)
                //    {
                //        item.AddComponent<TriggerStateCheckComponent>();
                //    }
                //}
            }
        }

        //public void OnTrigger(Entity source)
        public void OnTrigger(TriggerContext context)
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
                foreach (var item in TriggerConfig.TriggerEffects)
                {
                    var abilityEffectComponent = OwnerAbility.GetComponent<AbilityEffectComponent>();
                    var effects = abilityEffectComponent.AbilityEffects;
                    for (int i = 0; i < effects.Count; i++)
                    {
                        if (i == (int)item.EffectApplyType - 1 || item.EffectApplyType == EffectApplyType.AllEffects)
                        {
                            var effect = effects[i];
                            if (OwnerEntity.EffectAssignAbility.TryMakeAction(out var effectAssign))
                            {
                                effectAssign.AbilityEffect = effect;
                                effectAssign.AssignTarget = target;
                                effectAssign.SourceAbility = OwnerAbility;
                                effectAssign.TriggerContext = context;
                                effectAssign.AssignEffect();
                            }
                        }
                    }

                }
            }
        }

        public void EnableTrigger()
        {
            Enable = true;

            /// 立即触发
            if (TriggerConfig.TriggerType == EffectTriggerType.AutoTrigger && TriggerConfig.AutoTriggerType == EffectAutoTriggerType.Instant)
            {
                OnTrigger(new TriggerContext() { Target = ParentEntity, Source = OwnerEntity });
            }
        }

        public void DisableTrigger()
        {
            Enable = false;
        }
    }
}