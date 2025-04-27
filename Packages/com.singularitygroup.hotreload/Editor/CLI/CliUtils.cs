using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using SingularityGroup.HotReload.Newtonsoft.Json;
using UnityEngine;
using System;

namespace SingularityGroup.HotReload.Editor.Cli {
    internal static class CliUtils {
        static readonly string projectIdentifier = GetProjectIdentifier();

        class Config {
            public bool singleInstance;
        }

        public static string GetProjectIdentifier() {
            if (File.Exists(PackageConst.ConfigFileName)) {
                var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(PackageConst.ConfigFileName));
                if (config.singleInstance) {
                    return null;
                }
            }
            var path = Path.GetDirectoryName(UnityHelper.DataPath);
            var name = new DirectoryInfo(path).Name;
            using (SHA256 sha256 = SHA256.Create()) {
                byte[] inputBytes = Encoding.UTF8.GetBytes(path);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                var hash = BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, 6).ToUpper();
                return $"{name}-{hash}";
            }
        }
        
        public static string GetTempDownloadFilePath(string osxFileName) {
            if (UnityHelper.Platform == RuntimePlatform.OSXEditor) {
                // project specific temp directory that is writeable on MacOS (Path.GetTempPath() wasn't when run through HotReload.app)
                return Path.GetFullPath(PackageConst.LibraryCachePath + $"/HotReloadServerTemp/{osxFileName}");
            } else {
                return Path.GetTempFileName();
            }
        }
        
        public static string GetHotReloadTempDir() {
            if (UnityHelper.Platform == RuntimePlatform.OSXEditor) {
                // project specific temp directory that is writeable on MacOS (Path.GetTempPath() wasn't when run through HotReload.app)
                return Path.GetFullPath(PackageConst.LibraryCachePath + "/HotReloadServerTemp");
            } else {
                if (projectIdentifier != null) {
                    return Path.Combine(Path.GetTempPath(), "HotReloadTemp", projectIdentifier);
                } else {
                    return Path.Combine(Path.GetTempPath(), "HotReloadTemp");
                }
            }
        }
        
        public static string GetAppDataPath() {
#           if (UNITY_EDITOR_OSX)
                var baseDir = "/Users/Shared";
#           elif (UNITY_EDITOR_LINUX)
                var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
#           else
                var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
#endif
            return Path.Combine(baseDir, "singularitygroup-hotreload");
        }
        
        public static string GetExecutableTargetDir() {
            if (PackageConst.IsAssetStoreBuild) {
                return Path.Combine(GetAppDataPath(), "asset-store", $"executables_{PackageConst.ServerVersion.Replace('.', '-')}");
            }
            return Path.Combine(GetAppDataPath(), $"executables_{PackageConst.ServerVersion.Replace('.', '-')}");
        }
        
        public static string GetCliTempDir() {
            return Path.Combine(GetHotReloadTempDir(), "MethodPatches");
        }
        
        public static void Chmod(string targetFile, string flags = "+x") {
            // ReSharper disable once PossibleNullReferenceException
            Process.Start(new ProcessStartInfo("chmod", $"{flags} \"{targetFile}\"") {
                UseShellExecute = false,
            }).WaitForExit(2000);
        }
        
        public static bool TryFindServerDir(out string path) {
            const string serverBasePath = "Packages/com.singularitygroup.hotreload/Server";
            if(Directory.Exists(serverBasePath)) {
                path = Path.GetFullPath(serverBasePath);
                return true;
            }
            
            //Not found in packages. Try to find in assets folder.
            //fast path - this is the expected folder
            const string alternativeExecutablePath = "Assets/HotReload/Server";
            if(Directory.Exists(alternativeExecutablePath)) {
                path = Path.GetFullPath(alternativeExecutablePath);
                return true;
            }
            //slow path - try to find the server directory somewhere in the assets folder
            var candidates = Directory.GetDirectories("Assets", "HotReload", SearchOption.AllDirectories);
            foreach(var candidate in candidates) {
                var serverDir = Path.Combine(candidate, "Server");
                if(Directory.Exists(serverDir)) {
                    path = Path.GetFullPath(serverDir);
                    return true;
                }
            }
            path = null;
            return false;
        }
        
        public static string GetPidFilePath(string hotreloadTempDir) {
            return Path.GetFullPath(Path.Combine(hotreloadTempDir, "server.pid"));
        }
        
        public static void KillLastKnownHotReloadProcess() {
            var pidPath = GetPidFilePath(GetHotReloadTempDir());
            try {
                var pid = int.Parse(File.ReadAllText(pidPath));
                Process.GetProcessById(pid).Kill();
            }
            catch {
                //ignore
            }
            File.Delete(pidPath);
        }
    }
}