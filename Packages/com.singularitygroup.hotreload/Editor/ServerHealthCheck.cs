using System;
using System.IO;
using SingularityGroup.HotReload.Editor.Cli;

namespace SingularityGroup.HotReload.Editor {
    public class ServerHealthCheck : IServerHealthCheckInternal {
        private static readonly TimeSpan heartBeatTimeout = TimeSpan.FromMilliseconds(5000);
        internal static readonly IServerHealthCheckInternal instance = new ServerHealthCheck();
        
        public static IServerHealthCheck I => instance;
        public static TimeSpan HeartBeatTimeout => heartBeatTimeout;
        
        ServerHealthCheck() { }
        
        /// <summary>
        /// Whether or not the server is running and responsive
        /// </summary>
        public bool IsServerHealthy { get; private set; }

        void IServerHealthCheckInternal.CheckHealth() {
            var fi = new FileInfo(Path.Combine(CliUtils.GetCliTempDir(), "health"));
            IsServerHealthy = fi.Exists && DateTime.UtcNow - fi.LastWriteTimeUtc < heartBeatTimeout;
        }
    }
}