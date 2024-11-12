using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;
using ET;

#if EGAMEPLAY_ET
using Unity.Mathematics;
using Vector3 = Unity.Mathematics.float3;
using Quaternion = Unity.Mathematics.quaternion;
using JsonIgnore = MongoDB.Bson.Serialization.Attributes.BsonIgnoreAttribute;
#endif

#if !NOT_UNITY
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif
#endif

namespace EGamePlay.Combat
{
    [CreateAssetMenu(fileName = "能力配置", menuName = "能力/能力配置")]
    [LabelText("技能配置")]
    public class AbilityConfigObject
#if !NOT_UNITY
        : SerializedScriptableObject
#endif
    {
        [LabelText("技能ID"), DelayedProperty]
        public int Id;
        [LabelText("显示名称")]
        public string ShowName;

        [HideInInspector]
        public SkillSpellType SkillSpellType;

        [HideInInspector]
        [ShowIf("SkillSpellType", SkillSpellType.Initiative)]
        public SkillAffectTargetType AffectTargetType;

        [HideInInspector]
        [LabelText("目标选取类型"), ShowIf("SkillSpellType", SkillSpellType.Initiative)]
        public SkillTargetSelectType TargetSelectType;

        [LabelText("触发点"), Space(30)]
        [ListDrawerSettings(DefaultExpandedState = true, DraggableItems = true, ShowItemCount = false, CustomAddFunction = "AddTrigger")]
        [HideReferenceObjectPicker]
        public List<TriggerConfig> TriggerActions = new List<TriggerConfig>();

        [LabelText("效果列表"), Space(30)]
        [ListDrawerSettings(DefaultExpandedState = true, DraggableItems = true, ShowItemCount = false, HideAddButton = true)]
        [HideReferenceObjectPicker]
        public List<Effect> Effects = new List<Effect>();

        [OnInspectorGUI("BeginBox", append: false)]
        [HorizontalGroup(PaddingLeft = 40, PaddingRight = 40)]
        [HideLabel, OnValueChanged("AddEffect"), ValueDropdown("EffectTypeSelect"), JsonIgnore]
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
                    .FirstOrDefault();

                var effect = Activator.CreateInstance(effectType) as Effect;
                effect.Enabled = true;
                effect.IsSkillEffect = true;
                Effects.Add(effect);

                EffectTypeName = "(添加效果)";
            }
        }

        private void AddTrigger()
        {
            TriggerActions.Add(new TriggerConfig() { Enabled = true });
        }

#if UNITY_EDITOR
        private void DrawSpace()
        {
            GUILayout.Space(20);
        }

        private void BeginBox()
        {
            GUILayout.Space(10);
            //if (GUILayout.Button("Save Json"))
            //{
            //    SaveJson();
            //}
        }

#if EGAMEPLAY_ET
        [Button("Save Json")]
        private void SaveJson()
        {
            var skillConfigFolder = Application.dataPath + "/../../../SkillConfigs";
            var filePath = skillConfigFolder + $"/Skill_{Id}.json";
            Debug.Log("SaveJson" + filePath);
            File.WriteAllText(filePath, JsonHelper.ToJson(this));
        }
#endif

        private void EndBox()
        {
            GUILayout.Space(30);
        }

        private void RenameFile()
        {
            string[] guids = UnityEditor.Selection.assetGUIDs;
            int i = guids.Length;
            if (i == 1)
            {
                string guid = guids[0];
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var so = UnityEditor.AssetDatabase.LoadAssetAtPath<AbilityConfigObject>(assetPath);
                if (so != this)
                {
                    return;
                }
                var fileName = Path.GetFileName(assetPath);
                var newName = $"Skill_{this.Id}";
                if (!fileName.StartsWith(newName))
                {
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