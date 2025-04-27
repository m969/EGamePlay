using System;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Threading.Tasks;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace SingularityGroup.HotReload.Editor {
    internal static class EditorWindowHelper {
        #if UNITY_2020_1_OR_NEWER
        public static bool supportsNotifications = true;
        #else
        public static bool supportsNotifications = false;
        #endif
        
        private static readonly Regex ValidEmailRegex = new Regex(@"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
            + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
            + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$", RegexOptions.IgnoreCase);

        public static bool IsValidEmailAddress(string email) {
            return ValidEmailRegex.IsMatch(email);
        }

        public static bool IsHumanControllingUs() {
            if (Application.isBatchMode) {
                return false;
            }
            
            var isCI = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));
            return !isCI;
        }

        internal enum NotificationStatus {  
            None,
            Patching,
            NeedsRecompile
        }

        private static readonly Dictionary<NotificationStatus, GUIContent> notificationContent = new Dictionary<NotificationStatus, GUIContent> {
            { NotificationStatus.Patching, new GUIContent("[Hot Reload] Applying patches...")},
            { NotificationStatus.NeedsRecompile, new GUIContent("[Hot Reload] Unsupported Changes detected! Recompiling...")},
        };
        
        static Type gameViewT;
        private static EditorWindow[] gameViewWindows {
            get {
                gameViewT = gameViewT ?? typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView");
                return Resources.FindObjectsOfTypeAll(gameViewT).Cast<EditorWindow>().ToArray();
            }
        }

        private static EditorWindow[] sceneWindows {
            get {
                return Resources.FindObjectsOfTypeAll(typeof(SceneView)).Cast<EditorWindow>().ToArray();
            }
        }

        private static EditorWindow[] notificationWindows {
            get {
                return gameViewWindows.Concat(sceneWindows).ToArray();
            }
        }

        static NotificationStatus lastNotificationStatus;
        private static DateTime? latestNotificationStartedAt;
        private static bool notificationShownRecently => latestNotificationStartedAt != null && DateTime.UtcNow - latestNotificationStartedAt < TimeSpan.FromSeconds(1);
        internal static void ShowNotification(NotificationStatus notificationType, float maxDuration = 3) {
            // Patch status goes from Unsupported changes to patching rapidly when making unsupported change
            // patching also shows right before unsupported changes sometimes 
            // so we don't override NeedsRecompile notification ever
            bool willOverrideNeedsCompileNotification = notificationType != NotificationStatus.NeedsRecompile && notificationShownRecently || lastNotificationStatus == NotificationStatus.NeedsRecompile && notificationShownRecently;
            if (!supportsNotifications || willOverrideNeedsCompileNotification) {
                return;
            }

            foreach (EditorWindow notificationWindow in notificationWindows) {
                notificationWindow.ShowNotification(notificationContent[notificationType], maxDuration);
                notificationWindow.Repaint();
            }
            latestNotificationStartedAt = DateTime.UtcNow;
            lastNotificationStatus = notificationType;
        }

        internal static void RemoveNotification() {
            if (!supportsNotifications) {
                return;
            }
            // only patching notifications should be removed after showing less than 1 second
            if (notificationShownRecently && lastNotificationStatus != NotificationStatus.Patching) {
                return;
            }
            foreach (EditorWindow notificationWindow in notificationWindows) {                
                notificationWindow.RemoveNotification(); 
                notificationWindow.Repaint();
            }   
            latestNotificationStartedAt = null;    
            lastNotificationStatus = NotificationStatus.None;
        }
    }
}
