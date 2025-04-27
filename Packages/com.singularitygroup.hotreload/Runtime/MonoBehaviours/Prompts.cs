#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SingularityGroup.HotReload {
    internal class Prompts : MonoBehaviour {
        public GameObject retryPrompt;
        public GameObject connectedPrompt;
        public GameObject questionPrompt;
        
        [Header("Other")]
        [Tooltip("Used when project does not create an EventSystem early enough")]
        public GameObject fallbackEventSystem;
        
        #region Singleton
        
        private static Prompts _I;

        /// <summary>
        /// All usages must check that <see cref="PlayerEntrypoint.RuntimeSupportsHotReload"/> is true before accessing this singleton.
        /// </summary>
        /// <remarks>
        /// This getter can throw on unsupported platforms (HotReloadSettingsObject resource doesn't exist on unsupported platforms).
        /// </remarks>
        public static Prompts I {
            get {
                if (_I == null) {
                    // allow showing prompts in editor (for testing)
                    if (!Application.isEditor && !PlayerEntrypoint.IsPlayerWithHotReload()) {
                        throw new NotSupportedException("IsPlayerWithHotReload() is false");
                    }
                    var go = Instantiate(HotReloadSettingsObject.I.PromptsPrefab,
                        new Vector3(0, 0, 0), Quaternion.identity);
                    go.name = nameof(Prompts) + "_singleton";
                    if (Application.isPlaying) {
                        DontDestroyOnLoad(go);
                    }

                    _I = go.GetComponentInChildren<Prompts>();
                }

                return _I;
            }
        }
        #endregion

        /// <seealso cref="ShowConnectionDialog"/>
        public static void SetConnectionState(string state, bool log = true) {
            var connectionDialog = I.connectedPrompt.GetComponentInChildren<ConnectionDialog>();
            if (log) Log.Debug($"SetConnectionState( {state} )");
            if (connectionDialog) {
                connectionDialog.SetSummary(state);
            }
        }

        /// <seealso cref="SetConnectionState"/>
        public static void ShowConnectionDialog() {
            I.retryPrompt.SetActive(false);
            I.connectedPrompt.SetActive(true);
        }

        public static async Task<bool> ShowQuestionDialog(QuestionDialog.Config config) {
            var tcs = new TaskCompletionSource<bool>();
            var holder = I.questionPrompt;
            var dialog = holder.GetComponentInChildren<QuestionDialog>();
            dialog.completion = tcs;
            dialog.UpdateView(config);
            holder.SetActive(true);
            return await tcs.Task;
        }

        public static void ShowRetryDialog(
            PatchServerInfo patchServerInfo,
            ServerHandshake.Result handshakeResults = ServerHandshake.Result.None,
            bool auto = true
        ) {
            
            var retryDialog = I.retryPrompt.GetComponentInChildren<RetryDialog>();
            
            RetryDialog.TargetServer = patchServerInfo;
            RetryDialog.HandshakeResults = handshakeResults;
            
            if (patchServerInfo == null) {
                retryDialog.DebugInfo = $"patchServerInfo == null  {handshakeResults}";
            } else {
                retryDialog.DebugInfo = $"{RequestHelper.CreateUrl(patchServerInfo)} {handshakeResults}";
            }
            retryDialog.autoConnect = auto;

            I.connectedPrompt.SetActive(false);
            I.retryPrompt.SetActive(true);
        }

        #region fallback event system

        private void Start() {
            StartCoroutine(DelayedEnsureEventSystem());
        }

        private bool userTriedToInteract = false;

        private void Update() {
            if (!userTriedToInteract) {
                // when user interacts with the screen, make sure overlay can handle taps
#if ENABLE_INPUT_SYSTEM
                if ((Touchscreen.current != null && Touchscreen.current.touches.Count > 0) || 
                    (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)) {
                    userTriedToInteract = true;
                    DoEnsureEventSystem();
                }
#else
                if (Input.touchCount > 0 || Input.GetMouseButtonDown(0)) {
                    userTriedToInteract = true;
                    DoEnsureEventSystem();
                }
#endif
            }
        }

        private IEnumerator DelayedEnsureEventSystem() {
            // allow some delay in-case the project loads the EventSystem asynchronously (perhaps in a second scene)
            if (EventSystem.current == null) {
                yield return new WaitForSeconds(1f);
                DoEnsureEventSystem();
            }
        }

        /// Scene must contain an EventSystem and StandaloneInputModule, otherwise clicking/tapping on the overlay does nothing.
        private void DoEnsureEventSystem() {
            if (EventSystem.current == null) {
                Log.Info($"No EventSystem is active, enabling an EventSystem inside Hot Reload {name} prefab." +
                    " A Unity EventSystem and an Input module is required for tapping buttons on the Unity UI.");
                fallbackEventSystem.SetActive(true);
            }
        }
        #endregion
    }
}
#endif
