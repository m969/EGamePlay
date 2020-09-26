using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.IO;
using Sirenix.Utilities.Editor;

namespace EGamePlay.Combat
{
    [CreateAssetMenu(fileName = "状态配置", menuName = "技能|状态/状态配置")]
    [LabelText("状态配置")]
    public class StatusConfigObject : SerializedScriptableObject
    {
        [LabelText("状态ID")]
        [DelayedProperty]
        public string ID = "Status1";
        [LabelText("状态名称")]
        [DelayedProperty]
        public string Name = "状态1";
        [LabelText("状态类型")]
        public StatusType StatusType;
        [LabelText("是否在状态栏显示")]
        public bool ShowInStatusIconList;
        [LabelText("最高叠加层数")]
        [Range(1, 99)]
        public int MaxStack = 1;

        [Toggle("Enabled")]
        public DurationToggleGroup DurationToggleGroup = new DurationToggleGroup();
        
        //[Toggle("Enabled")]
        //public StateToggleGroup StateToggleGroup = new StateToggleGroup();

        //[Space(10)]
        //[LabelText("效果触发机制")]
        //public EffectTriggerType EffectTriggerType;
        //[ShowIf("EffectTriggerType", EffectTriggerType.Condition)]
        //public ConditionType ConditionType;
        //[ShowIf("EffectTriggerType", EffectTriggerType.Action)]
        //public ActionType ActionType;
        //[ShowIf("EffectTriggerType", EffectTriggerType.Interval)]
        //[LabelText("间隔时间")]
        //[SuffixLabel("毫秒", true)]
        //public uint Interval;
        
        //[TitleGroup("状态效果配置", Alignment = TitleAlignments.Centered)]
        //[TabGroup("状态效果配置/TabGroup", "开始")]
        //[LabelText("效果列表")]
        //public SkillEffectToggleGroup[] StartEffectGroupList;
        
        //[TabGroup("状态效果配置/TabGroup", "活跃时")]
        [Space(30)]
        [LabelText("效果列表")]
        public SkillEffectToggleGroup[] RunningEffectGroupList;
        
        //[TabGroup("状态效果配置/TabGroup", "结束")]
        //[LabelText("效果列表")]
        //public SkillEffectToggleGroup[] EndEffectGroupList;

        private void BeginBox()
        {
            GUILayout.Space(30);
            SirenixEditorGUI.BeginBox("状态表现");
        }
        [LabelText("状态特效")]
        [OnInspectorGUI("BeginBox", append:false)]
        public GameObject SkillParticleEffect;

        [LabelText("状态音效")]
        [OnInspectorGUI("EndBox", append:true)]
        public AudioClip SkillAudio;
        private void EndBox()
        {
            SirenixEditorGUI.EndBox();
        }


        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            string[] guids = UnityEditor.Selection.assetGUIDs;
            int i = guids.Length;
            if (i == 1)
            {
                string guid = guids[0];
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var fileName = Path.GetFileName(assetPath);
                var newName = $"Status_{this.ID}_{this.Name}";
                if (!fileName.StartsWith(newName))
                {
                    Debug.Log(assetPath);
                    UnityEditor.AssetDatabase.RenameAsset(assetPath, newName);
                }
            }
        }
    }

    [Serializable]
    public class MyToggleObject
    {
        public bool Enabled;
    }

    [Serializable]
    [LabelText("持续时间")]
    public class DurationToggleGroup : MyToggleObject
    {
        [Tooltip("不勾即代表永久，0也代表永久")]
        [LabelText("持续时间")]
        [SuffixLabel("毫秒", true)]
        public uint Duration;
    }
    
    [Serializable]
    [LabelText("设置状态")]
    public class StateToggleGroup : MyToggleObject
    {
        [LabelText("设置")]
        public StateType StateType;
    }
}