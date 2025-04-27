#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SingularityGroup.HotReload.DTO;
using SingularityGroup.HotReload.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

[assembly: InternalsVisibleTo("CodePatcherEditor")]
[assembly: InternalsVisibleTo("TestProject")]
[assembly: InternalsVisibleTo("SingularityGroup.HotReload.IntegrationTests")]
[assembly: InternalsVisibleTo("SingularityGroup.HotReload.EditorTests")]

namespace SingularityGroup.HotReload {
    class HttpResponse {
        public readonly HttpStatusCode statusCode;
        public readonly Exception exception;
        public readonly string responseText;

        public HttpResponse(HttpStatusCode statusCode, Exception exception, string responseText) {
            this.statusCode = statusCode;
            this.exception = exception;
            this.responseText = responseText;
        }
    }
    
    public class ChangelogVersion {
        public string versionNum;
        public List<string> fixes;
        public List<string> improvements;
        public string date;
        public List<string> features;
        public string generalInfo;
    }

    static class RequestHelper {
        internal const ushort defaultPort = 33242;
        internal const string defaultServerHost = "127.0.0.1";
        const string ChangelogURL = "https://d2tc55zjhw51ly.cloudfront.net/releases/latest/changelog.json";
        static readonly string defaultOrigin = Path.GetDirectoryName(UnityHelper.DataPath);
        public static string origin { get; private set; } = defaultOrigin;
        
        static PatchServerInfo serverInfo = new PatchServerInfo(defaultServerHost, null, null);
        public static PatchServerInfo ServerInfo => serverInfo;
        
        static string cachedUrl;
        static string url => cachedUrl ?? (cachedUrl = CreateUrl(serverInfo));
        
        public static int port => serverInfo?.port ?? defaultPort;

        static readonly HttpClient client = CreateHttpClientWithOrigin();
        // separate client for each long polling request
        static readonly HttpClient clientPollPatches = CreateHttpClientWithOrigin();
        static readonly HttpClient clientPollAssets = CreateHttpClientWithOrigin();
        static readonly HttpClient clientPollStatus = CreateHttpClientWithOrigin();
        
        static readonly HttpClient[] allClients = new[] { client, clientPollPatches, clientPollAssets, clientPollStatus };
        
        static HttpClient CreateHttpClientWithOrigin() {
            var httpClient = HttpClientUtils.CreateHttpClient();
            httpClient.DefaultRequestHeaders.Add("origin", Path.GetDirectoryName(UnityHelper.DataPath));

            return httpClient;
        }
        
        /// <summary>
        /// Create url for a hostname and port
        /// </summary>
        internal static string CreateUrl(PatchServerInfo server) {
            return $"http://{server.hostName}:{server.port.ToString()}";
        }
        
        public static void SetServerPort(int port) {
            serverInfo = new PatchServerInfo(serverInfo.hostName, port, serverInfo.commitHash, serverInfo.rootPath);
            cachedUrl = null;
            Log.Debug($"SetServerInfo to {CreateUrl(serverInfo)}");
        }

        public static void SetServerInfo(PatchServerInfo info) {
            if (info != null) Log.Debug($"SetServerInfo to {CreateUrl(info)}");
            serverInfo = info;
            cachedUrl = null;

            if (info?.customRequestOrigin != null) {
                SetOrigin(info.customRequestOrigin);
            }
        }

        // This function is not thread safe but is currently called before the first request is sent so no issue.
        static void SetOrigin(string newOrigin) {
            if (newOrigin == origin) {
                return;
            }
            origin = newOrigin;
            
            foreach (var httpClient in allClients) {
                httpClient.DefaultRequestHeaders.Remove("origin");
                httpClient.DefaultRequestHeaders.Add("origin", newOrigin);
            }
        }

        static string[] assemblySearchPaths;
        public static void ChangeAssemblySearchPaths(string[] paths) {
            assemblySearchPaths = paths;
        }

        // Don't use for requests to HR server
        [UsedImplicitly]
        internal static async Task<string> GetAsync(string path) {
            using (UnityWebRequest www = UnityWebRequest.Get(path)) {
                await SendRequestAsync(www);

                if (string.IsNullOrEmpty(www.error)) {
                    return www.downloadHandler.text;
                } else {
                    return null;
                }
            }
        }

