using Sirenix.Config;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    [Serializable]
    public class TriggerEventData
    {
        public EffectApplyData EffectApplyData;
    }

    [Serializable]
#if UNITY
    public class ItemTriggerConfig : System.Object
#else
    public class ItemTriggerConfig : ET.Object
#endif
    {
        [HideInInspector]
        public string Label => TriggerType switch
        {
            ItemTriggerType.BeginTrigger => "片段开始执行",
            ItemTriggerType.CollisionTrigger => "碰撞执行",
            ItemTriggerType.TimeStateTrigger => "计时状态执行",
        };

        [ToggleGroup("Enabled", "$Label")]
        public bool Enabled;

        [ToggleGroup("Enabled")]
        public ItemTriggerType TriggerType = ItemTriggerType.CollisionTrigger;

        //public bool HideAutoTrigger => TriggerType == ItemTriggerType.CollisionTrigger;

        //[FoldoutGroup("Enabled/TriggerType", GroupName = "触发机制")]
        //[ToggleGroup("Enabled"), HideIf("HideAutoTrigger"), LabelText("被动事件")]
        //public EffectAutoTriggerType AutoTriggerType;

        //public bool ShowActionTrigger => !HideAutoTrigger && AutoTriggerType == EffectAutoTriggerType.Action;

        [FoldoutGroup("Enabled/TriggerType")]
        [ToggleGroup("Enabled")]
        public ActionPointType ActionPointType;

        //public bool ShowConditionTrigger => !HideAutoTrigger && AutoTriggerType == EffectAutoTriggerType.Condition;

        [FoldoutGroup("Enabled/TriggerType")]
        [ToggleGroup("Enabled")]
        public TimeStateEventType ConditionType;

        [FoldoutGroup("Enabled/TriggerType")]
        [ToggleGroup("Enabled"), LabelText("x=")]
        public string ConditionParam;

        [ToggleGroup("Enabled")]
        public AddSkillEffetTargetType AddSkillEffectTargetType;

        [ToggleGroup("Enabled"), LabelText("状态判断")]
        public List<string> StateCheckList = new List<string>();

        [ToggleGroup("Enabled")]
        [LabelText("事件列表")]
        public List<TriggerEventData> TriggerEffects = new List<TriggerEventData>();

#if UNITY_EDITOR
        private void DrawSpace()
        {
            GUILayout.Space(20);
        }

        private void BeginBox()
        {
        }

        private void EndBox()
        {
        }
#endif
    }
}