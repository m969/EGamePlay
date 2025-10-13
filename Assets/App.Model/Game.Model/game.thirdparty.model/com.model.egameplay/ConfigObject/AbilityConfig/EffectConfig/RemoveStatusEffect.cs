using Sirenix.OdinInspector;

namespace EGamePlay.Combat
{
    [Effect("移除Buff", 40)]
    public class RemoveStatusEffect : Effect
    {
        public override string Label
        {
            get
            {
                if (this.RemoveStatus != null)
                {
                    return $"移除 [ {this.RemoveStatus.ShowName} ] Buff";
                }
                return "移除Buff";
            }
        }

        [ToggleGroup("Enabled")]
        [LabelText("状态配置")]
        public AbilityConfigObject RemoveStatus;
    }
}