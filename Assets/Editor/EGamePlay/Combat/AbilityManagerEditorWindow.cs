using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace EGamePlay.Combat
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using Sirenix.OdinInspector.Editor;
	using Sirenix.OdinInspector;
	using Sirenix.Utilities.Editor;
	using Sirenix.Utilities;
    using System;
    using System.Linq;

	public class AbilityManagerEditorWindow : OdinEditorWindow
	{
        public static AbilityManagerObject AbilityManagerObject { get; private set; }

        //[MenuItem("Tools/EGamePlay/AbilityManagerEditor")]
        private static void ShowWindow()
        {
			AbilityManagerObject = AssetDatabase.LoadAssetAtPath<AbilityManagerObject>("Assets/EGamePlay.Unity/AbilityManager.asset");
			if (AbilityManagerObject == null )
			{
                AbilityManagerObject = new AbilityManagerObject();
                AssetDatabase.CreateAsset(AbilityManagerObject, "Assets/EGamePlay.Unity/AbilityManager.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            var window = GetWindow<AbilityManagerEditorWindow>(false, "AbilityManagerEditor");
        }

        protected override void OnImGUI()
		{
			base.OnImGUI();

			if (GUILayout.Button("CreateAbility"))
			{
                var ability = new AbilityConfigObject();
                AssetDatabase.CreateAsset(ability, AbilityManagerObject.SkillAssetFolder + "/AbilityConfig.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

			GUILayout.TextArea("AbilityManagerEditorWindow");
		}
    }
}