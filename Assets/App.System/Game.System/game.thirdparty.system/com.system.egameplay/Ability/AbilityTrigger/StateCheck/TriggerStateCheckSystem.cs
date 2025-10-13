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
    public class TriggerStateCheckSystem : AComponentSystem<AbilityTrigger, TriggerStateCheckComponent>,
        IAwake<AbilityTrigger, TriggerStateCheckComponent>
    {
        public void Awake(AbilityTrigger entity, TriggerStateCheckComponent component)
        {
            var effectConfig = entity.TriggerConfig;
            if (effectConfig.StateCheckList == null)
            {
                return;
            }
            foreach (var item in effectConfig.StateCheckList)
            {
                var conditionStr = item;
                if (string.IsNullOrEmpty(conditionStr))
                {
                    continue;
                }
                if (conditionStr.StartsWith("#"))
                {
                    continue;
                }
                var condition = conditionStr;
                if (conditionStr.StartsWith("!")) condition = conditionStr.TrimStart('!');
                var arr2 = condition.Split('<', '=', '≤');
                var conditionType = arr2[0];
                var scriptType = $"EGamePlay.Combat.StateCheck_{conditionType}Check";
                var typeClass = System.Type.GetType(scriptType);
                if (typeClass != null)
                {
                    var affectCheck = conditionStr.ToString().ToLower();
                    var stateCheck = entity.AddChild(typeClass) as IStateCheck;
                    //component.StateChecks.Add(stateCheck);
                    component.StateCheckMap.Add((affectCheck, affectCheck.StartsWith("!")), stateCheck);
                }
                else
                {
                    Log.Error($"Condition class not found: {scriptType}");
                }
            }
        }

        public void Destroy(AbilityTrigger entity, TriggerStateCheckComponent component)
        {
            foreach (var item in component.StateCheckMap.Values)
            {
                EcsObject.Destroy(item as EcsEntity);
            }
            component.StateCheckMap.Clear();
        }

        public static bool CheckTargetState(AbilityTrigger entity, EcsEntity target)
        {
            var component = entity.GetComponent<TriggerStateCheckComponent>();
            /// 这里是状态判断，状态判断是判断目标的状态是否满足条件，满足则触发效果
            var conditionCheckResult = true;
            foreach (var pair in component.StateCheckMap)
            {
                var item = pair.Value;
                var invert = pair.Key.Item2;
                var affectCheck = pair.Key.Item1;
                /// 条件取反
                if (invert)
                {
                    if (item.CheckWith(affectCheck, target))
                    {
                        conditionCheckResult = false;
                        ConsoleLog.Debug($"TriggerStateCheckSystem CheckTargetState invert {affectCheck}");
                        break;
                    }
                }
                else
                {
                    if (!item.CheckWith(affectCheck, target))
                    {
                        conditionCheckResult = false;
                        ConsoleLog.Debug($"TriggerStateCheckSystem CheckTargetState {affectCheck}");
                        break;
                    }
                }
            }
            return conditionCheckResult;
        }
    }
}