#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System;
using System.Threading;
using System.Threading.Tasks;
using SingularityGroup.HotReload.DTO;

namespace SingularityGroup.HotReload {
    
    static class PlayerCodePatcher {
        static Timer timer;

        static PlayerCodePatcher() {
            if (PlayerEntrypoint.IsPlayerWithHotReload()) {
                timer = new Timer(OnIntervalThreaded, (Action) OnIntervalMainThread, 500, 500);
                serverHealthyAt = DateTime.MinValue;
            }
        }

        private static DateTime serverHealthyAt;
        private static TimeSpan TimeSinceServerHealthy() => DateTime.UtcNow - serverHealthyAt;

        /// <summary>
        /// Set server that you want to try connect to.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This allows repetitions of:
        /// - try handshake
        ///   - success -> try healthcheck
        ///   - success -> poll method patches
        ///   - 
        /// </para>
        /// <para>
        /// Only do this after confirming (with /handshake) that server is compatible with this build.<br/>
        /// The user will be prompted if handshake needs confirmation.
        /// </para>
        /// </remarks>
        internal static Task<ServerHandshake.Result> UpdateHost(PatchServerInfo serverInfo) {
            Log.Debug($"UpdateHost to {(serverInfo == null ? "null" : serverInfo.hostName)}");
            // In player builds, server is remote, se we don't load assemblies from any paths
            RequestHelper.ChangeAssemblySearchPaths(Array.Empty<string>());
            ServerHealthCheck.I.SetServerInfo(null); // stop doing health check on old server
            RequestHelper.SetServerInfo(serverInfo);
            // Show feedback about connection progress (handshake can take ~5 seconds for our big game)
            if (serverInfo == null) {
                Prompts.SetConnectionState(ConnectionSummary.Disconnected);
            } else {
                Prompts.SetConnectionState(ConnectionSummary.Connected);
                Prompts.ShowConnectionDialog();
            }
            return ServerHandshake.I.SetServerInfo(serverInfo);
        }

        public static Task Disconnect() => UpdateHost(null);

        static void OnIntervalThreaded(object o) {
            ServerHandshake.I.CheckHandshake();
            ServerHealthCheck.I.CheckHealthAsync().Forget();

            ThreadUtility.RunOnMainThread((Action)o);
        }
        
        static string lastPatchId = string.Empty;
        static void OnIntervalMainThread() {
            PatchServerInfo verifiedServer;
            if(ServerHandshake.I.TryGetVerifiedServer(out verifiedServer)) {
                // now that handshake verified, we are connected.
                // Note: If there is delay between handshake done and chosing to connect, then it may be outdated.
                Prompts.SetConnectionState(ConnectionSummary.Connecting);
                // Note: verified does not imply that server is running, sometimes we verify the host just from the deeplink data 
                ServerHealthCheck.I.SetServerInfo(verifiedServer);
            }

            if(ServerHealthCheck.I.IsServerHealthy) {
                // we may have reconnected to the same host, after losing connection for several seconds
                Prompts.SetConnectionState(ConnectionSummary.Connected, false);
                serverHealthyAt = DateTime.UtcNow;
                RequestHelper.PollMethodPatches(lastPatchId, resp => HandleResponseReceived(resp));
            } else if (ServerHealthCheck.I.WasServerResponding) { // only update prompt state if disconnected server 
                var secondsSinceHealthy = TimeSinceServerHealthy().TotalSeconds;
                var reconnectTimeout = 30; // seconds
                if (secondsSinceHealthy > 2) {
                    Log.Info("Hot Reload was unreachable for 5 seconds, trying to reconnect...");
                    // feedback for the user so they know why patches are not applying
                    Prompts.SetConnectionState($"{ConnectionSummary.TryingToReconnect} {reconnectTimeout - secondsSinceHealthy:F0}s", false);
                    Prompts.ShowConnectionDialog();
                }
                if (secondsSinceHealthy > reconnectTimeout) {
                    // give up on the server, give user a way to connect to another
                    Log.Info($"Hot Reload was unreachable for {reconnectTimeout} seconds, disconnecting");
                    var disconnectedServer = RequestHelper.ServerInfo;
                    Disconnect().Forget();
                    // Let user tap button to retry connecting to the same server (maybe just need to run Hot Reload again)
                    // Assumption: prompt also has a way to connect to a different server 
                    Prompts.ShowRetryDialog(disconnectedServer);
                }
            }
        }
        
        static void HandleResponseReceived(MethodPatchResponse response) {
            Log.Debug("PollMethodPatches handling MethodPatchResponse id:{0} response.patches.Length:{1} response.failures.Length:{2}",
                response.id, response.patches.Length, response.failures.Length);
            if(response.patches.Length > 0) {
                CodePatcher.I.RegisterPatches(response, persist: true);
            }
            if(response.failures.Length > 0) {
                foreach (var failure in response.failures) {
                    // feedback to user so they know why their patch wasn't applied
                    Log.Warning(failure);
                }
            }
            lastPatchId = response.id;
        }
    }

}
#endif
