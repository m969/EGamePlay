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
    //[CreateAssetMenu(fileName = "效果配置", menuName = "技能|状态/效果配置")]
    //[LabelText("效果配置")]
    public class EffectConfigObject : SerializedScriptableObject
    {
        [LabelText("效果ID")]
        [DelayedProperty]
        public string ID = "Effect1";
        [LabelText("效果名称")]
        [DelayedProperty]
        public string Name = "效果1";
        //[LabelText("效果类型")]
        //public StatusType StatusType;
        //[LabelText("是否在状态栏显示")]
        //public bool ShowInStatusIconList;
        //[LabelText("最高叠加层数")]
        //[Range(1, 99)]
        //public int MaxStack = 1;

        //[Toggle("Enabled")]
        //public DurationToggleGroup DurationToggleGroup = new DurationToggleGroup();

        //[Space(30)]
        //[LabelText("效果列表")]
        //[ListDrawerSettings(Expanded = true, DraggableItems = false, /*HideAddButton = true, */ShowItemCount = false)]
        //public SkillEffectGroup[] RunningEffectGroupList;

        private void BeginBox()
        {
            GUILayout.Space(30);
            SirenixEditorGUI.DrawThickHorizontalSeparator();
            GUILayout.Space(10);
            SirenixEditorGUI.BeginBox("效果表现");
        }
        [LabelText("效果特效")]
        [OnInspectorGUI("BeginBox", append:false)]
        public GameObject ParticleEffect;

        [LabelText("效果音效")]
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
                var newName = $"Effect_{this.ID}_{this.Name}";
                if (!fileName.StartsWith(newName))
                {
                    Debug.Log(assetPath);
                    UnityEditor.AssetDatabase.RenameAsset(assetPath, newName);
                }
            }
        }
    }
}