using UnityEditor;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    internal class HotReloadOptionsSection {
        /// <remarks>
        /// Opening options tab does not automatically create the settings asset file.
        ///  - The Options UI shows defaults if the object asset doesn't exist.
        ///  - When a build starts, we also ensure the asset file exists.
        /// </remarks>
        public void DrawGUI(SerializedObject so) {
            so.Update(); // must update in-case asset was modified externally

            foreach (var option in HotReloadSettingsTab.allOptions) {
                GUILayout.Space(4f);
                DrawOption(option, so);
            }

            // commit any changes to the underlying ScriptableObject
            if (so.hasModifiedProperties) {
                so.ApplyModifiedProperties();
                // Ensure asset file exists on disk, because we initially create it in memory (to provide the default values)
                // This does not save the asset, user has to do that by saving assets in Unity (e.g. press hotkey Ctrl + S)
                var target = so.targetObject as HotReloadSettingsObject;
                if (target == null) {
                    Log.Warning("Unexpected problem unable to save HotReloadSettingsObject");
                } else {
                    // when one of the project options changed then we ensure the asset file exists.
                    HotReloadSettingsEditor.EnsureSettingsCreated(target);
                }
            }
        }

        static void DrawOption(IOption option, SerializedObject so) {
            EditorGUILayout.BeginVertical(HotReloadWindowStyles.BoxStyle);

            var before = option.GetValue(so);
            var after = EditorGUILayout.BeginToggleGroup(new GUIContent(" " + option.Summary), before);
            if (after != before) {
                option.SetValue(so, after);
            }

            option.InnerOnGUI(so);

            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndVertical();
        }
    }
}
