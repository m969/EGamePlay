using UnityEditor;

namespace SingularityGroup.HotReload.Editor {
    internal class IncludeInBuildOption : ProjectOptionBase, ISerializedProjectOption {
        static IncludeInBuildOption _I;
        public static IncludeInBuildOption I = _I ?? (_I = new IncludeInBuildOption());
        public override string ShortSummary => "Include Hot Reload in player builds";
        public override string Summary => ShortSummary;

        public override string ObjectPropertyName =>
            nameof(HotReloadSettingsObject.IncludeInBuild);

        public override void InnerOnGUI(SerializedObject so) {
            string description;
            if (GetValue(so)) {
                description = "The Hot Reload runtime is included in development builds that use the Mono scripting backend.";
            } else {
                description = "The Hot Reload runtime will not be included in any build. Use this option to disable HotReload without removing it from your project.";
            }
            description += " This option does not affect Hot Reload usage in Playmode";
            EditorGUILayout.LabelField(description, HotReloadWindowStyles.WrapStyle);
        }
    }
}