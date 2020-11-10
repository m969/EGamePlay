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
        [LabelText("状态ID"), DelayedProperty]
        public string ID = "Status1";
        [LabelText("状态名称"), DelayedProperty]
        public string Name = "状态1";
        [LabelText("状态类型")]
        public StatusType StatusType;
        //[Tooltip("不勾即代表永久，0也代表永久")]
        //[LabelText("持续时间"), SuffixLabel("毫秒", true)]
        [HideInInspector]
        public uint Duration;
        [LabelText("是否在状态栏显示")]
        public bool ShowInStatusIconList;
        [LabelText("是否可叠加")]
        public bool CanStack;
        [LabelText("最高叠加层数"), ShowIf("CanStack"), Range(1, 99)]
        public int MaxStack = 1;

        //public DurationConfig DurationConfig = new DurationConfig();

        [LabelText("效果列表"), Space(30)]
        [ListDrawerSettings(Expanded = true, DraggableItems = false, ShowItemCount = false, HideAddButton = true)]
        [HideReferenceObjectPicker]
        public List<Effect> Effects = new List<Effect>();

        [HorizontalGroup(PaddingLeft = 40, PaddingRight = 40)]
        [HideLabel]
        [OnValueChanged("AddEffect")]
        [ValueDropdown("EffectTypeSelect")]
        public string EffectTypeName = "(添加效果)";

        public IEnumerable<string> EffectTypeSelect()
        {
            var types = typeof(Effect).Assembly.GetTypes()
                .Where(x => !x.IsAbstract)
                .Where(x => typeof(Effect).IsAssignableFrom(x))
                .Where(x => x.GetCustomAttribute<EffectAttribute>() != null)
                .OrderBy(x => x.GetCustomAttribute<EffectAttribute>().Order)
                .Select(x => x.GetCustomAttribute<EffectAttribute>().EffectType);
            var results = types.ToList();
            results.Insert(0, "(添加效果)");
            return results;
        }

        private void AddEffect()
        {
            if (EffectTypeName != "(添加效果)")
            {
                var effectType = typeof(Effect).Assembly.GetTypes()
                    .Where(x => !x.IsAbstract)
                    .Where(x => typeof(Effect).IsAssignableFrom(x))
                    .Where(x => x.GetCustomAttribute<EffectAttribute>() != null)
                    .Where(x => x.GetCustomAttribute<EffectAttribute>().EffectType == EffectTypeName)
                    .First();

                var effect = Activator.CreateInstance(effectType) as Effect;
                effect.Enabled = true;
                Effects.Add(effect);

                EffectTypeName = "(添加效果)";
            }
            //SkillHelper.AddEffect(Effects, EffectType);
        }

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
    [HideLabel]
    //[LabelText("持续时间")]
    public class DurationConfig
    {
        [ToggleGroup("Enabled", "持续时间")]
        public bool Enabled;

        [ToggleGroup("Enabled")]
        [Tooltip("不勾即代表永久，0也代表永久")]
        [LabelText("持续时间")]
        [SuffixLabel("毫秒", true)]
        public uint Duration;
    }
    
    //[Serializable]
    //[HideLabel]
    ////[LabelText("设置状态")]
    //public class StateConfig
    //{
    //    [ToggleGroup("Enabled", "设置状态")]
    //    public bool Enabled;

    //    [ToggleGroup("Enabled")]
    //    [LabelText("设置")]
    //    public StateType StateType;
    //}
}