        internal static Task<UnityWebRequestAsyncOperation> SendRequestAsync(UnityWebRequest www) {
            var req = www.SendWebRequest();
            var tcs = new TaskCompletionSource<UnityWebRequestAsyncOperation>();
            req.completed += op => tcs.TrySetResult((UnityWebRequestAsyncOperation)op);
            return tcs.Task;
        }

        static bool pollPending;
        internal static async void PollMethodPatches(string lastPatchId, Action<MethodPatchResponse> onResponseReceived) {
            if (pollPending) {
                return;
            }
            pollPending = true;
            var searchPaths = assemblySearchPaths ?? CodePatcher.I.GetAssemblySearchPaths();
            var body = SerializeRequestBody(new MethodPatchRequest(lastPatchId, searchPaths, TimeSpan.FromSeconds(20), Path.GetDirectoryName(Application.dataPath)));
            
            await ThreadUtility.SwitchToThreadPool();
            
            try {
                var result = await PostJson(url + "/patch", body, 30, overrideClient: clientPollPatches).ConfigureAwait(false);
                if(result.statusCode == HttpStatusCode.OK) {
                    var responses = JsonConvert.DeserializeObject<MethodPatchResponse[]>(result.responseText);
                    await ThreadUtility.SwitchToMainThread();
                    foreach(var response in responses) {
                        onResponseReceived(response);
                    }
                } else if(result.statusCode == HttpStatusCode.Unauthorized || result.statusCode == 0) {
                    // Server is not running or not authorized.
                    // We don't want to spam requests in that case.
                    await Task.Delay(5000);
                } else if(result.statusCode == HttpStatusCode.ServiceUnavailable) {
                    //Server shut down
                    await Task.Delay(5000);
                } else {
                    Log.Info("PollMethodPatches failed with code {0} {1} {2}", (int)result.statusCode, result.responseText, result.exception);
                }
            } finally {
                pollPending = false;
            }
        }
        
        static bool pollPatchStatusPending;
        internal static async void PollPatchStatus(Action<PatchStatusResponse> onResponseReceived, PatchStatus latestStatus) {
            if (pollPatchStatusPending) return;

            pollPatchStatusPending = true;
            var body = SerializeRequestBody(new PatchStatusRequest(TimeSpan.FromSeconds(20), latestStatus));
            
            try {
                var result = await PostJson(url + "/patchStatus", body, 30, overrideClient: clientPollStatus).ConfigureAwait(false);
                if(result.statusCode == HttpStatusCode.OK) {
                    var response = JsonConvert.DeserializeObject<PatchStatusResponse>(result.responseText);
                    await ThreadUtility.SwitchToMainThread();
                    onResponseReceived(response);
                } else if(result.statusCode == HttpStatusCode.Unauthorized || result.statusCode == 0) {
                    // Server is not running or not authorized.
                    // We don't want to spam requests in that case.
                    await Task.Delay(5000);
                } else if(result.statusCode == HttpStatusCode.ServiceUnavailable) {
                    //Server shut down
                    await Task.Delay(5000);
                } else {
                    Log.Info("PollPatchStatus failed with code {0} {1} {2}", (int)result.statusCode, result.responseText, result.exception);
                }
            } finally {
                pollPatchStatusPending = false;
            }
        }
        
        static bool assetPollPending;
        internal static async void PollAssetChanges(Action<string> onResponseReceived) {
            if (assetPollPending) return;
        
            assetPollPending = true;
            
            try {
                await ThreadUtility.SwitchToThreadPool();
                var body = SerializeRequestBody(new AssetChangesRequest(TimeSpan.FromSeconds(20)));
                var result = await PostJson(url + "/assetChanges", body, 30, overrideClient: clientPollAssets).ConfigureAwait(false);
                
                if (result.statusCode == HttpStatusCode.OK) {
                    var responses = JsonConvert.DeserializeObject<List<string>>(result.responseText);
                    await ThreadUtility.SwitchToMainThread();
                    // Looping in reverse order fixes moving files:
                    // by default new files come in before old ones which causes issues because meta files for old location has to be deleted first
                    for (var i = responses.Count - 1; i >= 0; i--) {
                        var response = responses[i];
                        // Avoid importing assets twice
                        if (responses.Contains(response + ".meta")) {
                            Log.Debug($"Ignoring asset change inside Unity: {response}");
                            continue;
                        }
                        onResponseReceived(response);
                    }
                } else if(result.statusCode == HttpStatusCode.Unauthorized || result.statusCode == 0) {
                    // Server is not running or not authorized.
                    // We don't want to spam requests in that case.
                    await Task.Delay(5000);
                } else if(result.statusCode == HttpStatusCode.ServiceUnavailable) {
                    //Server shut down
                    await Task.Delay(5000);
                } else {
                    Log.Info("PollAssetChanges failed with code {0} {1} {2}", (int)result.statusCode, result.responseText, result.exception);
                }
            } finally {
                assetPollPending = false;
            }
        }
        
