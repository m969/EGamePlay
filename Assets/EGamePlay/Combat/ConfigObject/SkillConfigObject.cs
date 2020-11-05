using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.IO;
using Sirenix.OdinInspector.Editor;
using UnityEngine.PlayerLoop;
using Sirenix.Utilities.Editor;
using UnityEngine.Serialization;
using System.Linq;

namespace EGamePlay.Combat
{
    [CreateAssetMenu(fileName = "技能配置", menuName = "技能|状态/技能配置")]
    [LabelText("技能配置")]
    public class SkillConfigObject : SerializedScriptableObject
    {
        [LabelText("技能ID")]
        [DelayedProperty]
        public uint ID;
        [LabelText("技能名称")]
        [DelayedProperty]
        public string Name = "技能1";
        public SkillSpellType SkillSpellType;
        [LabelText("技能目标检测方式")]
        public SkillTargetSelectType TargetSelectType;
        [ShowIf("TargetSelectType", SkillTargetSelectType.AreaSelect)]
        [LabelText("区域场类型")]
        public SkillAffectAreaType AffectAreaType;

        [LabelText("圆形区域场半径")]
        [ShowIf("ShowCircleAreaRadius")]
        public float CircleAreaRadius;
        public bool ShowCircleAreaRadius { get { return AffectAreaType == SkillAffectAreaType.Circle && TargetSelectType == SkillTargetSelectType.AreaSelect; } }

        [LabelText("区域场引导配置")]
        [ShowIf("TargetSelectType", SkillTargetSelectType.AreaSelect)]
        public GameObject AreaGuideObj;
        [LabelText("区域场配置")]
        [ShowIf("TargetSelectType", SkillTargetSelectType.AreaSelect)]
        public GameObject AreaCollider;

        [LabelText("技能作用对象")]
        public SkillAffectTargetType AffectTargetType;

        [ToggleGroup("Cold", "$ColdGroupTitle")]
        public bool Cold = false;
        [ToggleGroup("Cold")]
        [HideInInspector]
        public string ColdGroupTitle = "技能冷却";
        [ToggleGroup("Cold")]
        [LabelText("冷却时间")]
        [SuffixLabel("毫秒", true)]
        public uint ColdTime;

        [Space(30)]
        [LabelText("效果列表")]
        [OnInspectorGUI("DrawSpace", append:true)]
        [ListDrawerSettings(Expanded = true, DraggableItems = false, ShowItemCount = false)]
        public SkillEffectGroup[] EffectGroupList;
        //[ListDrawerSettings(Expanded = true, DraggableItems = false, ShowItemCount = false)]
        //[LabelText("效果列表")]
        //public EffectConfigObject[] Effects;
        private void DrawSpace()
        {
            GUILayout.Space(30);
        }
        private void OnEndListElementGUI()
        {
            if (GUILayout.Button("+"))
            {
                var list = EffectGroupList.ToList();
                list.Add(new SkillEffectGroup());
                EffectGroupList = list.ToArray();
            }
        }

