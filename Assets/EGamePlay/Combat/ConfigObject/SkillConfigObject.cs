﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.IO;
using UnityEngine.PlayerLoop;
using Sirenix.Utilities.Editor;
using UnityEngine.Serialization;
using System.Linq;
using System.Reflection;

namespace EGamePlay.Combat
{
    [CreateAssetMenu(fileName = "技能配置", menuName = "技能|状态/技能配置")]
    [LabelText("技能配置")]
    public class SkillConfigObject : SerializedScriptableObject
    {
        [LabelText("技能ID"), DelayedProperty]
        public uint ID;
        [LabelText("技能名称"), DelayedProperty]
        public string Name = "技能1";
        public SkillSpellType SkillSpellType;
        [LabelText("技能目标检测方式"), ShowIf("SkillSpellType", SkillSpellType.Initiative)]
        public SkillTargetSelectType TargetSelectType;
        [LabelText("区域场类型"), ShowIf("TargetSelectType", SkillTargetSelectType.AreaSelect)]
        public SkillAffectAreaType AffectAreaType;

        [LabelText("圆形区域场半径"), ShowIf("ShowCircleAreaRadius")]
        public float CircleAreaRadius;
        public bool ShowCircleAreaRadius { get { return AffectAreaType == SkillAffectAreaType.Circle && TargetSelectType == SkillTargetSelectType.AreaSelect; } }

        //[LabelText("区域场引导配置"), ShowIf("TargetSelectType", SkillTargetSelectType.AreaSelect)]
        //public GameObject AreaGuideObj;
        [LabelText("区域场配置"), ShowIf("TargetSelectType", SkillTargetSelectType.AreaSelect)]
        public GameObject AreaCollider;

        [LabelText("技能作用对象"), ShowIf("SkillSpellType", SkillSpellType.Initiative)]
        public SkillAffectTargetType AffectTargetType;

        //[ToggleGroup("Cold", "技能冷却")]
        //public bool Cold = false;
        [/*ToggleGroup("Cold"), */LabelText("冷却时间"), SuffixLabel("毫秒", true), ShowIf("SkillSpellType", SkillSpellType.Initiative)]
        public uint ColdTime;

        [LabelText("效果列表"), Space(30)]
        [ListDrawerSettings(Expanded = true, DraggableItems = true, ShowItemCount = false, HideAddButton = true)]
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
                effect.IsSkillEffect = true;
                Effects.Add(effect);

                EffectTypeName = "(添加效果)";
            }
            //SkillHelper.AddEffect(Effects, EffectType);
        }

        [OnInspectorGUI("BeginBox", append: false)]
        [LabelText("技能动作")]
        public AnimationClip SkillAnimationClip;
        [LabelText("技能特效")]
        public GameObject SkillEffectObject;
        [LabelText("技能音效")]
        [OnInspectorGUI("EndBox", append: true)]
        public AudioClip SkillAudio;

        [TextArea, LabelText("技能描述")]
        public string SkillDescription;

#if UNITY_EDITOR
        [SerializeField, LabelText("自动重命名")]
        public bool AutoRename { get { return StatusConfigObject.AutoRenameStatic; } set { StatusConfigObject.AutoRenameStatic = value; } }

        private void OnEnable()
        {
            StatusConfigObject.AutoRenameStatic = UnityEditor.EditorPrefs.GetBool("AutoRename", true);
        }

        private void OnDisable()
        {
            UnityEditor.EditorPrefs.SetBool("AutoRename", StatusConfigObject.AutoRenameStatic);
        }

        private void BeginBox()
        {
            GUILayout.Space(30);
            SirenixEditorGUI.DrawThickHorizontalSeparator();
            GUILayout.Space(10);
            SirenixEditorGUI.BeginBox("技能表现");
        }

        private void EndBox()
        {
            SirenixEditorGUI.EndBox();
            GUILayout.Space(30);
            SirenixEditorGUI.DrawThickHorizontalSeparator();
            GUILayout.Space(10);
        }

        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            //foreach (var item in this.Effects)
            //{
            //    item.IsSkillEffect = true;
            //}

            if (!AutoRename)
            {
                return;
            }

            RenameFile();
        }

        [Button("重命名配置文件"), HideIf("AutoRename")]
        private void RenameFile()
        {
            string[] guids = UnityEditor.Selection.assetGUIDs;
            int i = guids.Length;
            if (i == 1)
            {
                string guid = guids[0];
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var so = UnityEditor.AssetDatabase.LoadAssetAtPath<SkillConfigObject>(assetPath);
                if (so != this)
                {
                    return;
                }
                var fileName = Path.GetFileName(assetPath);
                var newName = $"Skill_{this.ID}_{this.Name}";
                if (!fileName.StartsWith(newName))
                {
                    //Debug.Log(assetPath);
                    UnityEditor.AssetDatabase.RenameAsset(assetPath, newName);
                }
            }
        }
#endif
    }

    [LabelText("护盾类型")]
    public enum ShieldType
    {
        [LabelText("普通护盾")]
        Shield,
        [LabelText("物理护盾")]
        PhysicShield,
        [LabelText("魔法护盾")]
        MagicShield,
        [LabelText("技能护盾")]
        SkillShield,
    }

    [LabelText("标记类型")]
    public enum TagType
    {
        [LabelText("能量标记")]
        Power,
    }
}