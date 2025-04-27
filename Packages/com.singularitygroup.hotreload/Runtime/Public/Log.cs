using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
#if (UNITY_2019_4_OR_NEWER)
using UnityEngine;
#endif

namespace SingularityGroup.HotReload {
    public static class Log {
        [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
        public static LogLevel minLevel = LogLevel.Info;

        /// <summary>
        /// Tag every log so that users know which logs came from Hot Reload
        /// </summary>
        private const string TAG = "[HotReload] ";

        public static void Debug(string message) {
            if (minLevel <= LogLevel.Debug) {
            #if (UNITY_2019_4_OR_NEWER)
                UnityEngine.Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "{0}{1}", TAG, message);
            #else
                UnityEngine.Debug.Log(TAG + message);
            #endif
            }
        }

        [StringFormatMethod("message")]
        public static void Debug(string message, params object[] args) {
            if (minLevel <= LogLevel.Debug) {
            #if (UNITY_2019_4_OR_NEWER)
                UnityEngine.Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, TAG + message, args);
            #else
                UnityEngine.Debug.LogFormat(TAG + message, args);
            #endif
            }
        }
        
        public static void Info(string message) {
            if (minLevel <= LogLevel.Info) {
            #if (UNITY_2019_4_OR_NEWER)
                UnityEngine.Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, "{0}{1}", TAG, message);
            #else
                UnityEngine.Debug.Log(TAG + message);
            #endif
            }
        }

        [StringFormatMethod("message")]
        public static void Info(string message, params object[] args) {
            if (minLevel <= LogLevel.Info) {
            #if (UNITY_2019_4_OR_NEWER)
                UnityEngine.Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null, TAG + message, args);
            #else
                UnityEngine.Debug.LogFormat(TAG + message, args);
            #endif
            }
        }

        public static void Warning(string message) {
            if (minLevel <= LogLevel.Warning) {
            #if (UNITY_2019_4_OR_NEWER)
                UnityEngine.Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, "{0}{1}", TAG, message);
            #else
                UnityEngine.Debug.LogWarning(TAG + message);
            #endif
            }
        }
        
        [StringFormatMethod("message")]
        public static void Warning(string message, params object[] args) {
            if (minLevel <= LogLevel.Warning) {
            #if (UNITY_2019_4_OR_NEWER)
                UnityEngine.Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null, TAG + message, args);
            #else
                UnityEngine.Debug.LogWarningFormat(TAG + message, args);
            #endif
            }
        }

        public static void Error(string message) {
            if (minLevel <= LogLevel.Error) {
            #if (UNITY_2019_4_OR_NEWER)
                UnityEngine.Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, "{0}{1}", TAG, message);
            #else
                UnityEngine.Debug.LogError(TAG + message);
            #endif
            }
        }
        
        [StringFormatMethod("message")]
        public static void Error(string message, params object[] args) {
            if (minLevel <= LogLevel.Error) {
            #if (UNITY_2019_4_OR_NEWER)
                UnityEngine.Debug.LogFormat(LogType.Error, LogOption.NoStacktrace, null, TAG + message, args);
            #else
                UnityEngine.Debug.LogErrorFormat(TAG + message, args);
            #endif
            }
        }
        
        public static void Exception(Exception exception) {
            if (minLevel <= LogLevel.Exception) {
                UnityEngine.Debug.LogException(exception);
            }
        }
    }
}
