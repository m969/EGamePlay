using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace EGamePlay.Combat
{
    [Effect("自定义效果", 1000)]
    public class CustomEffect : Effect
    {
        public override string Label => "自定义效果";

        [ToggleGroup("Enabled"), LabelText("自定义效果")]
        public string CustomEffectType;

        [ToggleGroup("Enabled"), LabelText("参数列表")]
        public Dictionary<string, string> Params = new Dictionary<string, string>();
    }
}