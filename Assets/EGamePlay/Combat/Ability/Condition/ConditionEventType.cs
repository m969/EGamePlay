using Sirenix.OdinInspector;

namespace EGamePlay.Combat
{
    public enum ConditionEventType
    {
        [LabelText("自定义条件事件")]
        CustomCondition = 0,
        [LabelText("当生命值低于x")]
        WhenHPLower = 1,
        [LabelText("当生命值低于百分比x")]
        WhenHPPctLower = 2,
        [LabelText("当x秒内没有受伤")]
        WhenInTimeNoDamage = 3,
        [LabelText("当间隔x秒")]
        WhenIntervalTime = 4,
    }
}