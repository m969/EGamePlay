using System;
using System.Diagnostics;
using System.IO;
using System.Net;
#if UNITY_EDITOR_WIN
using System.Net.NetworkInformation;
#else
using System.Net.Sockets;
#endif
using System.Threading.Tasks;
using SingularityGroup.HotReload.Newtonsoft.Json;
using UnityEditor;

namespace SingularityGroup.HotReload.Editor.Cli {
    [InitializeOnLoad]
    public static class HotReloadCli {
        internal static readonly ICliController controller;
        
        //InitializeOnLoad ensures controller gets initialized on unity thread
        static HotReloadCli() {
            controller =
    #if UNITY_EDITOR_OSX
                new OsxCliController();
    #elif UNITY_EDITOR_LINUX
                new LinuxCliController();
    #elif UNITY_EDITOR_WIN
                new WindowsCliController();
    #else
                new FallbackCliController();
    #endif
        }

        public static bool CanOpenInBackground => controller.CanOpenInBackground;
        
        /// <summary>
        /// Public API: Starts the Hot Reload server. Must be on the main thread
        /// </summary>
        public static Task StartAsync() {
            return StartAsync(
                isReleaseMode: RequestHelper.IsReleaseMode(),
                exposeServerToNetwork: HotReloadPrefs.ExposeServerToLocalNetwork, 
                allAssetChanges: HotReloadPrefs.AllAssetChanges, 
                createNoWindow: HotReloadPrefs.DisableConsoleWindow,
                detailedErrorReporting: !HotReloadPrefs.DisableDetailedErrorReporting
            );
        }
        
        internal static async Task StartAsync(bool exposeServerToNetwork, bool allAssetChanges, bool createNoWindow, bool isReleaseMode, bool detailedErrorReporting, LoginData loginData = null) {
            var port = await Prepare().ConfigureAwait(false);
            await ThreadUtility.SwitchToThreadPool();
            StartArgs args;
            if (TryGetStartArgs(UnityHelper.DataPath, exposeServerToNetwork, allAssetChanges, createNoWindow, isReleaseMode, detailedErrorReporting, loginData, port, out args)) {
                await controller.Start(args);
            }
        }
        
        /// <summary>
        /// Public API: Stops the Hot Reload server
        /// </summary>
        /// <remarks>
        /// This is a no-op in case the server is not running
        /// </remarks>
        public static Task StopAsync() {
            return controller.Stop();
        }
        
        class Config {
#pragma warning disable CS0649
            public bool useBuiltInProjectGeneration;
#pragma warning restore CS0649
        }
        
        static bool TryGetStartArgs(string dataPath, bool exposeServerToNetwork, bool allAssetChanges, bool createNoWindow, bool isReleaseMode, bool detailedErrorReporting, LoginData loginData, int port, out StartArgs args) {
            string serverDir;
            if(!CliUtils.TryFindServerDir(out serverDir)) {
                Log.Warning($"Failed to start the Hot Reload Server. " +
                                 $"Unable to locate the 'Server' directory. " +
                                 $"Make sure the 'Server' directory is " +
                                 $"somewhere in the Assets folder inside a 'HotReload' folder or in the HotReload package");
                args = null;
                return false;
            }
            
            Config config;
            if (File.Exists(PackageConst.ConfigFileName)) {
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(PackageConst.ConfigFileName));
            } else {
                config = new Config();
            }
            var hotReloadTmpDir = CliUtils.GetHotReloadTempDir();
            var cliTempDir = CliUtils.GetCliTempDir();
            // Versioned path so that we only need to extract the binary once. User can have multiple projects
            //  on their machine using different HotReload versions.
            var executableTargetDir = CliUtils.GetExecutableTargetDir();
            Directory.CreateDirectory(executableTargetDir); // ensure exists
            var executableSourceDir = Path.Combine(serverDir, controller.PlatformName);
            var unityProjDir = Path.GetDirectoryName(dataPath);
            string slnPath;
            if (config.useBuiltInProjectGeneration) {
                var info = new DirectoryInfo(Path.GetFullPath("."));
                slnPath = Path.Combine(Path.GetFullPath("."), info.Name + ".sln");
                if (!File.Exists(slnPath)) {
                    Log.Warning($"Failed to start the Hot Reload Server. Cannot find solution file. Please disable \"useBuiltInProjectGeneration\" in settings to enable custom project generation.");
                    args = null;
                    return false;
                }
                Log.Info("Using default project generation. If you encounter any problem with Unity's default project generation consider disabling it to use custom project generation.");
                try {
                    Directory.Delete(ProjectGeneration.ProjectGeneration.tempDir, true);
                } catch(Exception ex) {
                    Log.Exception(ex);
                }
            } else {
                slnPath = ProjectGeneration.ProjectGeneration.GetSolutionFilePath(dataPath);
            }

            if (!File.Exists(slnPath)) {
                Log.Warning($"No .sln file found. Open any c# file to generate it so Hot Reload can work properly");
            }
            
