#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System.IO;

namespace SingularityGroup.HotReload {
    static class PersistencePaths {
        public static string GetPatchesFilePath(string basePath) {
            return Path.Combine(basePath, "CodePatcher", "patches.bin");
        }
        
        public static string GetServerInfoFilePath(string basePath) {
            return Path.Combine(basePath, "CodePatcher", "hostInfo.json");
        }
    }
}
#endif
