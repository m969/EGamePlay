#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace SingularityGroup.HotReload {
    internal class RetryDialog : MonoBehaviour {
        [Header("UI controls")]
        public Button buttonHide;
        public Button buttonRetryAutoPair;
        public Button buttonTroubleshoot;

        public Text textSummary;
        public Text textSuggestion;
        public InputField ipInput;
        
        [Tooltip("Hidden by default")]
        public Text textForDebugging;
        
        [Header("For HotReload Devs")]
        // In Unity Editor, click checkbox to see info helpful for debugging bugs
        public bool enableDebugging;

        // [Header("Other")]
        // [Tooltip("Used when your project does not create an EventSystem early enough")]
        // public GameObject fallbackEventSystem;

        private static RetryDialog _I;
        
        public string DebugInfo {
            set {
                textForDebugging.text = value;
            }
        }

        public bool autoConnect { get; set; }

        void Start() {
            buttonHide.onClick.AddListener(() => {
                Hide();   
            });
            
            buttonRetryAutoPair.onClick.AddListener(() => {
                Hide();
                int port;
                var ipAndPort = ipInput.textComponent.text.Split(':');
                if (ipAndPort.Length != 2 || !int.TryParse(ipAndPort[1], out port)) {
                    port = PlayerEntrypoint.PlayerBuildInfo?.buildMachinePort ?? RequestHelper.defaultPort;
                }
                var ip = ipAndPort.Length > 0 ? ipAndPort[0] : string.Empty;
                PlayerEntrypoint.TryConnectToIpAndPort(ip, port);
            });
            
            buttonTroubleshoot.onClick.AddListener(() => {
                Application.OpenURL("https://hotreload.net/documentation#connection-issues");
            });
        }

        [CanBeNull]
        public static PatchServerInfo TargetServer { private get; set; } = null;
        public static ServerHandshake.Result HandshakeResults { private get; set; } = ServerHandshake.Result.None;

        private void OnEnable() {
            ipInput.text = $"{PlayerEntrypoint.PlayerBuildInfo?.buildMachineHostName}:{PlayerEntrypoint.PlayerBuildInfo?.buildMachinePort}";
            UpdateUI();
        }

        void Update() {
            UpdateUI();
        }
            
        void UpdateUI() {
            // assumes that auto-pair already tried for several seconds
            // suggestions to help the user when auto-pair is failing
            var networkText = Application.isMobilePlatform ? "WiFi" : "LAN/WiFi";
            var noWifiNetwork = $"Is this device connected to {networkText}?";
            var waitForCompiling = "Wait for compiling to finish before trying again";
            var targetNetworkIsReachable = $"Make sure you're on the same {networkText} network. Also ensure Hot Reload is running";

            if (Application.internetReachability != NetworkReachability.ReachableViaLocalAreaNetwork) {
                textSuggestion.text = noWifiNetwork;
            } else if (HandshakeResults.HasFlag(ServerHandshake.Result.WaitForCompiling)) {
                // Note: Technically the player could do the waiting itself, and handshake again with the server
                // only after compiling finishes... Telling the user to do that is easier to implement though.
                textSuggestion.text = waitForCompiling;
            } else {
                textSuggestion.text = targetNetworkIsReachable;
            }

            textSummary.text = autoConnect ? "Auto-pair encountered an issue" : "Connection failed";

            if (enableDebugging && textForDebugging) {
                textForDebugging.enabled = true;
                textForDebugging.text = $"the target = {TargetServer}";
            }
        }

        /// hide this dialog
        void Hide() {
            gameObject.SetActive(false); // this should disable the Update loop?
        }
    }
}
#endif
