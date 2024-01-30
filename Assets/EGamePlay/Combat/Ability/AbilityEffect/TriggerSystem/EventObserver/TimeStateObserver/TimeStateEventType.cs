using Sirenix.OdinInspector;

namespace EGamePlay.Combat
{
    [LabelText("计时状态事件")]
    public enum TimeStateEventType
    {
        [LabelText("自定义计时状态事件")]
        CustomCondition = 0,
        [LabelText("当x秒内没有受伤")]
        WhenInTimeNoDamage = 3,
        [LabelText("当间隔x秒")]
        WhenIntervalTime = 4,
    }
}