            var searchAssemblies = string.Join(";", CodePatcher.I.GetAssemblySearchPaths());
            var cliArguments = $@"-u ""{unityProjDir}"" -s ""{slnPath}"" -t ""{cliTempDir}"" -a ""{searchAssemblies}"" -ver ""{PackageConst.Version}"" -proc ""{Process.GetCurrentProcess().Id}"" -assets ""{allAssetChanges}"" -p ""{port}"" -r {isReleaseMode} -detailed-error-reporting {detailedErrorReporting}";
            if (loginData != null) {
                cliArguments += $@" -email ""{loginData.email}"" -pass ""{loginData.password}""";
            }
            if (exposeServerToNetwork) {
                // server will listen on local network interface (default is localhost only)
                cliArguments += " -e true";
            }
            args = new StartArgs {
                hotreloadTempDir = hotReloadTmpDir,
                cliTempDir = cliTempDir,
                executableTargetDir = executableTargetDir,
                executableSourceDir = executableSourceDir,
                cliArguments = cliArguments,
                unityProjDir = unityProjDir,
                createNoWindow = createNoWindow,
            };
            return true;
        }
        
        private static int DiscoverFreePort() {
            var maxAttempts = 10;
            for (int attempt = 0; attempt < maxAttempts; attempt++) {
                var port = RequestHelper.defaultPort + attempt;
                if (IsPortInUse(port)) {
                    continue;
                }
                return port;
            }
            // we give up at this point
            return RequestHelper.defaultPort + maxAttempts;
        }
        
        public static bool IsPortInUse(int port) {
        // Note that there is a racecondition that a port gets occupied after checking.
        // However, it will very rare someone will run into this.
#if UNITY_EDITOR_WIN
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] activeTcpListeners = ipGlobalProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endPoint in activeTcpListeners) {
                if (endPoint.Port == port) {
                    return true;
                }
            }

            return false;
#else
            try {
                using (TcpClient tcpClient = new TcpClient()) {
                    tcpClient.Connect(IPAddress.Loopback, port); // Try to connect to the specified port
                    return true;
                }
            } catch (SocketException) {
                return false;
            } catch (Exception e) {
                Log.Exception(e);
                // act as if the port is allocated
                return true;
            }
#endif
        }
        
        
        static async Task<int> Prepare() {
            await ThreadUtility.SwitchToMainThread();
            
            var dataPath = UnityHelper.DataPath;
            await ProjectGeneration.ProjectGeneration.EnsureSlnAndCsprojFiles(dataPath);
            await PrepareBuildInfoAsync();
            PrepareSystemPathsFile();
            
            var port = DiscoverFreePort();
            HotReloadState.ServerPort = port;
            RequestHelper.SetServerPort(port);
            return port;
        }

        static bool didLogWarning;
        internal static async Task PrepareBuildInfoAsync() {
            await ThreadUtility.SwitchToMainThread();
            var buildInfoInput = await BuildInfoHelper.GetGenerateBuildInfoInput();
            await Task.Run(() => {
                try {
                    var buildInfo = BuildInfoHelper.GenerateBuildInfoThreaded(buildInfoInput);
                    PrepareBuildInfo(buildInfo);
                } catch (Exception e) {
                    if (!didLogWarning) {
                        Log.Warning($"Preparing build info failed! On-device functionality might not work. Exception: {e}");
                        didLogWarning = true;
                    } else { 
                        Log.Debug($"Preparing build info failed! On-device functionality might not work. Exception: {e}");
                    }
                }
            });
        }
        
        internal static void PrepareBuildInfo(BuildInfo buildInfo) {
            // When starting server make sure it starts with correct player data state.
            // (this fixes issue where Unity is in background and not sending files state).
            // Always write player data because you can be on any build target and want to connect with a downloaded android build.
            var json = buildInfo.ToJson();
            var cliTempDir = CliUtils.GetCliTempDir();
            Directory.CreateDirectory(cliTempDir);
            File.WriteAllText(Path.Combine(cliTempDir, "playerdata.json"), json);
        }
        
        static void PrepareSystemPathsFile() {
#pragma warning disable CS0618 // obsolete since 2023
            var lvl = PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup);
#pragma warning restore CS0618
#if UNITY_2020_3_OR_NEWER
            var dirs = UnityEditor.Compilation.CompilationPipeline.GetSystemAssemblyDirectories(lvl);
#else
            var t = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Scripting.ScriptCompilation.MonoLibraryHelpers");
            var m = t.GetMethod("GetSystemReferenceDirectories");
            var dirs = m.Invoke(null, new object[] { lvl });
#endif
            Directory.CreateDirectory(PackageConst.LibraryCachePath);
            File.WriteAllText(PackageConst.LibraryCachePath + "/systemAssemblies.json", JsonConvert.SerializeObject(dirs));
        }
    }
}
