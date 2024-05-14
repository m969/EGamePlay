using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY
using UnityEditor;
#endif

namespace EGamePlay.Combat
{
    public class EffectDescription
    {
        public int Id;
        public string Name;
        public string Description;
    }

    public class AbilityManagerObject
#if !NOT_UNITY
        : SerializedScriptableObject
#endif
    {
#if UNITY_EDITOR
        private static AbilityManagerObject _instance;
        public static AbilityManagerObject Instance
        {
            get
            {
                _instance = AssetDatabase.LoadAssetAtPath<AbilityManagerObject>("Assets/EGamePlay.Unity/AbilityManager.asset");
                if (_instance == null)
                {
                    _instance = new AbilityManagerObject();
                    AssetDatabase.CreateAsset(_instance, "Assets/EGamePlay.Unity/AbilityManager.asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                return _instance;
            }
        }
#endif

        public string SkillAssetFolder = "Assets/EGPsExamples/RpgExample/Resources/SkillConfigs";
        public string StatusAssetFolder = "Assets/EGPsExamples/RpgExample/Resources/StatusConfigs";

        public string SkillExecutionAssetFolder = "Assets/EGPsExamples/RpgExample/Resources";
        public string StatusExecutionAssetFolder = "Assets/EGPsExamples/RpgExample/Resources";

        [Space(10)]
        public Dictionary<int, string> EffectClasses = new Dictionary<int, string>();
        public Dictionary<int, EffectDescription> EffectTypes = new Dictionary<int, EffectDescription>();
    }
}