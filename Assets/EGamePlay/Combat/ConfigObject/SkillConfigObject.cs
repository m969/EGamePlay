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
        public SkillType SkillType;
        public SkillTargetSelectType TargetSelectType;

        [ToggleGroup("TargetSelect", "$TargetGroupTitle")]
        [ReadOnly]
        public bool TargetSelect = true;
        [ToggleGroup("TargetSelect")]
        [HideInInspector]
        public string TargetGroupTitle = "目标限制";
        [ToggleGroup("TargetSelect")]
        public SkillAffectTargetType AffectTargetType;
        [ToggleGroup("TargetSelect")]
        [HideIf("AffectTargetType", SkillAffectTargetType.Self)]
        public SkillTargetType TargetType;

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
        public SkillEffectToggleGroup[] EffectGroupList;
        private void DrawSpace()
        {
            GUILayout.Space(30);
        }

        [BoxGroup("技能表现")]
        [LabelText("技能动作")]
        public AnimationClip SkillAnimationClip;
        [BoxGroup("技能表现")]
        [LabelText("技能特效")]
        public SkillEffectObject SkillEffectObject;
        [BoxGroup("技能表现")]
        [LabelText("技能音效")]
        public AudioClip SkillAudio;


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

    //[Serializable]
    //public class CureToggleGroup : MyToggleObject
    //{
    //    [LabelText("治疗参数")]
    //    public string CureValue;
    //}

    //[Serializable]
    //public class DamageToggleGroup : MyToggleObject
    //{
    //    public DamageType DamageType;
    //    [LabelText("伤害参数")]
    //    public string DamageValue;
    //}

    [Serializable]
    public class SkillEffectToggleGroup
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
                    case SkillEffectType.AddBuff:
                        if (this.AddStatus != null)
                        {
                            return $"施加 [ {this.AddStatus.Name} ] 状态";
                        }
                        return "施加状态";
                    case SkillEffectType.RemoveBuff: return "移除状态";
                    case SkillEffectType.AddShield: return "添加护盾";
                    //case SkillEffectType.AddTag: return "标记叠加";
                    case SkillEffectType.ChangeNumeric: return "改变数值";
                    //case SkillEffectType.ChangeState:
                    //    {
                    //        switch (StateType)
                    //        {
                    //            case StateType.Vertigo: return $"改变[眩晕]状态";
                    //            case StateType.Silent: return $"改变[沉默]状态";
                    //            case StateType.Poison: return $"改变[中毒]状态";
                    //            default: return "改变状态";
                    //        }
                    //    }
                    default: return "（空）";
                }
            }
        }
        [ToggleGroup("Enabled")]
        [ShowIf("SkillEffectType", SkillEffectType.None)]
        public SkillEffectType SkillEffectType;

        [HideInInspector]
        public bool IsSkillEffect;
        
        [ToggleGroup("Enabled")]
        [ShowIf("IsSkillEffect", true)]
        public AddSkillEffetTargetType AddSkillEffectTargetType;

        [ToggleGroup("Enabled")]
        [HideIf("IsSkillEffect", true)]
        public EffectTriggerType EffectTriggerType;

        #region 造成伤害
        [ToggleGroup("Enabled")]
        [ShowIf("SkillEffectType", SkillEffectType.CauseDamage)]
        public DamageType DamageType;
        [ToggleGroup("Enabled")]
        [ShowIf("SkillEffectType", SkillEffectType.CauseDamage)]
        [LabelText("伤害取值")]
        public string DamageValue;
        [ToggleGroup("Enabled")]
        [ShowIf("SkillEffectType", SkillEffectType.CauseDamage)]
        [LabelText("是否可以暴击")]
        public bool CanCrit;
        #endregion

        #region 治疗英雄
        [ToggleGroup("Enabled")]
        [ShowIf("SkillEffectType", SkillEffectType.CureHero)]
        [LabelText("治疗参数")]
        public string CureValue;
        #endregion

        #region 施加状态
        [ToggleGroup("Enabled")]
        [ShowIf("SkillEffectType", SkillEffectType.AddBuff)]
        public StatusConfigObject AddStatus;
        #endregion

        #region 移除状态
        [ToggleGroup("Enabled")]
        [ShowIf("SkillEffectType", SkillEffectType.RemoveBuff)]
        public StatusConfigObject RemoveStatusConfigObject;
        [ToggleGroup("Enabled")]
        [ShowIf("SkillEffectType", SkillEffectType.RemoveBuff)]
        public AddSkillEffetTargetType RemoveBuffTargetType;
        #endregion

        #region 改变数值
        [ToggleGroup("Enabled")]
        [ShowIf("SkillEffectType", SkillEffectType.ChangeNumeric)]
        public NumericType NumericType;
        [ToggleGroup("Enabled")]
        [ShowIf("SkillEffectType", SkillEffectType.ChangeNumeric)]
        [LabelText("数值参数")]
        public string NumericValue;
        #endregion

        #region 添加护盾
        [ToggleGroup("Enabled")]
        [ShowIf("SkillEffectType", SkillEffectType.AddShield)]
        public ShieldType ShieldType;
        [ToggleGroup("Enabled")]
        [ShowIf("SkillEffectType", SkillEffectType.AddShield)]
        [LabelText("护盾值")]
        public uint ShieldValue;
        [ToggleGroup("Enabled")]
        [ShowIf("SkillEffectType", SkillEffectType.AddShield)]
        [LabelText("护盾持续时间")]
        [SuffixLabel("毫秒", true)]
        public uint ShieldDuration;
        #endregion

        //#region 标记叠加
        //[ToggleGroup("Enabled")]
        //[ShowIf("SkillEffectType", SkillEffectType.AddTag)]
        //public TagType TagType;
        //[ToggleGroup("Enabled")]
        //[ShowIf("SkillEffectType", SkillEffectType.AddTag)]
        //[LabelText("标记数量")]
        //public uint TagCount = 1;
        //[ToggleGroup("Enabled")]
        //[ShowIf("SkillEffectType", SkillEffectType.AddTag)]
        //[LabelText("标记停留时间")]
        //[SuffixLabel("毫秒", true)]
        //public uint TagDuration;
        //#endregion
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