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
//#if UNITY_EDITOR
//        private static AbilityManagerObject _instance;
//        public static AbilityManagerObject Instance
//        {
//            get
//            {
//                _instance = AssetDatabase.LoadAssetAtPath<AbilityManagerObject>("Assets/EGPsExamples/Resources/AbilityManager.asset");
//                if (_instance == null)
//                {
//                    _instance = new AbilityManagerObject();
//                    AssetDatabase.CreateAsset(_instance, "Assets/EGPsExamples/Resources/AbilityManager.asset");
//                    AssetDatabase.SaveAssets();
//                    AssetDatabase.Refresh();
//                }
//                return _instance;
//            }
//        }
//#endif

        //public string ObjectAssetFolder = "Assets/Resources";

        public string SkillAssetFolder = "Assets/EGPsExamples/Resources/AbilityObjects/Skill";
        public string BuffAssetFolder = "Assets/EGPsExamples/Resources/AbilityObjects/Buff";
        public string ExecutionAssetFolder = "Assets/EGPsExamples/Resources/ExecutionObjects";

        //public string SkillExecutionAssetFolder = "Assets/Resources/ExecutionObjects";
        //public string StatusExecutionAssetFolder = "Assets/Resources/ExecutionObjects";

        public const string SkillResFolder = "AbilityObjects/Skill";
        public const string BuffResFolder = "AbilityObjects/Buff";
        public const string ExecutionResFolder = "ExecutionObjects";

        [Space(10)]
        public Dictionary<int, string> EffectClasses = new Dictionary<int, string>();
        public Dictionary<int, EffectDescription> EffectTypes = new Dictionary<int, EffectDescription>();
    }
}