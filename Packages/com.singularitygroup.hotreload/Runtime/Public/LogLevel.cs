namespace SingularityGroup.HotReload {
    /// <summary>
    /// The log level enumeration for the Hot Reload package
    /// Used in <see cref="HotReloadLogging.SetLogLevel"/> to set the log level.
    /// </summary>
    public enum LogLevel {
        /// Debug logs are useful for developers of Hot Reload 
        Debug = 1,
        
        /// Info logs potentially useful for users of Hot Reload 
        Info = 2,
        
        /// Warnings are visible to users of Hot Reload 
        Warning = 3,
        
        /// Errors are visible to users of Hot Reload 
        Error = 4,
        
        /// Exceptions are visible to users of Hot Reload 
        Exception = 5,
        
        /// No logs are visible to users of Hot Reload 
        Disabled = 6,
    }
}
