using UnityEditor;

namespace SingularityGroup.HotReload.Editor {
    /// <summary>
    /// An option stored inside the current Unity project.
    /// </summary>
    internal abstract class ProjectOptionBase : IOption, ISerializedProjectOption {
        public abstract string ShortSummary { get; }
        public abstract string Summary { get; }

        public virtual bool GetValue(SerializedObject so) {
            return so.FindProperty(ObjectPropertyName).boolValue;
        }

        protected SerializedProperty GetProperty(SerializedObject so) {
            return so.FindProperty(ObjectPropertyName);
        }
        
        public virtual void SetValue(SerializedObject so, bool value) {
            so.FindProperty(ObjectPropertyName).boolValue = value;
        }

        public virtual void InnerOnGUI(SerializedObject so) { }

        public abstract string ObjectPropertyName { get; }

        /// <remarks>
        /// Override this if your option is not needed for on-device Hot Reload to work.<br/>
        /// (by default, a project option must be true for Hot Reload to work)
        /// </remarks>
        public virtual bool IsRequiredForBuild() {
            return true;
        }
    }

    /// <summary>
    /// An option that is stored on the user's computer (shared between Unity projects).
    /// </summary>
    internal abstract class ComputerOptionBase : IOption {
        public abstract string ShortSummary { get; }
        public abstract string Summary { get; }

        public abstract bool GetValue();
        
        /// Uses <see cref="HotReloadPrefs"/> for storing the value on the user's computer.
        public virtual void SetValue(bool value) { }

        public bool GetValue(SerializedObject so) => GetValue();

        public virtual void SetValue(SerializedObject so, bool value) => SetValue(value);

        void IOption.InnerOnGUI(SerializedObject so) {
            InnerOnGUI();
        }
        public virtual void InnerOnGUI() { }
    }
}