        public static async Task<FlushErrorsResponse> RequestFlushErrors(int timeoutSeconds = 30) {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            var resp = await PostJson(CreateUrl(serverInfo) + "/flush", "", timeoutSeconds, cts.Token);
            if (resp.statusCode == HttpStatusCode.OK) {
                try {
                    return JsonConvert.DeserializeObject<FlushErrorsResponse>(resp.responseText);
                } catch {
                    return null;
                }
            }
            return null;
        }
        
        public static async Task<LoginStatusResponse> RequestLogin(string email, string password, int timeoutSeconds) {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            var json = SerializeRequestBody(new Dictionary<string, object> {
                { "email", email },
                { "password", password },
            });
            var resp = await PostJson(url + "/login", json, timeoutSeconds, cts.Token);
            if (resp.exception == null) {
                return JsonConvert.DeserializeObject<LoginStatusResponse>(resp.responseText);
            } else {
                return LoginStatusResponse.FromRequestError($"{resp.exception.GetType().Name} {resp.exception.Message}");
            }
        }
        
        public static async Task<LoginStatusResponse> GetLoginStatus(int timeoutSeconds) {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            var resp = await PostJson(url + "/status", string.Empty, timeoutSeconds, cts.Token);
            if (resp.exception == null) {
                return JsonConvert.DeserializeObject<LoginStatusResponse>(resp.responseText);
            } else {
                return LoginStatusResponse.FromRequestError($"{resp.exception.GetType().Name} {resp.exception.Message}");
            }
        }
        
        internal static async Task<LoginStatusResponse> RequestLogout(int timeoutSeconds = 10) {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            var resp = await PostJson(CreateUrl(serverInfo) + "/logout", "", timeoutSeconds, cts.Token);
            if (resp.statusCode == HttpStatusCode.OK) {
                try {
                    return JsonConvert.DeserializeObject<LoginStatusResponse>(resp.responseText);
                } catch (Exception ex) {
                    return LoginStatusResponse.FromRequestError($"Deserializing response failed with {ex.GetType().Name}: {ex.Message}");
                }
            } else {
                return LoginStatusResponse.FromRequestError(resp.responseText ?? "Request timeout");
            }
        }

        internal static async Task<ActivatePromoCodeResponse> RequestActivatePromoCode(string promoCode, int timeoutSeconds = 20) {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            await ThreadUtility.SwitchToThreadPool();
            try {
                using (var resp = await client.PostAsync(CreateUrl(serverInfo) + "/activatePromoCode", new StringContent(promoCode), cts.Token).ConfigureAwait(false)) {
                    var str = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    try {
                        return JsonConvert.DeserializeObject<ActivatePromoCodeResponse>(str);
                    } catch {
                        return null;
                    }
                }
            } catch {
                return null;
            }
        }
        
        internal static async Task RequestEditorEventWithRetry(Stat stat, EditorExtraData extraData = null) {
            int attempt = 0;
            do {
                var resp = await RequestHelper.RequestEditorEvent(stat, extraData);
                if (resp.statusCode == HttpStatusCode.OK) {
                    return;
                }
                await Task.Delay(TimeSpan.FromMilliseconds(200));
            } while (attempt++ < 10000);
        }
        
        internal static Task<HttpResponse> RequestEditorEvent(Stat stat, EditorExtraData extraData = null) {
            var body = SerializeRequestBody(new EditorEventRequest(stat, extraData));
            return PostJson(url + "/editorEvent", body, int.MaxValue);
        }
        
        public static async Task KillServer() {
            await ThreadUtility.SwitchToThreadPool();
            await KillServerInternal().ConfigureAwait(false);
        }

