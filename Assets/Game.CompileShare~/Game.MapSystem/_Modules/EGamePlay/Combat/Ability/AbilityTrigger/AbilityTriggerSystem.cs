using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System;
using ET;

namespace EGamePlay
{
    public class AbilityTriggerSystem : AEntitySystem<AbilityTrigger>,
        IAwake<AbilityTrigger>,
        IEnable<AbilityTrigger>,
        IDisable<AbilityTrigger>,
        IUpdate<AbilityTrigger>
    {
        public void Awake(AbilityTrigger entity)
        {
            if (entity.TriggerConfig.StateCheckList != null && entity.TriggerConfig.StateCheckList.Count > 0)
            {
                entity.AddComponent<TriggerStateCheckComponent>();
            }
        }

        public void Update(AbilityTrigger entity)
        {
            if (entity.GetComponent<TimeStateObserveComponent>() is { Enable: true } component)
            {
                TimeStateObserveSystem.Update(entity, component);
            }
        }

        public void Enable(AbilityTrigger entity)
        {
            //entity.Enable = true;
            /// 被动触发
            if (entity.TriggerConfig.TriggerType == EffectTriggerType.AutoTrigger)
            {
                /// 能力激活时触发
                if (entity.TriggerConfig.AutoTriggerType == EffectAutoTriggerType.Instant)
                {
                    OnTrigger(entity, new TriggerContext() { Target = entity.ParentEntity });
                }
                /// 按行动点事件触发
                if (entity.TriggerConfig.AutoTriggerType == EffectAutoTriggerType.Action)
                {
                    entity.AddComponent<BehaviourObserveComponent>();
                }
                /// 按计时状态事件触发
                if (entity.TriggerConfig.AutoTriggerType == EffectAutoTriggerType.Condition)
                {
                    var conditionType = entity.TriggerConfig.ConditionType;
                    var paramObj = entity.ConditionParamValue;
                    if (float.TryParse((string)paramObj, out var time))
                    {
                        var condition = entity.AddComponent<TimeStateObserveComponent>(x => x.Timer = new GameTimer(time));
                        TimeStateObserveSystem.StartListen(entity, conditionType);
                    }
                    //if (conditionType == TimeStateEventType.WhenIntervalTime && float.TryParse((string)paramObj, out var intervalTime))
                    //{
                    //    var condition = entity.AddComponent<TimeState_TimeIntervalObserveComponent>(x => x.Interval = intervalTime);
                    //    condition.StartListen(null);
                    //}
                }
            }
        }

        public void Disable(AbilityTrigger entity)
        {
            //entity.Enable = false;
        }

        public static void OnTrigger(AbilityTrigger entity, TriggerContext context)
        {
            var newContext = context;
            newContext.AbilityTrigger = entity;
            context = newContext;
            var abilityTrigger = entity;

            var source = context.TriggerSource;
            var target = context.Target;
            if (target == null && source != null)
            {
                target = source;
            }
            if (target == null)
            {
                target = entity.ParentEntity;
            }

            var stateCheckResult = true;

            /// 这里是状态判断，状态判断是判断目标的状态是否满足条件，满足才能触发效果
            if (abilityTrigger.GetComponent<TriggerStateCheckComponent>() is { } component)
            {
                stateCheckResult = TriggerStateCheckSystem.CheckTargetState(abilityTrigger, target);
            }

            /// 条件满足则触发效果
            if (stateCheckResult)
            {
                foreach (var item in entity.TriggerConfig.TriggerEffects)
                {
                    var effects = entity.OwnerAbility.AbilityEffects;
                    for (int i = 0; i < effects.Count; i++)
                    {
                        if (i == (int)item.EffectApplyType - 1 || item.EffectApplyType == EffectApplyType.AllEffects)
                        {
                            var abilityEffect = effects[i];

                            if (entity.OwnerEntity.EffectAssignAbility.TryMakeAction(out var effectAssign))
                            {
                                effectAssign.AbilityEffect = abilityEffect;
                                effectAssign.AssignTarget = target;
                                effectAssign.SourceAbility = entity.OwnerAbility;
                                effectAssign.TriggerContext = context;
                                AssignActionSystem.Execute(effectAssign);
                            }
                        }
                    }
                }
            }
        }
    }
}