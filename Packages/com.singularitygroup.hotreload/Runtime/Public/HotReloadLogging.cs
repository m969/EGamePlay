namespace SingularityGroup.HotReload {
    /// <summary>
    /// Utility class to set the log level of the Hot Reload package
    /// </summary>
    public static class HotReloadLogging {
        /// <summary>
        /// Sets the log level for logs inside the Hot Reload package
        /// The default log level is <see cref="LogLevel.Info"/>
        /// </summary>
        /// <remarks>
        /// To see more detailed logs, set the log level to <see cref="LogLevel.Debug"/>
        /// </remarks>
        public static void SetLogLevel(LogLevel level) {
            Log.minLevel = level;
        }
    }
}
