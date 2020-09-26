namespace EGamePlay.Combat
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    using Sirenix.OdinInspector;

    public static class BuffHelper
    {
        public static Dictionary<int, string> buffTypes = new Dictionary<int, string>()
    {
        {0, "（功能效果）"  },
        {1, "立即执行逻辑"  },
        //{2, "条件执行逻辑"  },
        {2, "动作式触发（功能）"  },
        {3, "间隔式触发（功能）"  },
        {4, "条件式触发（功能）"  },
    };
        public static Dictionary<int, string> logicTypes = new Dictionary<int, string>()
    {
        {0, "（逻辑类型）"  },
        {1, "改变状态"  },
        {2, "改变数值"  },
        {3, "执行逻辑"  },
    };

        public static string[] buffTypeKArr;
        public static int[] buffTypeVArr;

        public static string[] logicTypeKArr;
        public static int[] logicTypeVArr;

        public static string[] stateTypeKArr;
        public static int[] stateTypeVArr;

        public static string[] numericTypeKArr;
        public static int[] numericTypeVArr;

        public static string[] actionTypeKArr;
        public static int[] actionTypeVArr;

        public static string[] conditionTypeKArr;
        public static int[] conditionTypeVArr;

        private static (string[], int[]) LoadConfig(string configName)
        {
            var data = UnityEditor.AssetDatabase.LoadAssetAtPath<NameArrayObject>($"Assets/{configName}.asset");
            var kArr = new string[data.Names.Length + 1];
            kArr[0] = "（空）";
            var vArr = new int[data.Names.Length + 1];
            for (int i = 0; i < data.Names.Length; i++)
            {
                kArr[i + 1] = data.Names[i];
            }
            for (int i = 0; i < vArr.Length; i++)
            {
                vArr[i] = i;
            }
            return (kArr, vArr);
        }

        public static void Init()
        {
            BuffHelper.buffTypeKArr = BuffHelper.buffTypes.Values.ToArray();
            BuffHelper.buffTypeVArr = BuffHelper.buffTypes.Keys.ToArray();
            BuffHelper.logicTypeKArr = BuffHelper.logicTypes.Values.ToArray();
            BuffHelper.logicTypeVArr = BuffHelper.logicTypes.Keys.ToArray();

            (BuffHelper.stateTypeKArr, BuffHelper.stateTypeVArr) = LoadConfig("状态配置");
            BuffHelper.stateTypeKArr[0] = "（请选择状态）";
            (BuffHelper.numericTypeKArr, BuffHelper.numericTypeVArr) = LoadConfig("属性配置");
            BuffHelper.numericTypeKArr[0] = "（请选择属性）";
            (BuffHelper.actionTypeKArr, BuffHelper.actionTypeVArr) = LoadConfig("动作配置");
            BuffHelper.actionTypeKArr[0] = "（请选择动作）";
            (BuffHelper.conditionTypeKArr, BuffHelper.conditionTypeVArr) = LoadConfig("条件配置");
            BuffHelper.conditionTypeKArr[0] = "（请选择条件）";
        }
    }


    public class BuffConfig
    {
        public int Id;
        public int Type;
        public string Name;
        public float Duration;
        public List<FunctionConfig> Functions;
    }

    public class FunctionConfig
    {
        public int Type;
        public ActionTrigger ActionTrigger;
        public IntervalTrigger IntervalTrigger;
        public ConditionTrigger ConditionTrigger;
        public ExecuteLogic ExecuteLogic;
    }

    public class LogicConfig
    {
        public int Type;
        public ChangeState ChangeState;
        public ChangeNumeric ChangeNumeric;
    }

    public abstract class Function
    {
        //public string ParamValue;
    }

    public abstract class Buff
    {

    }

    public class ChangeState : Function
    {
        public int State;
        public string Value;
    }

    public class ChangeNumeric : Function
    {
        public int Numeric;
        public string Value;
    }

    public class ActionTrigger : Function
    {
        public int Action;
        //public int Logic;
        public FunctionConfig LogicFunc = new FunctionConfig();
    }

    public class IntervalTrigger : Function
    {
        public int Interval;
        //public int Logic;
        public FunctionConfig LogicFunc = new FunctionConfig();
    }

    public class ConditionTrigger : Function
    {
        public int Condition;
        public string Value;
        public FunctionConfig LogicFunc = new FunctionConfig();
    }

    public class ExecuteLogic : Function
    {
        //public int Logic;
        //public LogicConfig LogicConfig = new LogicConfig();
        public int Type;
        public ChangeState ChangeState;
        public ChangeNumeric ChangeNumeric;
        public string Value;
    }

    public enum StatusType
    {
        [LabelText("Buff(增益)")]
        Buff,
        [LabelText("Debuff(减益)")]
        Debuff,
        [LabelText("其他")]
        Other,
    }

    [LabelText("效果触发机制")]
    public enum EffectTriggerType
    {
        [LabelText("立即触发")]
        Immediate,
        [LabelText("条件触发")]
        Condition,
        [LabelText("动作触发")]
        Action,
        [LabelText("间隔触发")]
        Interval,
    }

    [LabelText("动作类型")]
    public enum ActionType
    {
        [LabelText("发出普攻")]
        Attack,
        [LabelText("遭受普攻")]
        BeAttack,
    }

    [LabelText("条件类型")]
    public enum ConditionType
    {
        [LabelText("当生命值低于")]
        WhenHPLower,
        [LabelText("当生命值低于百分比")]
        WhenHPPctLower,
    } 
}