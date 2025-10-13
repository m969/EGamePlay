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
    [Effect("施加Buff", 30)]
    [Serializable]
    public class AddStatusEffect : Effect
    {
        public override string Label
        {
            get
            {
                if (this.AddStatus != null)
                {
                    return $"施加 [ {this.AddStatus.ShowName} ] Buff";
                }
                return "施加Buff";
            }
        }

        [ToggleGroup("Enabled")]
        [LabelText("状态配置"), JsonIgnore]
        public AbilityConfigObject AddStatus;

        private string addStatusId;
        public string AddStatusId
        {
            get
            {
                if (this.AddStatus != null) return AddStatus.Id.ToString();
                return addStatusId;
            }
            set
            {
                addStatusId = value;
            }
        }

        public AbilityConfig AddStatusConfig { get; set; }

        [ToggleGroup("Enabled"), LabelText("持续时间"), SuffixLabel("秒", true)]
        public float Duration;

        [HideReferenceObjectPicker]
        [ToggleGroup("Enabled")]
        [LabelText("参数列表")]
        public Dictionary<string, string> Params = new Dictionary<string, string>();
    }
}