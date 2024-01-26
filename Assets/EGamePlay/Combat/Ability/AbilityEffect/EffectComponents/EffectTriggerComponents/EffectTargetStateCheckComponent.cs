using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 效果目标状态判断组件
    /// </summary>
    public class EffectTargetStateCheckComponent : Component
    {
        public override bool DefaultEnable { get; set; } = false;
        public List<IConditionCheckSystem> ConditionChecks { get; set; } = new List<IConditionCheckSystem>();


        public override void Awake()
        {
            var effectConfig = Entity.GetParent<AbilityEffect>().EffectConfig;
            var arr = effectConfig.ConditionParam.Split('&');
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
                    ConditionChecks.Add(Entity.AddChild(type, conditionStr) as IConditionCheckSystem);
                }
                else
                {
                    Log.Error($"Condition class not found: {scriptType}");
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
        }

        public override void OnEnable()
        {

        }

        public override void OnDisable()
        {

        }

        public bool CheckTargetState(Entity target)
        {
            /// 这里是状态条件判断，状态条件判断是判断目标的状态是否满足条件，满足则触发效果
            var conditionCheckResult = true;
            foreach (var item in ConditionChecks)
            {
                /// 条件取反
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
            return conditionCheckResult;
        }
    }
}