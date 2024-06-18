using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Sirenix.OdinInspector;
using ET;

#if EGAMEPLAY_ET
using StatusConfig = cfg.Status.StatusCfg;
using Unity.Mathematics;
using Vector3 = Unity.Mathematics.float3;
using Quaternion = Unity.Mathematics.quaternion;
using JsonIgnore = MongoDB.Bson.Serialization.Attributes.BsonIgnoreAttribute;
#endif

namespace EGamePlay.Combat
{
    [CreateAssetMenu(fileName = "状态配置", menuName = "技能|状态/状态配置")]
    public class StatusConfigObject
#if UNITY
        : SerializedScriptableObject
#endif
    {
        [LabelText(StatusIdLabel), DelayedProperty]
        public string ID = "Status1";

        [LabelText(StatusNameLabel), DelayedProperty]
        public string Name = "状态1";

#if UNITY && !EGAMEPLAY_ET
        [LabelText("状态特效")]
        public GameObject ParticleEffect;
        public GameObject GetParticleEffect() => ParticleEffect;
#endif


        [LabelText("子状态效果")]
        public bool EnableChildrenStatuses;
        [OnInspectorGUI("DrawSpace", append: true)]
        [HideReferenceObjectPicker]
        [LabelText("子状态效果列表"), ShowIf("EnableChildrenStatuses"), ListDrawerSettings(DraggableItems = false, ShowItemCount = false, CustomAddFunction = "AddChildStatus")]
        public List<ChildStatus> ChildrenStatuses = new List<ChildStatus>();

        private void AddChildStatus()
        {
            ChildrenStatuses.Add(new ChildStatus());
        }

        [LabelText("效果列表"), Space(30)]
        [ListDrawerSettings(DefaultExpandedState = true, DraggableItems = false, ShowItemCount = false, HideAddButton = true)]
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
                Effects.Add(effect);
                EffectTypeName = "(添加效果)";
            }
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
            var skillConfigFolder = Application.dataPath + "/../../../StatusConfigs";
            var filePath = skillConfigFolder + $"/Status_{ID}.json";
            Debug.Log("SaveJson" + filePath);
            File.WriteAllText(filePath, JsonHelper.ToJson(this));
        }
#endif

        private void EndBox()
        {
            Sirenix.Utilities.Editor.SirenixEditorGUI.EndBox();
            GUILayout.Space(30);
            Sirenix.Utilities.Editor.SirenixEditorGUI.DrawThickHorizontalSeparator();
            GUILayout.Space(10);
        }

        private void RenameFile()
        {
            string[] guids = UnityEditor.Selection.assetGUIDs;
            int i = guids.Length;
            if (i == 1)
            {
                string guid = guids[0];
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var so = UnityEditor.AssetDatabase.LoadAssetAtPath<StatusConfigObject>(assetPath);
                if (so != this)
                {
                    return;
                }
                var fileName = Path.GetFileNameWithoutExtension(assetPath);
                var newName = $"Status_{this.ID}";
                if (fileName != newName)
                {
                    UnityEditor.AssetDatabase.RenameAsset(assetPath, newName);
                }
            }
        }
#endif


#if EGamePlay_EN
        private const string StatusIdLabel = "StatusID";
        private const string StatusNameLabel = "Name";
        private const string StatusTypeLabel = "Type";
#else
        private const string StatusIdLabel = "状态ID";
        private const string StatusNameLabel = "状态名称";
        private const string StatusTypeLabel = "状态类型";
#endif
    }

    public class ChildStatus
    {
        [LabelText("状态效果")]
        public StatusConfigObject StatusConfigObject;

        public StatusConfig StatusConfig { get; set; }

        [LabelText("参数列表"), HideReferenceObjectPicker]
        public Dictionary<string, string> Params = new Dictionary<string, string>();
    }

    public enum StatusType
    {
        [LabelText("Buff(增益)")]
        Buff,
        [LabelText("Debuff(减益)")]
        Debuff,
        [LabelText("其他")]
        Other,
    }

    public enum EffectTriggerType
    {
        [LabelText("（空）")]
        None = 0,
        [LabelText("能力激活时生效")]
        Instant = 1,
        [LabelText("按行动点事件")]
        Action = 2,
        [LabelText("按计时状态事件")]
        Condition = 3,
    }

    public enum StateCheckType
    {
        [LabelText("（空）")]
        None = 0,
        [LabelText("如果目标生命值低于x")]
        WhenTargetHPLower = 1,
        [LabelText("如果目标生命值低于百分比x")]
        WhenTargetHPPctLower = 2,
    }
}