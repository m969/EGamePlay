using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.IO;
using Sirenix.Utilities.Editor;
using System.Linq;

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
        [LabelText("是否可叠加")]
        public bool CanStack;
        [LabelText("最高叠加层数"), ShowIf("CanStack"), Range(1, 99)]
        public int MaxStack = 1;

        [Toggle("Enabled")]
        public DurationToggleGroup DurationToggleGroup = new DurationToggleGroup();

        [LabelText("效果列表"), Space(30)]
        [ListDrawerSettings(Expanded = true, DraggableItems = false, /*HideAddButton = true, */ShowItemCount = false)]
        public SkillEffectGroup[] RunningEffectGroupList;

        private void BeginBox()
        {
            GUILayout.Space(30);
            SirenixEditorGUI.DrawThickHorizontalSeparator();
            GUILayout.Space(10);
            SirenixEditorGUI.BeginBox("状态表现");
        }
        [LabelText("状态特效")]
        [OnInspectorGUI("BeginBox", append:false)]
        public GameObject ParticleEffect;

        [LabelText("状态音效")]
        [OnInspectorGUI("EndBox", append:true)]
        public AudioClip Audio;
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