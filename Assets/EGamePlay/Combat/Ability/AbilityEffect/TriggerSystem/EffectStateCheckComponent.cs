using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 状态判断组件
    /// </summary>
    public class EffectStateCheckComponent : Component
    {
        public override bool DefaultEnable { get; set; } = false;
        public List<IStateCheck> StateChecks { get; set; } = new List<IStateCheck>();


        public override void Awake()
        {
            var effectConfig = Entity.GetParent<AbilityEffect>().EffectConfig;
            if (effectConfig.StateCheckList == null)
            {
                return;
            }
            foreach (var item in effectConfig.StateCheckList)
            {
                //var type = item.Key;
                //var typeStr = item.Key.ToString();
                var conditionStr = item;
                if (string.IsNullOrEmpty(conditionStr))
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
                    StateChecks.Add(Entity.AddChild(typeClass, conditionStr) as IStateCheck);
                }
                else
                {
                    Log.Error($"Condition class not found: {scriptType}");
                }
            }
        }

        public override void OnDestroy()
        {
            foreach (var item in StateChecks)
            {
                Entity.Destroy(item as Entity);
            }
            StateChecks.Clear();
        }

        public override void OnEnable()
        {

        }

        public override void OnDisable()
        {

        }

        public bool CheckTargetState(Entity target)
        {
            //Log.Debug("EffectTargetStateCheckComponent CheckTargetState");
            /// 这里是状态判断，状态判断是判断目标的状态是否满足条件，满足则触发效果
            var conditionCheckResult = true;
            foreach (var item in StateChecks)
            {
                //Log.Debug($"ConditionChecks {item.GetType().Name}");
                /// 条件取反
                if (item.IsInvert)
                {
                    if (item.CheckWith(target))
                    {
                        conditionCheckResult = false;
                        break;
                    }
                }
                else
                {
                    if (!item.CheckWith(target))
                    {
                        conditionCheckResult = false;
                        break;
                    }
                }
                //Log.Debug($"ConditionChecks {item.GetType().Name} true");
            }
            return conditionCheckResult;
        }
    }
}