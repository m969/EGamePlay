using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SingularityGroup.HotReload.DTO;
using SingularityGroup.HotReload.Editor.Cli;
using SingularityGroup.HotReload.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    internal class ServerDownloader : IProgress<float> {
        public float Progress {get; private set;}
        public bool Started {get; private set;}

        class Config {
            public Dictionary<string, string> customServerExecutables;
        }
        
        public string GetExecutablePath(ICliController cliController) {
            var targetDir = CliUtils.GetExecutableTargetDir();
            var targetPath = Path.Combine(targetDir, cliController.BinaryFileName);
            return targetPath;
        }
        
        public bool IsDownloaded(ICliController cliController) {
            return File.Exists(GetExecutablePath(cliController));
        }
        
        public bool CheckIfDownloaded(ICliController cliController) {
            if(TryUseUserDefinedBinaryPath(cliController, GetExecutablePath(cliController))) {
                Started = true;
                Progress = 1f;
                return true;
            } else if(IsDownloaded(cliController)) {
                Started = true;
                Progress = 1f;
                return true;
            } else {
                Started = false;
                Progress = 0f;
                return false;
            }
        }
        
        public async Task<bool> EnsureDownloaded(ICliController cliController, CancellationToken cancellationToken) {
            var targetDir = CliUtils.GetExecutableTargetDir();
            var targetPath = Path.Combine(targetDir, cliController.BinaryFileName);
            Started = true;
            if(File.Exists(targetPath)) {
                Progress = 1f;
                return true;
            }
            Progress = 0f;
            await ThreadUtility.SwitchToThreadPool(cancellationToken);

            Directory.CreateDirectory(targetDir);
            if(TryUseUserDefinedBinaryPath(cliController, targetPath)) {
                Progress = 1f;
                return true;
            }

            var tmpPath = CliUtils.GetTempDownloadFilePath("Server.tmp");
            var attempt = 0;
            bool sucess = false;
            HashSet<string> errors = null;
            while(!sucess) {
                try {
                    if (File.Exists(targetPath)) {
                        Progress = 1f;
                        return true;
                    }
                    // Note: we are writing to temp file so if downloaded file is corrupted it will not cause issues until it's copied to target location
                    var result = await DownloadUtility.DownloadFile(GetDownloadUrl(cliController), tmpPath, this, cancellationToken).ConfigureAwait(false);
                    sucess = result.statusCode == HttpStatusCode.OK;
                } catch (Exception e) {
                    var error = $"{e.GetType().Name}: {e.Message}";
                    errors = (errors ?? new HashSet<string>());
                    if (errors.Add(error)) {
                        Log.Warning($"Download attempt failed. If the issue persists please reach out to customer support for assistance. Exception: {error}");
                    }
                }
                if (!sucess) {
                    await Task.Delay(ExponentialBackoff.GetTimeout(attempt), cancellationToken).ConfigureAwait(false);
                }
                Progress = 0;
                attempt++;
            }
            
            if (errors?.Count > 0) {
                var data = new EditorExtraData {
                    { StatKey.Errors, new List<string>(errors) },
                };
                // sending telemetry requires server to be running so we only attempt after server is downloaded
                RequestHelper.RequestEditorEventWithRetry(new Stat(StatSource.Client, StatLevel.Error, StatFeature.Editor, StatEventType.Download), data).Forget();
                Log.Info("Download succeeded!");
            }
            
            const int ERROR_ALREADY_EXISTS = 0xB7;
            try {
                File.Move(tmpPath, targetPath);
            } catch(IOException ex) when((ex.HResult & 0x0000FFFF) == ERROR_ALREADY_EXISTS) {
                //another downloader came first
                try {
                    File.Delete(tmpPath); 
                } catch {
                    //ignored 
                }
            }
            Progress = 1f;
            return true;
        }

        static bool TryUseUserDefinedBinaryPath(ICliController cliController, string targetPath) {
            if (!File.Exists(PackageConst.ConfigFileName)) {
                return false;
            } 
            
            var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(PackageConst.ConfigFileName));
            var customExecutables = config?.customServerExecutables;
            if (customExecutables == null) {
                return false;
            }

            string customBinaryPath;
            if(!customExecutables.TryGetValue(cliController.PlatformName, out customBinaryPath)) {
                return false;
            }
            
            if (!File.Exists(customBinaryPath)) {
                Log.Warning($"unable to find server binary for platform '{cliController.PlatformName}' at '{customBinaryPath}'. " +
                            $"Will proceed with downloading the binary (default behavior)");
                return false;
            } 
            
            try {
                var targetFile = new FileInfo(targetPath);
                bool copy = true;
                if (targetFile.Exists) {
                    copy = File.GetLastWriteTimeUtc(customBinaryPath) > targetFile.LastWriteTimeUtc;
                }
                if (copy) {
                    Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                    File.Copy(customBinaryPath, targetPath, true);
                }
                return true;
            } catch(IOException ex) {
                Log.Warning("encountered exception when copying server binary in the specified custom executable path '{0}':\n{1}", customBinaryPath, ex);
                return false;
            }
        }

        static string GetDownloadUrl(ICliController cliController) {
            const string version = PackageConst.ServerVersion;
            var key = $"{DownloadUtility.GetPackagePrefix(version)}/server/{cliController.PlatformName}/{cliController.BinaryFileName}";
            return DownloadUtility.GetDownloadUrl(key);
        }

        void IProgress<float>.Report(float value) {
            Progress = value;
        }
        
        public Task<bool> PromptForDownload() {
            if (EditorUtility.DisplayDialog(
                title: "Install platform specific components",
                message: InstallDescription,
                ok: "Install",
                cancel: "More Info")
            ) {
                return EnsureDownloaded(HotReloadCli.controller, CancellationToken.None);
            }
            Application.OpenURL(Constants.AdditionalContentURL);
            return Task.FromResult(false);
        }
        
        public const string InstallDescription = "For Hot Reload to work, additional components specific to your operating system have to be installed";
    }
    
    class DownloadResult {
        public readonly HttpStatusCode statusCode;
        public readonly string error;
        public DownloadResult(HttpStatusCode statusCode, string error) {
            this.statusCode = statusCode;
            this.error = error;
        }
    }
}
