using UnityEditor;

namespace SingularityGroup.HotReload.Editor {
    internal static class HotReloadState {
        private const string ServerPortKey = "HotReloadWindow.ServerPort";
        private const string LastPatchIdKey = "HotReloadWindow.LastPatchId";
        private const string ShowingRedDotKey = "HotReloadWindow.ShowingRedDot";
        private const string ShowedEditorsWithoutHRKey = "HotReloadWindow.ShowedEditorWithoutHR";
        private const string ShowedFieldInitializerWithSideEffectsKey = "HotReloadWindow.ShowedFieldInitializerWithSideEffects";
        private const string ShowedAddMonobehaviourMethodsKey = "HotReloadWindow.ShowedAddMonobehaviourMethods";
        private const string ShowedFieldInitializerExistingInstancesEditedKey = "HotReloadWindow.ShowedFieldInitializerExistingInstancesEdited";
        private const string ShowedFieldInitializerExistingInstancesUneditedKey = "HotReloadWindow.ShowedFieldInitializerExistingInstancesUnedited";
        private const string RecompiledUnsupportedChangesOnExitPlaymodeKey = "HotReloadWindow.RecompiledUnsupportedChangesOnExitPlaymode";
        private const string RecompiledUnsupportedChangesInPlaymodeKey = "HotReloadWindow.RecompiledUnsupportedChangesInPlaymode";
        private const string EditorCodePatcherInitKey = "HotReloadWindow.EditorCodePatcherInit";
        private const string ShowedDebuggerCompatibilityKey = "HotReloadWindow.ShowedDebuggerCompatibility";
        

        public static int ServerPort {
            get { return SessionState.GetInt(ServerPortKey, RequestHelper.defaultPort); }
            set { SessionState.SetInt(ServerPortKey, value); }
        }
        
        public static string LastPatchId {
            get { return SessionState.GetString(LastPatchIdKey, string.Empty); }
            set { SessionState.SetString(LastPatchIdKey, value); }
        }
        
        public static bool ShowingRedDot {
            get { return SessionState.GetBool(ShowingRedDotKey, false); }
            set { SessionState.SetBool(ShowingRedDotKey, value); }
        }
        
        public static bool ShowedEditorsWithoutHR {
            get { return SessionState.GetBool(ShowedEditorsWithoutHRKey, false); }
            set { SessionState.SetBool(ShowedEditorsWithoutHRKey, value); }
        }
        
        public static bool ShowedFieldInitializerWithSideEffects {
            get { return SessionState.GetBool(ShowedFieldInitializerWithSideEffectsKey, false); }
            set { SessionState.SetBool(ShowedFieldInitializerWithSideEffectsKey, value); }
        }
        
        public static bool ShowedAddMonobehaviourMethods {
            get { return SessionState.GetBool(ShowedAddMonobehaviourMethodsKey, false); }
            set { SessionState.SetBool(ShowedAddMonobehaviourMethodsKey, value); }
        }
        
        public static bool ShowedFieldInitializerExistingInstancesEdited {
            get { return SessionState.GetBool(ShowedFieldInitializerExistingInstancesEditedKey, false); }
            set { SessionState.SetBool(ShowedFieldInitializerExistingInstancesEditedKey, value); }
        }
        
        public static bool ShowedFieldInitializerExistingInstancesUnedited {
            get { return SessionState.GetBool(ShowedFieldInitializerExistingInstancesUneditedKey, false); }
            set { SessionState.SetBool(ShowedFieldInitializerExistingInstancesUneditedKey, value); }
        }
        
        public static bool RecompiledUnsupportedChangesOnExitPlaymode {
            get { return SessionState.GetBool(RecompiledUnsupportedChangesOnExitPlaymodeKey, false); }
            set { SessionState.SetBool(RecompiledUnsupportedChangesOnExitPlaymodeKey, value); }
        }
        
        public static bool RecompiledUnsupportedChangesInPlaymode {
            get { return SessionState.GetBool(RecompiledUnsupportedChangesInPlaymodeKey, false); }
            set { SessionState.SetBool(RecompiledUnsupportedChangesInPlaymodeKey, value); }
        }
        
        public static bool EditorCodePatcherInit {
            get { return SessionState.GetBool(EditorCodePatcherInitKey, false); }
            set { SessionState.SetBool(EditorCodePatcherInitKey, value); }
        }
        
        public static bool ShowedDebuggerCompatibility {
            get { return SessionState.GetBool(ShowedDebuggerCompatibilityKey, false); }
            set { SessionState.SetBool(ShowedDebuggerCompatibilityKey, value); }
        }
    }

}
