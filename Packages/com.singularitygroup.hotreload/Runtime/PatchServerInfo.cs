#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System;
using SingularityGroup.HotReload.Newtonsoft.Json;
using UnityEngine;

namespace SingularityGroup.HotReload {
    [Serializable]
    class PatchServerInfo {
        public readonly string hostName;
        public readonly int port;
        public readonly string commitHash;
        public readonly string rootPath;
        [Obsolete]public readonly bool isRemote;
        public readonly string customRequestOrigin;

        public const string UnknownCommitHash = "unknown";

        /// <param name="hostName">an ip address or "localhost"</param>
        public PatchServerInfo(string hostName, string commitHash, string rootPath) {
            this.hostName = hostName;
            this.commitHash = commitHash ?? UnknownCommitHash;
            this.rootPath = rootPath;
            this.port = RequestHelper.defaultPort;
        }
        
        /// <param name="hostName">an ip address or "localhost"</param>
        // constructor should (must?) have a param for each field
        [JsonConstructor]
        public PatchServerInfo(string hostName, int port, string commitHash, string rootPath, bool isRemote = false, string customRequestOrigin = null) {
            this.hostName = hostName;
            this.port = port;
            this.commitHash = commitHash ?? UnknownCommitHash;
            this.rootPath = rootPath;
#pragma warning disable CS0612 // Type or member is obsolete
            this.isRemote = isRemote;
#pragma warning restore CS0612 // Type or member is obsolete
            this.customRequestOrigin = customRequestOrigin;
        }
    }
}
#endif