        internal static async Task KillServerInternal() {
            try {
                using(await client.PostAsync(CreateUrl(serverInfo) + "/kill", new StringContent(origin)).ConfigureAwait(false)) { }
            } catch {
                //ignored
            } 
        }

        public static async Task<bool> PingServer(Uri uri) {
            await ThreadUtility.SwitchToThreadPool();
            
            try {
                using (var resp = await client.GetAsync(uri).ConfigureAwait(false)) {
                    return resp.StatusCode == HttpStatusCode.OK;
                }
            } catch {
                return false;
            }
        }
        
        public static bool IsReleaseMode() {
#           if (UNITY_EDITOR && UNITY_2022_1_OR_NEWER)
                return UnityEditor.Compilation.CompilationPipeline.codeOptimization == UnityEditor.Compilation.CodeOptimization.Release;
#           elif (UNITY_EDITOR)
                return false;
#           elif (DEBUG)
                return false;
#           else
                return true;
#endif
        }
        
        public static Task RequestClearPatches() {
            var body = SerializeRequestBody(new CompileRequest(serverInfo.rootPath, IsReleaseMode()));
            return PostJson(url + "/clearpatches", body, 10);
        }
        
        public static async Task RequestCompile(Action<string> onResponseReceived) {
            var body = SerializeRequestBody(new CompileRequest(serverInfo.rootPath, IsReleaseMode()));
            var result = await PostJson(url + "/compile", body, 10);
            if (result.statusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(result.responseText)) {
                var responses = JsonConvert.DeserializeObject<List<string>>(result.responseText);
                if (responses == null) {
                    return;
                }
                await ThreadUtility.SwitchToMainThread();
                foreach (var response in responses) {
                    // Avoid importing assets twice
                    if (responses.Contains(response + ".meta")) {
                        Log.Debug($"Ignoring asset change inside Unity: {response}");
                        continue;
                    }
                    onResponseReceived(response);
                }
            }
        }
        
        internal static async Task<List<ChangelogVersion>> FetchChangelog(int timeoutSeconds = 20) {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            await ThreadUtility.SwitchToThreadPool();
            
            try {
                using(var resp = await client.GetAsync(ChangelogURL, cts.Token).ConfigureAwait(false)) {
                    var str = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (resp.StatusCode == HttpStatusCode.OK) {
                        return JsonConvert.DeserializeObject<List<ChangelogVersion>>(str);
                    }
                    return new List<ChangelogVersion>();
                }
            } catch {
                return new List<ChangelogVersion>();
            }
        }
        
        [UsedImplicitly]
        internal static async Task<bool> Post(string route, string json) {
            var resp = await PostJson(url + route, json, 10);
            return resp.statusCode == HttpStatusCode.OK;
        }

        internal static async Task<MobileHandshakeResponse> RequestHandshake(PatchServerInfo info, HashSet<string> defineSymbols, string projectExclusionRegex) {
            await ThreadUtility.SwitchToThreadPool();
            
            var body = SerializeRequestBody(new MobileHandshakeRequest(defineSymbols, projectExclusionRegex));
            
            var requestUrl = CreateUrl(info) + "/handshake";
            Log.Debug($"RequestHandshake to {requestUrl}");
            var resp = await PostJson(requestUrl, body, 120).ConfigureAwait(false);
            if (resp.statusCode == HttpStatusCode.OK) {
                return JsonConvert.DeserializeObject<MobileHandshakeResponse>(resp.responseText);
            } else if(resp.statusCode == HttpStatusCode.ServiceUnavailable) {
                return new MobileHandshakeResponse(null, ServerHandshake.Result.WaitForCompiling.ToString());
            } else {
                return new MobileHandshakeResponse(null, resp.responseText);
            }
        }
        
        static string SerializeRequestBody<T>(T request) {
            return JsonConvert.SerializeObject(request);
        }
        
        static async Task<HttpResponse> PostJson(string uri, string json, int timeoutSeconds, CancellationToken token = default(CancellationToken), HttpClient overrideClient = null) {
            var httpClient = overrideClient ?? client;
            await ThreadUtility.SwitchToThreadPool();
            
            try {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                using(var resp = await httpClient.PostAsync(uri, content, token).ConfigureAwait(false)) {
                    var str = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return new HttpResponse(resp.StatusCode, null, str);
                }
            } catch(Exception ex) {
                return new HttpResponse(0, ex, null);
            } 
        }
    }
}
#endif
