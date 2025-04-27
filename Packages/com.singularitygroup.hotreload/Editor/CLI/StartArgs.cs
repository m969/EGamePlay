namespace SingularityGroup.HotReload.Editor.Cli {
    class StartArgs {
        public string hotreloadTempDir;
        // aka method patch temp dir
        public string cliTempDir;
        public string executableTargetDir;
        public string executableSourceDir;
        public string cliArguments;
        public string unityProjDir;
        public bool createNoWindow;
    }
}