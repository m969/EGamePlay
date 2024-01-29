using Sirenix.OdinInspector;

namespace EGamePlay.Combat
{
    [LabelText("条件类型")]
    public enum TimeStateEventType
    {
        [LabelText("自定义条件事件")]
        CustomCondition = 0,
        [LabelText("当x秒内没有受伤")]
        WhenInTimeNoDamage = 3,
        [LabelText("当间隔x秒")]
        WhenIntervalTime = 4,
    }
}