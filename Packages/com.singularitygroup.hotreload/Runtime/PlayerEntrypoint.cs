#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
#if UNITY_ANDROID && !UNITY_EDITOR
#define MOBILE_ANDROID
#endif
#if UNITY_IOS && !UNITY_EDITOR
#define MOBILE_IOS
#endif
#if MOBILE_ANDROID || MOBILE_IOS
#define MOBILE
#endif

using System;
using System.Threading.Tasks;
#if MOBILE_ANDROID
// not able to use File apis for reading from StreamingAssets
using UnityEngine.Networking;
#endif
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.IO;

namespace SingularityGroup.HotReload {
    // entrypoint for Unity Player builds. Not necessary in Unity Editor.
    internal static class PlayerEntrypoint {
        /// Set when behaviour is created, when you access this instance through the singleton,
        /// you can assume that this field is not null.
        /// <remarks>
        /// In Player code you can assume this is set.<br/>
        /// When in Editor this is usually null.
        /// </remarks>
        static BuildInfo buildInfo { get; set; }

        /// In Player code you can assume this is set (not null)
        public static BuildInfo PlayerBuildInfo => buildInfo;

        #if ENABLE_MONO
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        #endif
        private static void InitOnAppLoad() {
            AppCallbackListener.Init(); // any platform might be using this
            UnityHelper.Init();
            bool onlyPrefabMissing;
            if (!IsPlayerWithHotReload(out onlyPrefabMissing)) {
                if (onlyPrefabMissing) {
                    Log.Warning("Hot Reload is not available in this build because one or more build settings were not supported.");
                }
                return;
            }

            TryAutoConnect().Forget();
        }

        static async Task TryAutoConnect() {
            try {
                buildInfo = await GetBuildInfo();
            } catch (Exception e) {
                if (e is IOException) {
                    Log.Warning("Hot Reload is not available in this build because one or more build settings were not supported.");
                } else {
                    Log.Error($"Uknown exception happened when reading build info\n{e.GetType().Name}: {e.Message}");
                }
                return;
            }
            if (buildInfo == null) {
                Log.Error($"Uknown issue happened when reading build info.");
                return;
            }

            try {
                var customIp = PlayerPrefs.GetString("HotReloadRuntime.CustomIP", "");
                if (!string.IsNullOrEmpty(customIp)) {
                    buildInfo.buildMachineHostName = customIp;
                }
                var customPort = PlayerPrefs.GetString("HotReloadRuntime.CustomPort", "");
                if (!string.IsNullOrEmpty(customPort)) {
                    buildInfo.buildMachinePort = int.Parse(customPort);
                }

                if (buildInfo.BuildMachineServer == null) {
                    Prompts.ShowRetryDialog(null);
                } else {
                    // try reach server running on the build machine.
                    TryConnect(buildInfo.BuildMachineServer, auto: true).Forget();
                }
            } catch (Exception ex) {
                Log.Exception(ex);
            }
        }

        public static Task TryConnectToIpAndPort(string ip, int port) {
            ip = ip.Trim();
            if (buildInfo == null) {
                throw new ArgumentException("Build info not found");
            }
            buildInfo.buildMachineHostName = ip;
            buildInfo.buildMachinePort = port;
            PlayerPrefs.SetString("HotReloadRuntime.CustomIP", ip);
            PlayerPrefs.SetString("HotReloadRuntime.CustomPort", port.ToString());
            return TryConnect(buildInfo.BuildMachineServer, auto: false);
        }

        public static async Task TryConnect(PatchServerInfo serverInfo, bool auto) {
            // try reach server running on the build machine.
            var handshake = PlayerCodePatcher.UpdateHost(serverInfo);
            await Task.WhenAny(handshake, Task.Delay(TimeSpan.FromSeconds(40)));
            await ThreadUtility.SwitchToMainThread();
            var handshakeResults = await handshake;
            var handshakeOk = handshakeResults.HasFlag(ServerHandshake.Result.Verified);
            if (!handshakeOk) {
                Log.Debug("ShowRetryPrompt because handshake result is {0}", handshakeResults);
                Prompts.ShowRetryDialog(serverInfo, handshakeResults, auto);
                // cancel trying to connect. They can use the retry button
                PlayerCodePatcher.UpdateHost(null).Forget();
            }

            Log.Info($"Server is healthy after first handshake? {handshakeOk}");
        }

        /// on Android, streaming assets are inside apk zip, which can only be read using unity web request
        private static async Task<BuildInfo> GetBuildInfo() {
            var path = BuildInfo.GetStoredPath();
            #if MOBILE_ANDROID
            var json = await RequestHelper.GetAsync(path);
            return await Task.Run(() => BuildInfo.FromJson(json));
            #else
            return await Task.Run(() => {
                return BuildInfo.FromJson(File.ReadAllText(path));
            });
            #endif
        }

        public static bool IsPlayer() => !Application.isEditor;

        public static bool IsPlayerWithHotReload() {
            bool _;
            return IsPlayerWithHotReload(out _);
        }

        public static bool IsPlayerWithHotReload(out bool onlyPrefabMissing) {
            onlyPrefabMissing = false;
            if (!IsPlayer() || !RuntimeSupportsHotReload || !HotReloadSettingsObject.I.IncludeInBuild) {
                return false;
            }
            onlyPrefabMissing = !HotReloadSettingsObject.I.PromptsPrefab;
            return !onlyPrefabMissing;
        }
        
        public static bool RuntimeSupportsHotReload {
            get {
                #if DEVELOPMENT_BUILD && ENABLE_MONO
                return true;
                #else
                return false;
                #endif
            }
        }
    }
}
#endif
