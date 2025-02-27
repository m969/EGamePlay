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
        public Ability OwnerAbility => GetParent<Ability>();
        public CombatEntity OwnerEntity => OwnerAbility.OwnerEntity;
        public Entity ParentEntity => OwnerAbility.ParentEntity;
        public TriggerConfig TriggerConfig { get; set; }
        public string ConditionParamValue { get; set; }


        public override void Awake(object initData)
        {
            this.TriggerConfig = initData as TriggerConfig;

            if (TriggerConfig.StateCheckList != null && TriggerConfig.StateCheckList.Count > 0)
            {
                AddComponent<TriggerStateCheckComponent>();
            }
        }

        public void EnableTrigger()
        {
            Enable = true;

            /// 被动触发
            if (TriggerConfig.TriggerType == EffectTriggerType.AutoTrigger)
            {
                /// 能力激活时触发
                if (TriggerConfig.AutoTriggerType == EffectAutoTriggerType.Instant)
                {
                    OnTrigger(new TriggerContext() { Target = ParentEntity });
                }
                /// 按行动点事件触发
                if (TriggerConfig.AutoTriggerType == EffectAutoTriggerType.Action)
                {
                    AddComponent<BehaviourObserveComponent>();
                }
                /// 按计时状态事件触发
                if (TriggerConfig.AutoTriggerType == EffectAutoTriggerType.Condition)
                {
                    var conditionType = TriggerConfig.ConditionType;
                    var paramObj = ConditionParamValue;
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
        }

        public void DisableTrigger()
        {
            Enable = false;
        }

        public void OnTrigger(TriggerContext context)
        {
            //Log.Debug($"{GetParent<Ability>().Config.KeyName} AbilityTrigger OnTrigger");
            var newContext = context;
            newContext.AbilityTrigger = this;
            context = newContext;
            var abilityTrigger = this;

            var source = context.TriggerSource;
            Entity target = context.Target;
            if (target == null && source != null)
            {
                target = source;
            }
            if (target == null)
            {
                target = ParentEntity;
            }

            var stateCheckResult = true;

            /// 这里是状态判断，状态判断是判断目标的状态是否满足条件，满足才能触发效果
            if ((abilityTrigger as Entity).TryGet(out TriggerStateCheckComponent component))
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
                            var abilityEffect = effects[i];

                            if (OwnerEntity.EffectAssignAbility.TryMakeAction(out var effectAssign))
                            {
                                effectAssign.AbilityEffect = abilityEffect;
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
    }
}