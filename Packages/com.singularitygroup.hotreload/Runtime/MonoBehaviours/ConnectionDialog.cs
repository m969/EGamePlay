#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using UnityEngine;
using UnityEngine.UI;

namespace SingularityGroup.HotReload {
    internal class ConnectionDialog : MonoBehaviour {
        [Header("UI controls")]
        public Button buttonHide;

        [Header("Information")]
        public Text textSummary;
        public Text textSuggestion;
        
        void Start() {
            buttonHide.onClick.AddListener(Hide);
        }

        public int pendingPatches = 0;
        public int patchesApplied = 0;

        private void Awake() {
            SyncPatchCounts();
        }

        bool SyncPatchCounts() {
            var changed = false;
            if (pendingPatches != CodePatcher.I.PendingPatches.Count) {
                pendingPatches = CodePatcher.I.PendingPatches.Count;
                changed = true;
            }

            if (patchesApplied != CodePatcher.I.PatchesApplied) {
                patchesApplied = CodePatcher.I.PatchesApplied;
                changed = true;
            }

            return changed;
        }

        /// <param name="summary">One of the <see cref="ConnectionSummary"/> constants</param>
        public void SetSummary(string summary = ConnectionSummary.Connected) {
            if (textSummary != null) textSummary.text = summary;
            isConnected = summary == ConnectionSummary.Connected;
        }

        private bool isConnected = false;

        // assumes that auto-pair already tried for several seconds
        void Update() {
            textSuggestion.enabled = isConnected;
            if (SyncPatchCounts()) {
                textSuggestion.text = $"Patches: {pendingPatches} pending, {patchesApplied} applied";
            }
        }

        /// hide this dialog
        void Hide() {
            gameObject.SetActive(false); // this should disable the Update loop?
        }
    }

    /// <summary>
    /// The connection between device and Hot Reload can be summarized in a few words.
    /// </summary>
    /// <remarks>
    /// The summary may be shown for less than a second, as the connection can change without warning.<br/>
    /// Therefore, we use short and simple messages.
    /// </remarks>
    internal static class ConnectionSummary {
        public const string Cancelled = "Cancelled";
        public const string Connecting = "Connecting ...";
        public const string Handshaking = "Handshaking ...";
        public const string DifferencesFound = "Differences found";
        public const string Connected = "Connected!";
        // reconnecting can be shown for a long time, so a longer message is okay
        public const string TryingToReconnect = "Trying to reconnect ...";
        public const string Disconnected = "Disconnected";
    }
}
#endif
