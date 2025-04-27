using UnityEditor;

namespace SingularityGroup.HotReload.Editor {
    public interface IOption {
        string ShortSummary { get; }
        string Summary { get; }

        /// <param name="so">The <see cref="HotReloadSettingsObject"/> wrapped by SerializedObject</param>
        bool GetValue(SerializedObject so);

        /// <summary>
        /// Handle the new value.
        /// </summary>
        /// <remarks>
        /// Note: caller must skip calling this if value same as GetValue! 
        /// </remarks>
        /// <param name="so">The <see cref="HotReloadSettingsObject"/> wrapped by SerializedObject</param>
        /// <param name="value"></param>
        void SetValue(SerializedObject so, bool value);

        /// <param name="so">The <see cref="HotReloadSettingsObject"/> wrapped by SerializedObject</param>
        void InnerOnGUI(SerializedObject so);
    }

    /// <summary>
    /// An option scoped to the current Unity project.
    /// </summary>
    /// <remarks>
    /// These options are intended to be shared with collaborators and used by Unity Player builds.
    /// </remarks>
    public interface ISerializedProjectOption {
        string ObjectPropertyName { get; }
    }
}