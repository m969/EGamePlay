using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using ET;

#if EGAMEPLAY_ET
using Unity.Mathematics;
using Vector3 = Unity.Mathematics.float3;
using Quaternion = Unity.Mathematics.quaternion;
using JsonIgnore = MongoDB.Bson.Serialization.Attributes.BsonIgnoreAttribute;
using StatusConfig = cfg.Status.StatusCfg;
#endif


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
        [LabelText("状态配置"), JsonIgnore]
        public StatusConfigObject AddStatus;

        private string addStatusId;
        public string AddStatusId
        {
            get
            {
                if (this.AddStatus != null) return AddStatus.ID;
                return addStatusId;
            }
            set
            {
                addStatusId = value;
            }
        }

        public StatusConfig AddStatusConfig { get; set; }

        [ToggleGroup("Enabled"), LabelText("持续时间"), SuffixLabel("毫秒", true)]
        public uint Duration;

        [HideReferenceObjectPicker]
        [ToggleGroup("Enabled")]
        [LabelText("参数列表")]
        public Dictionary<string, string> Params = new Dictionary<string, string>();
    }
}