        private void BeginBox()
        {
            GUILayout.Space(30);
            SirenixEditorGUI.DrawThickHorizontalSeparator();
            GUILayout.Space(10);
            SirenixEditorGUI.BeginBox("技能表现");
        }
        [OnInspectorGUI("BeginBox", append: false)]
        [LabelText("技能动作")]
        public AnimationClip SkillAnimationClip;
        [LabelText("技能特效")]
        public SkillEffectObject SkillEffectObject;
        [LabelText("技能音效")]
        [OnInspectorGUI("EndBox", append: true)]
        public AudioClip SkillAudio;
        private void EndBox()
        {
            SirenixEditorGUI.EndBox();
        }

        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            foreach (var item in this.EffectGroupList)
            {
                item.IsSkillEffect = true;
            }
            string[] guids=UnityEditor.Selection.assetGUIDs;
            int i=guids.Length;
            if (i == 1)
            {
                string guid=guids[0];
                string assetPath=UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var fileName = Path.GetFileName(assetPath);
                var newName = $"Skill_{this.ID}_{this.Name}";
                if (!fileName.StartsWith(newName))
                {
                    Debug.Log(assetPath);
                    UnityEditor.AssetDatabase.RenameAsset(assetPath, newName);
                }
            }
        }
    }

    [Serializable]
    public class SkillEffectGroup
    {
        [ToggleGroup("Enabled", "$Label")]
        public bool Enabled;
        public string Label
        {
            get
            {
                switch (SkillEffectType)
                {
                    case SkillEffectType.None: return "（空）";
                    case SkillEffectType.CauseDamage: return "造成伤害";
                    case SkillEffectType.CureHero: return "治疗英雄";
                    case SkillEffectType.AddStatus:
                        if (this.AddStatus != null)
                        {
                            return $"施加 [ {this.AddStatus.Name} ] 状态";
                        }
                        return "施加状态";
                    case SkillEffectType.RemoveStatus:
                        if (this.RemoveStatus != null)
                        {
                            return $"移除 [ {this.RemoveStatus.Name} ] 状态";
                        }
                        return "移除状态";
                    case SkillEffectType.AddShield: return "添加护盾";
                    case SkillEffectType.NumericModify: return "增减数值";
                    case SkillEffectType.StackTag: return "标记叠加";
                    default: return "（空）";
                }
            }
        }
        [ToggleGroup("Enabled"), ShowIf("SkillEffectType", SkillEffectType.None)]
        public SkillEffectType SkillEffectType;

        [HideInInspector]
        public bool IsSkillEffect;
        
        [ToggleGroup("Enabled"), ShowIf("IsSkillEffect", true)]
        public AddSkillEffetTargetType AddSkillEffectTargetType;

        [ToggleGroup("Enabled"), HideIf("IsSkillEffect", true)]
        public EffectTriggerType EffectTriggerType;

        [ToggleGroup("Enabled"), LabelText("间隔时间"), ShowIf("EffectTriggerType", EffectTriggerType.Interval), SuffixLabel("毫秒", true)]
        public uint Interval;

        #region 造成伤害
        [ToggleGroup("Enabled"), ShowIf("SkillEffectType", SkillEffectType.CauseDamage)]
        public DamageType DamageType;
        [ToggleGroup("Enabled"), LabelText("伤害取值"), ShowIf("SkillEffectType", SkillEffectType.CauseDamage)]
        public string DamageValue;
        [ToggleGroup("Enabled"), LabelText("是否可以暴击"), ShowIf("SkillEffectType", SkillEffectType.CauseDamage)]
        public bool CanCrit;
        #endregion

        #region 治疗英雄
        [ToggleGroup("Enabled"), LabelText("治疗参数"), ShowIf("SkillEffectType", SkillEffectType.CureHero)]
        public string CureValue;
        #endregion

        #region 施加状态
        [ToggleGroup("Enabled"), ShowIf("SkillEffectType", SkillEffectType.AddStatus)]
        public StatusConfigObject AddStatus;
        #endregion

        #region 移除状态
        [ToggleGroup("Enabled"), ShowIf("SkillEffectType", SkillEffectType.RemoveStatus)]
        public StatusConfigObject RemoveStatus;
        //[ToggleGroup("Enabled"), ShowIf("SkillEffectType", SkillEffectType.RemoveStatus)]
        //public AddSkillEffetTargetType RemoveBuffTargetType;
        #endregion

        #region 数值增减
        [ToggleGroup("Enabled"), ShowIf("SkillEffectType", SkillEffectType.NumericModify)]
        public NumericType NumericType;
        [ToggleGroup("Enabled"), LabelText("数值参数"), ShowIf("SkillEffectType", SkillEffectType.NumericModify)]
        public string NumericValue;
        #endregion

        #region 添加护盾
        [ToggleGroup("Enabled"), ShowIf("SkillEffectType", SkillEffectType.AddShield)]
        public ShieldType ShieldType;
        [ToggleGroup("Enabled"), LabelText("护盾值"), ShowIf("SkillEffectType", SkillEffectType.AddShield)]
        public uint ShieldValue;
        [ToggleGroup("Enabled"), LabelText("护盾持续时间"), ShowIf("SkillEffectType", SkillEffectType.AddShield), SuffixLabel("毫秒", true)]
        public uint ShieldDuration;
        #endregion

        #region 标记叠加
        [ToggleGroup("Enabled"), ShowIf("SkillEffectType", SkillEffectType.StackTag)]
        public TagType TagType;
        [ToggleGroup("Enabled"), LabelText("叠加数量"), ShowIf("SkillEffectType", SkillEffectType.StackTag)]
        public uint TagCount = 1;
        [ToggleGroup("Enabled"), LabelText("标记停留时间"), ShowIf("SkillEffectType", SkillEffectType.StackTag), SuffixLabel("毫秒", true)]
        public uint TagDuration;
        #endregion
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