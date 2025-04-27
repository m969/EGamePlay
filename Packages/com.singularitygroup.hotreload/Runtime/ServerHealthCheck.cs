#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System;
using System.Threading.Tasks;

namespace SingularityGroup.HotReload {
    internal class ServerHealthCheck : IServerHealthCheck {
        private static readonly TimeSpan heartBeatTimeout = TimeSpan.FromMilliseconds(5001);
        public static readonly ServerHealthCheck I = new ServerHealthCheck();

        private Uri healthCheckEndpoint = null;
        private Task<bool> healthCheck = null;
        private DateTime healthOkayAt = DateTime.MinValue;

        public void SetServerInfo(PatchServerInfo serverInfo) {
            if (serverInfo == null) {
                Log.Debug("ServerHealthCheck SetServerInfo to null");
                healthCheckEndpoint = null;
            } else {
                var url = RequestHelper.CreateUrl(serverInfo) + "/ping";
                Log.Debug("ServerHealthCheck SetServerInfo using url {0}", url);
                healthCheckEndpoint = new Uri(url);
            }
            healthCheck = null;
            healthOkayAt = DateTime.MinValue;
        }

        public bool IsServerHealthy => DateTime.UtcNow - healthOkayAt < heartBeatTimeout;

        /// Is it confirmed the server has been running before? 
        public bool WasServerResponding => healthOkayAt != DateTime.MinValue;

        // any thread
        public async Task CheckHealthAsync() {
            if (healthCheckEndpoint == null
                // wait for existing healthcheck to finish
                || healthCheck?.IsCompleted == false
            ) {
                return;
            }
            healthCheck = CheckHealthAsync(healthCheckEndpoint);
            if (await healthCheck) {
                healthOkayAt = DateTime.UtcNow;
            }
        }

        public static async Task<bool> CheckHealthAsync(PatchServerInfo info) {
            var url = RequestHelper.CreateUrl(info) + "/ping";
            return await CheckHealthAsync(new Uri(url));
        }
        
        public static async Task<bool> CheckHealthAsync(Uri uri) {
            var ping = RequestHelper.PingServer(uri);
            await Task.WhenAny(ping, Task.Delay(heartBeatTimeout));
            return ping.IsCompleted && ping.Result;
        }
    }
}
#endif
