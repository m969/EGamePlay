using Sirenix.OdinInspector;
using UnityEngine;

namespace EGamePlay.Combat
{
    [Effect("触发事件", 20)]
    public class ActionEventEffect : ItemEffect
    {
        public override string Label => "触发事件";

        [ToggleGroup("Enabled")]
        public FireType FireType;

        [ToggleGroup("Enabled")]
        [HideIf("FireType", FireType.None)]
        public FireEventType ActionEventType;

        [JsonIgnore]
        public bool IsTriggerAssign
        {
            get
            {
                return FireType != FireType.None && ActionEventType == FireEventType.AssignEffect;
            }
        }

        [JsonIgnore]
        public bool IsTriggerExecution
        {
            get
            {
                return FireType != FireType.None && ActionEventType == FireEventType.TriggerNewExecution;
            }
        }

        [ToggleGroup("Enabled")]
        [ShowIf("IsTriggerAssign")]
        [LabelText("主动触发效果")]
        public ExecuteTriggerType ExecuteTrigger;

        [ToggleGroup("Enabled")]
        [ShowIf("IsTriggerAssign")]
        public EffectApplyTarget EffectApplyTarget;

        [ToggleGroup("Enabled")]
        [ShowIf("IsTriggerExecution")]
        [LabelText("新执行体")]
        public string NewExecution;
    }
}