using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace EGamePlay.Combat
{
    [Effect("施加状态效果", 30)]
    [Serializable]
    public class AddStatusEffect : Effect
    {
        public override string Label
        {
            get
            {
                if (this.AddStatus != null)
                {
                    return $"施加 [ {this.AddStatus.Name} ] 状态效果";
                }
                return "施加状态效果";
            }
        }

        [ToggleGroup("Enabled")]
        [LabelText("状态配置")]
        public StatusConfigObject AddStatus;

        [ToggleGroup("Enabled"), LabelText("持续时间"), SuffixLabel("毫秒", true)]
        public uint Duration;

        //[ToggleGroup("Enabled"), LabelText("参数")]
        //public string ParamValue;

        [HideReferenceObjectPicker]
        [ToggleGroup("Enabled")]
        [LabelText("参数列表")]
        public Dictionary<string, string> Params = new Dictionary<string, string>();
    }
}