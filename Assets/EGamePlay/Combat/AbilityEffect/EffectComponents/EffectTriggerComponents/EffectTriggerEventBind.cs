using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 效果触发事件绑定
    /// </summary>
    public class EffectTriggerEventBind : Entity
    {
        public CombatEntity OwnerEntity => GetParent<AbilityEffect>().OwnerEntity;
        public IAbilityEntity OwnerAbility => GetParent<AbilityEffect>().OwnerAbility as IAbilityEntity;
        public SkillAbility SkillAbility => (OwnerAbility as SkillAbility);
        public string AffectCheck { get; set; }
        public List<IConditionCheckSystem> ConditionChecks { get; set; } = new List<IConditionCheckSystem>();
        public List<Entity> ActionPointBinds { get; set; } = new List<Entity>();


        public override void Awake()
        {
            //Log.Debug($"EffectTriggerEventBind Awake {affectCheck}");
            var effectConfig = GetParent<AbilityEffect>().EffectConfig;
            /// 行动点事件触发
            var isAction = effectConfig.EffectTriggerType == EffectTriggerType.Action;
            if (isAction) AddComponent<EffectActionPointTriggerComponent>();
            /// 条件事件触发
            var isCondition = effectConfig.EffectTriggerType == EffectTriggerType.Condition && !string.IsNullOrEmpty(effectConfig.ConditionParam);
            if (isCondition) AddComponent<EffectConditionEventTriggerComponent>();

            /// 条件判断检测
            if (!string.IsNullOrEmpty(effectConfig.ConditionParam) && effectConfig.ConditionParam.Contains('&'))
            {
                var arr = effectConfig.ConditionParam.Split('&');
                //
                for (int i = 1; i < arr.Length; i++)
                {
                    var conditionStr = arr[i];
                    if (string.IsNullOrEmpty(conditionStr))
                    {
                        continue;
                    }
                    var condition = conditionStr;
                    if (conditionStr.StartsWith("!")) condition = conditionStr.TrimStart('!');
                    var arr2 = condition.Split('<', '=', '≤');
                    var conditionType = arr2[0];
                    var scriptType = $"EGamePlay.Combat.Condition{conditionType}Check";
                    var type = System.Type.GetType(scriptType);
                    if (type != null)
                    {
                        ConditionChecks.Add(this.AddChild(type, conditionStr) as IConditionCheckSystem);
                    }
                    else
                    {
                        Log.Error($"Condition class not found: {scriptType}");
                    }
                }
            }
        }

        public override void OnDestroy()
        {
            foreach (var item in ConditionChecks)
            {
                Entity.Destroy(item as Entity);
            }
            ConditionChecks.Clear();
            foreach (var item in ActionPointBinds)
            {
                Entity.Destroy(item);
            }
            ActionPointBinds.Clear();
            base.OnDestroy();
        }

        public void EnableTriggerBind()
        {
            foreach (var item in Components.Values)
            {
                item.Enable = true;
            }
            /// 立即触发
            var effectConfig = GetParent<AbilityEffect>().EffectConfig;
            if (effectConfig.EffectTriggerType == EffectTriggerType.Instant)
            {
                var effectAssign = GetParent<AbilityEffect>().CreateAssignAction((OwnerAbility as IAbilityEntity).ParentEntity);
                effectAssign.AssignEffect();
            }
        }

        public void TriggerEffectCheck(SkillExecution skillExecution)
        {
            var affectCheck = GetParent<AbilityEffect>().EffectConfig.ConditionParam;
            //Log.Debug($"EffectTriggerEventBind TriggerEffectCheck {affectCheck}");
            //if (GetParent<AbilityEffect>().GetComponent<EffectTargetSelection>() == null && GetParent<AbilityEffect>().IsTargetEffect)
            //{
            //    foreach (var target in skillExecution.MainTargetBattlers)
            //    {
            //        TriggerEffectCheckWithTarget(target);
            //    }
            //}
            //else
            {
                TriggerEffectCheckWithTarget(skillExecution);
            }
        }

        public void TriggerEffectCheckWithTarget(Entity target)
        {
            var affectCheck = GetParent<AbilityEffect>().EffectConfig.ConditionParam;
            //Log.Debug($"EffectTriggerEventBind TriggerEffectCheckWithTarget {affectCheck}");
            var conditionCheckResult = true;
            foreach (var item in ConditionChecks)
            {
                if (item.IsInvert)
                {
                    if (item.CheckCondition(target))
                    {
                        conditionCheckResult = false;
                        break;
                    }
                }
                else
                {
                    if (!item.CheckCondition(target))
                    {
                        conditionCheckResult = false;
                        break;
                    }
                }
            }

            if (conditionCheckResult)
            {
                //var effectTargetSelection = GetParent<AbilityEffect>().GetComponent<EffectTargetSelection>();
                //if (effectTargetSelection != null)
                //{
                //    var targets = effectTargetSelection.GetTargets();
                //    foreach (var battler in targets)
                //    {
                //        GetParent<AbilityEffect>().AssignEffectTo(battler);
                //    }
                //}
                //else
                {
                    var effectAssign = GetParent<AbilityEffect>().CreateAssignAction(target);
                    effectAssign.AssignEffect();
                }
            }
        }

        public void TriggerEffectToParent()
        {
            TriggerEffectCheckWithTarget(OwnerAbility.ParentEntity);
        }
    }
}