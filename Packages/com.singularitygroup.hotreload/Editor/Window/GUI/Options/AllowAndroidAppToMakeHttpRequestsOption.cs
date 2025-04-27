using UnityEditor;

namespace SingularityGroup.HotReload.Editor {
    internal class AllowAndroidAppToMakeHttpRequestsOption : ProjectOptionBase {
        public override string ShortSummary {
            get {
                return "Allow app to make HTTP requests";
            }
        }

        public override string Summary => ShortSummary;

        public override bool GetValue(SerializedObject so) {
            #if UNITY_2022_1_OR_NEWER
            // use PlayerSettings as the source of truth 
            return PlayerSettings.insecureHttpOption != InsecureHttpOption.NotAllowed;
            #else
            return GetProperty(so).boolValue;
            #endif
        }

        public override string ObjectPropertyName =>
            nameof(HotReloadSettingsObject.AllowAndroidAppToMakeHttpRequests);

        public override void SetValue(SerializedObject so, bool value) {
            base.SetValue(so, value);

            // Enabling on Unity 2022 or newer → set the Unity option to ‘Development Builds only’
            #if UNITY_2022_1_OR_NEWER
            var notAllowed = PlayerSettings.insecureHttpOption == InsecureHttpOption.NotAllowed;
            if (value) {
                // user chose to enable it
                if (notAllowed) {
                    PlayerSettings.insecureHttpOption = InsecureHttpOption.DevelopmentOnly;
                }
            } else {
                // user chose to disable it
                PlayerSettings.insecureHttpOption = InsecureHttpOption.NotAllowed;
            }
            #endif
        }

        public override void InnerOnGUI(SerializedObject so) {
            var description = "For Hot Reload to work on-device, please allow HTTP requests";
            EditorGUILayout.LabelField(description, HotReloadWindowStyles.WrapStyle);
        }
    }
}