using Sirenix.Config;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    [Serializable]
    public class EffectApplyData
    {
        [HorizontalGroup, HideLabel]
        public EffectApplyType EffectApplyType;
        [HorizontalGroup, HideLabel]
        public string[] Params;
    }

    [Serializable]
#if UNITY
    public class TriggerConfig : System.Object
#else
    public class TriggerConfig : ET.Object
#endif
    {
        [HideInInspector]
        public string Label => TriggerType == EffectTriggerType.ExecuteTrigger ? "主动触发" : "被动触发";

        [ToggleGroup("Enabled", "$Label")]
        public bool Enabled;

        [ToggleGroup("Enabled")]
        public EffectTriggerType TriggerType = EffectTriggerType.ExecuteTrigger;

        #region 被动触发
        public bool HideAutoTrigger => TriggerType == EffectTriggerType.ExecuteTrigger;

        [FoldoutGroup("Enabled/TriggerType", GroupName = "触发机制")]
        [ToggleGroup("Enabled"), HideIf("HideAutoTrigger"), LabelText("被动事件")]
        public EffectAutoTriggerType AutoTriggerType;

        public bool ShowActionTrigger => !HideAutoTrigger && AutoTriggerType == EffectAutoTriggerType.Action;

        [FoldoutGroup("Enabled/TriggerType")]
        [ToggleGroup("Enabled"), ShowIf("ShowActionTrigger")]
        public ActionPointType ActionPointType;

        public bool ShowConditionTrigger => !HideAutoTrigger && AutoTriggerType == EffectAutoTriggerType.Condition;

        [FoldoutGroup("Enabled/TriggerType")]
        [ToggleGroup("Enabled"), ShowIf("ShowConditionTrigger")]
        public TimeStateEventType ConditionType;

        [FoldoutGroup("Enabled/TriggerType")]
        [ToggleGroup("Enabled"), LabelText("x="), ShowIf("ShowConditionTrigger")]
        public string ConditionParam;

        #endregion

        [Space(10)]
        [ToggleGroup("Enabled")]
        public AddSkillEffetTargetType AddSkillEffectTargetType;

        //[FoldoutGroup("Enabled/TriggerType", GroupName = "触发机制")]
        [ToggleGroup("Enabled"), LabelText("状态判断")]
        public List<string> StateCheckList = new List<string>();

        [ToggleGroup("Enabled")]
        [LabelText("触发效果")]
        public List<EffectApplyData> TriggerEffects = new List<EffectApplyData>();

        //[OnInspectorGUI("EndBox", append: true)]
        ////[FoldoutGroup("Enabled/TriggerType")]
        //[ToggleGroup("Enabled"), LabelText("触发概率")]
        //public string TriggerProbability = "100%";

        //[ToggleGroup("Enabled")]
        //[LabelText("触发效果")]
        //[SerializeField]
        //[DictionaryDrawerSettings(KeyLabel = "", ValueLabel = "")]
        //public Dictionary<EffectApplyType, string[]> TriggerEffects = new Dictionary<EffectApplyType, string[]>();

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