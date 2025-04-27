using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SingularityGroup.HotReload.Editor.Semver;
using SingularityGroup.HotReload.Newtonsoft.Json;
using SingularityGroup.HotReload.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace SingularityGroup.HotReload.Editor {
    internal class PackageUpdateChecker {
        const string persistedFile = PackageConst.LibraryCachePath + "/updateChecker.json";
        readonly JsonSerializer jsonSerializer = JsonSerializer.CreateDefault();
        SemVersion newVersionDetected;
        bool started;
        bool warnedVersionCheckFailed;

        private static TimeSpan RetryInterval => TimeSpan.FromSeconds(30);
        private static TimeSpan CheckInterval => TimeSpan.FromHours(1);
        
        private static readonly HttpClient client = HttpClientUtils.CreateHttpClient();

        private static string _lastRemotePackageVersion;

        public static string lastRemotePackageVersion => _lastRemotePackageVersion;

        public async void StartCheckingForNewVersion() {
            if(started) {
                return;
            }
            started = true;
            
            for (;;) {
                try {
                    await PerformVersionCheck();
                    if(newVersionDetected != null) {
                        break;
                    }
                } catch(Exception ex) {
                    Log.Warning("encountered exception when checking for new Hot Reload package version:\n{0}", ex);
                }
                await Task.Delay(RetryInterval);
            }
        }

        public bool TryGetNewVersion(out SemVersion version) {
            var currentVersion = SemVersion.Parse(PackageConst.Version, strict: true);
            return !ReferenceEquals(version = newVersionDetected, null) && newVersionDetected > currentVersion;
        }
        
        async Task PerformVersionCheck() { 
            var state = await LoadPersistedState();
            var currentVersion = SemVersion.Parse(PackageConst.Version, strict: true);
            if(state != null) {
                _lastRemotePackageVersion = state.lastRemotePackageVersion;
                var newVersion = SemVersion.Parse(state.lastRemotePackageVersion);
                if(newVersion > currentVersion) {
                    newVersionDetected = newVersion;
                    return;
                }
                if(DateTime.UtcNow - state.lastVersionCheck < CheckInterval) {
                    return;
                }
            }
            
            var response = await GetLatestPackageVersion();
            if(response.err != null) {
                if(response.statusCode == 0 || response.statusCode == 404) {
                    // probably no internet, fail silently and retry
                } else if (!warnedVersionCheckFailed) {
                    Log.Warning("version check failed: {0}", response.err);
                    warnedVersionCheckFailed = true;
                }
            } else {
                var newVersion = response.data;
                if (response.data > currentVersion) {
                    newVersionDetected = newVersion;
                }
                await Task.Run(() => PersistState(response.data));
            }
        }

        void PersistState(SemVersion newVersion) {
            // ReSharper disable once AssignNullToNotNullAttribute
            var fi = new FileInfo(persistedFile);
            fi.Directory.Create();
            using (var streamWriter = new StreamWriter(fi.OpenWrite()))
            using (var writer = new JsonTextWriter(streamWriter)) {
                jsonSerializer.Serialize(writer, new State {
                    lastVersionCheck = DateTime.UtcNow,
                    lastRemotePackageVersion = newVersion.ToString()
                });
            }
        }
        
        Task<State> LoadPersistedState() {
            return Task.Run(() => {
                var fi = new FileInfo(persistedFile);
                if(!fi.Exists) {
                    return null;
                }
                
                using(var streamReader = fi.OpenText())
                using(var reader = new JsonTextReader(streamReader)) {
                    return jsonSerializer.Deserialize<State>(reader);
                }
            });
        }
        


        static async Task<Response<SemVersion>> GetLatestPackageVersion() {
            string versionUrl;
            
            if (PackageConst.IsAssetStoreBuild) {
                // version updates are synced with asset store
                versionUrl = "https://d2tc55zjhw51ly.cloudfront.net/releases/latest/asset-store-version.json";
            } else {
                versionUrl = "https://gitlab.hotreload.net/root/hot-reload-releases/-/raw/production/package.json";
            }
            try {
                using(var resp = await client.GetAsync(versionUrl).ConfigureAwait(false)) {
                    if(resp.StatusCode != HttpStatusCode.OK) {
                        return Response.FromError<SemVersion>($"Request failed with statusCode: {resp.StatusCode} {resp.ReasonPhrase}");
                    }
                    
                    var json = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var o = await JObject.LoadAsync(new JsonTextReader(new StringReader(json))).ConfigureAwait(false);
                    SemVersion newVersion;
                    JToken value;
                    if (!o.TryGetValue("version", out value)) {
                        return Response.FromError<SemVersion>("Invalid package.json");
                    } else if(!SemVersion.TryParse(value.Value<string>(), out newVersion, strict: true)) {
                        return Response.FromError<SemVersion>($"Invalid version in package.json: '{value.Value<string>()}'");
                    } else {
                        return Response.FromResult(newVersion);
                    }
                }
            } catch(Exception ex) {
                return Response.FromError<SemVersion>($"{ex.GetType().Name} {ex.Message}");
            }
        }
        
        public async Task UpdatePackageAsync(SemVersion newVersion) {
            //Package can be updated by updating the git url via the package manager
            if(EditorUtility.DisplayDialog($"Update To v{newVersion}", $"By pressing 'Update' the Hot Reload package will be updated to v{newVersion}", "Update", "Cancel")) {
                var resp = await GetLatestPackageVersion();
                if(resp.err == null && resp.data > newVersion) {
                    newVersion = resp.data;
                }
                
                if(await IsUsingGitRepo()) {
                    var err = UpdateGitUrlInManifest(newVersion);
                    if(err != null) {
                        Log.Warning("Encountered issue when updating Hot Reload: {0}", err);
                    } else {
                        //Delete state to force another version check after the package is installed
                        File.Delete(persistedFile);
                        #if UNITY_2020_3_OR_NEWER
                        UnityEditor.PackageManager.Client.Resolve();
                        #else
                        CompileMethodDetourer.Reset();
                        AssetDatabase.Refresh();
                        #endif
                    }
                } else {
                    var err = await UpdateUtility.Update(newVersion.ToString(), null, CancellationToken.None);
                    if(err != null) {
                        Log.Warning("Failed to update package: {0}", err);
                    } else {
                        CompileMethodDetourer.Reset();
                        AssetDatabase.Refresh();
                    }
                }
                
                //open changelog
                HotReloadPrefs.ShowChangeLog = true;
                HotReloadWindow.Current.SelectTab(typeof(HotReloadAboutTab));
            }
        }
        
        string UpdateGitUrlInManifest(SemVersion newVersion) {
            const string repoUrl = "git+https://gitlab.hotreload.net/root/hot-reload-releases.git";
            const string manifestJsonPath = "Packages/manifest.json";
            var repoUrlToNewVersion = $"{repoUrl}#{newVersion}";
            if(!File.Exists(manifestJsonPath)) {
                return "Unable to find manifest.json";
            }
            
            var root = JObject.Load(new JsonTextReader(new StringReader(File.ReadAllText(manifestJsonPath))));
            JObject deps;
            var err = TryGetManfestDeps(root, out deps);
            if(err != null) {
                return err;
            }
            deps[PackageConst.PackageName] = repoUrlToNewVersion;
            root["dependencies"] = deps;
            File.WriteAllText(manifestJsonPath, root.ToString(Formatting.Indented));
            return null;
        }
        
        static string TryGetManfestDeps(JObject root, out JObject deps) {
            JToken value;
            if(!root.TryGetValue("dependencies", out value)) {
                deps = null;
                return "no dependencies object found in manifest.json";
            }
            deps = value.Value<JObject>();
            if(deps == null) {
                return "dependencies object null in manifest.json";
            }
            return null;
        }

        static async Task<bool> IsUsingGitRepo() {
            var respose = await Task.Run(() => IsUsingGitRepoThreaded(PackageConst.PackageName));
            if(respose.err != null) {
                Log.Warning("Unable to find package. message: {0}", respose.err);
                return false;
            } else {
                return respose.data;
            }
        }
        
        static Response<bool> IsUsingGitRepoThreaded(string packageId) {
            var fi = new FileInfo("Packages/manifest.json");
            if(!fi.Exists) {
                return "Unable to find manifest.json";
            }
            
            using(var reader = fi.OpenText()) {
                var root = JObject.Load(new JsonTextReader(reader));
                JObject deps;
                var err = TryGetManfestDeps(root, out deps);
                if(err != null) {
                    return "no dependencies specified in manifest.json";
                }
                JToken value;
                if(!deps.TryGetValue(packageId, out value)) {
                    //Likely a local package directly in the packages folder of the unity project
                    //or the package got moved into the Assets folder
                    return Response.FromResult(false);
                }
                var pathToPackage = value.Value<string>();
                if(pathToPackage.StartsWith("git+", StringComparison.Ordinal)) {
                    return Response.FromResult(true);
                }
                if(pathToPackage.StartsWith("https://", StringComparison.Ordinal)) {
                    return Response.FromResult(true);
                }
                return Response.FromResult(false);
            }
        }

        class Response<T> {
            public readonly T data;
            public readonly string err;
            public readonly long statusCode;
            public Response(T data, string err, long statusCode) {
                this.data = data;
                this.err = err;
                this.statusCode = statusCode;
            }
            
            public static implicit operator Response<T>( string err) {
                return Response.FromError<T>(err);
            }
        }
        
        static class Response {
            public static Response<T> FromError<T>(string error) {
                return new Response<T>(default(T), error, -1);
            }
            public static Response<T> FromResult<T>(T result) {
                return new Response<T>(result, null, 200);
            }
        }
        
        class State {
            public DateTime lastVersionCheck;
            public string lastRemotePackageVersion;
        }
    }
    
    
}