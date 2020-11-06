using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.IO;
using Sirenix.Utilities.Editor;
using System.Linq;
using UnityEditor;
using System.Reflection;

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
        [ListDrawerSettings(Expanded = true, DraggableItems = false, ShowItemCount = false)]
        public SkillEffectGroup[] RunningEffectGroupList;

        [LabelText("效果列表"), Space(30)]
        [ListDrawerSettings(Expanded = true, DraggableItems = false, ShowItemCount = false, HideAddButton = true)]
        //[TypeFilter("GetFilteredTypeList")]
        [HideReferenceObjectPicker]
        public List<MyToggleObject> MyToggleObjects = new List<MyToggleObject>();
        [HorizontalGroup(/*Width = 120*//*, */PaddingLeft = 40, PaddingRight = 40/*, MarginLeft = 60, MarginRight = 60*/)]
        [HideLabel]
        [OnValueChanged("AddEffect")]
        public SkillEffectType EffectType;

        private void AddEffect()
        {
            if (EffectType != SkillEffectType.None)
            {
                if (EffectType == SkillEffectType.AddStatus) MyToggleObjects.Add(new StateToggleGroup());
                if (EffectType == SkillEffectType.NumericModify) MyToggleObjects.Add(new DurationToggleGroup());
                EffectType = SkillEffectType.None;
            }
        }

        //public IEnumerable<Type> GetFilteredTypeList()
        //{
        //    var q = typeof(MyToggleObject).Assembly.GetTypes()
        //        .Where(x => !x.IsAbstract)
        //        .Where(x => typeof(MyToggleObject).IsAssignableFrom(x));
        //    return q;
        //}

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

        //private bool NeedClearLog;
        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            //if (NeedClearLog)
            //{
            //    var assembly = Assembly.GetAssembly(typeof(UnityEditor.SceneView));
            //    var type = assembly.GetType("UnityEditor.LogEntries");
            //    var method = type.GetMethod("Clear");
            //    method.Invoke(new object(), null);
            //    NeedClearLog = false;
            //}
            //if (EffectType != SkillEffectType.None)
            //{
            //    if (EffectType == SkillEffectType.AddStatus) MyToggleObjects.Add(new StateToggleGroup());
            //    if (EffectType == SkillEffectType.NumericModify) MyToggleObjects.Add(new DurationToggleGroup());
            //    EffectType = SkillEffectType.None;
            //    NeedClearLog = true;
            //}

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
    public abstract class MyToggleObject
    {

    }

    [Serializable]
    [HideLabel]
    //[LabelText("持续时间")]
    public class DurationToggleGroup : MyToggleObject
    {
        [ToggleGroup("Enabled", "持续时间")]
        public bool Enabled;

        [ToggleGroup("Enabled")]
        [Tooltip("不勾即代表永久，0也代表永久")]
        [LabelText("持续时间")]
        [SuffixLabel("毫秒", true)]
        public uint Duration;
    }
    
    [Serializable]
    [HideLabel]
    //[LabelText("设置状态")]
    public class StateToggleGroup : MyToggleObject
    {
        [ToggleGroup("Enabled", "设置状态")]
        public bool Enabled;

        [ToggleGroup("Enabled")]
        [LabelText("设置")]
        public StateType StateType;
    }
}