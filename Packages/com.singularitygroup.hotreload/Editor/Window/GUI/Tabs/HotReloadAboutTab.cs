using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;
using SingularityGroup.HotReload.Newtonsoft.Json;
using SingularityGroup.HotReload.EditorDependencies;

namespace SingularityGroup.HotReload.Editor {
    internal struct HotReloadAboutTabState {
        public readonly bool logsFodlerExists;
        public readonly IReadOnlyList<ChangelogVersion> changelog;
        public readonly bool loginRequired;
        public readonly bool hasTrialLicense;
        public readonly bool hasPayedLicense;
        
        public HotReloadAboutTabState(
            bool logsFodlerExists,
            IReadOnlyList<ChangelogVersion> changelog,
            bool loginRequired,
            bool hasTrialLicense,
            bool hasPayedLicense
        ) {
            this.logsFodlerExists = logsFodlerExists;
            this.changelog = changelog;
            this.loginRequired = loginRequired;
            this.hasTrialLicense = hasTrialLicense;
            this.hasPayedLicense = hasPayedLicense;
        }
    }
    
    internal class HotReloadAboutTab : HotReloadTabBase {
        internal static readonly OpenURLButton seeMore = new OpenURLButton("See More", Constants.ChangelogURL);
        internal static readonly OpenDialogueButton manageLicenseButton = new OpenDialogueButton("Manage License", Constants.ManageLicenseURL, "Manage License", "Upgrade/downgrade/edit your subscription and edit payment info.", "Open in browser", "Cancel");
        internal static readonly OpenDialogueButton manageAccountButton = new OpenDialogueButton("Manage Account", Constants.ManageAccountURL, "Manage Account", "Login with company code 'naughtycult'. Use the email you signed up with. Your initial password was sent to you by email.", "Open in browser", "Cancel");
        internal static readonly OpenURLButton contactButton = new OpenURLButton("Contact", Constants.ContactURL);
        internal static readonly OpenURLButton discordButton = new OpenURLButton("Join Discord", Constants.DiscordInviteUrl);
        internal static readonly OpenDialogueButton reportIssueButton = new OpenDialogueButton("Report issue", Constants.ReportIssueURL, "Report issue", "Report issue in our public issue tracker. Requires gitlab.com account (if you don't have one and are not willing to make it, please contact us by other means such as our website).", "Open in browser", "Cancel");

        private Vector2 _changelogScroll;
        private IReadOnlyList<ChangelogVersion> _changelog = new List<ChangelogVersion>();
        private bool _requestedChangelog;
        private int _changelogRequestAttempt;
        private string _changelogDir = Path.Combine(PackageConst.LibraryCachePath, "changelog.json");
        public static string logsPath = Path.Combine(PackageConst.LibraryCachePath, "logs");

        private static bool LatestChangelogLoaded(IReadOnlyList<ChangelogVersion> changelog) {
            return changelog.Any() && changelog[0].versionNum == PackageUpdateChecker.lastRemotePackageVersion;
        }
        
        private async Task FetchChangelog() {
            if(!_changelog.Any()) {
                var file = new FileInfo(_changelogDir);
                if (file.Exists) {
                    await Task.Run(() => {
                        var bytes = File.ReadAllText(_changelogDir);
                        _changelog = JsonConvert.DeserializeObject<List<ChangelogVersion>>(bytes);
                    });
                }
            }
            if (_requestedChangelog || LatestChangelogLoaded(_changelog)) {
                return;
            }
            _requestedChangelog = true;
            try {
                do {
                    var changelogRequestTimeout = ExponentialBackoff.GetTimeout(_changelogRequestAttempt);
                    _changelog = await RequestHelper.FetchChangelog() ?? _changelog;
                    if (LatestChangelogLoaded(_changelog)) {
                        await Task.Run(() => {
                            Directory.CreateDirectory(PackageConst.LibraryCachePath);
                            File.WriteAllText(_changelogDir, JsonConvert.SerializeObject(_changelog));
                        });
                        Repaint();
                        return;
                    }
                    await Task.Delay(changelogRequestTimeout);
                } while (_changelogRequestAttempt++ < 1000 && !LatestChangelogLoaded(_changelog));
            } catch {
                // ignore
            } finally {
                _requestedChangelog = false;    
            }
        }
        
        public HotReloadAboutTab(HotReloadWindow window) : base(window, "Help", "_Help", "Info and support for Hot Reload for Unity.") { }

        string GetRelativeDate(DateTime givenDate) {
            const int second = 1;
            const int minute = 60 * second;
            const int hour = 60 * minute;
            const int day = 24 * hour;
            const int month = 30 * day;

            var ts = new TimeSpan(DateTime.UtcNow.Ticks - givenDate.Ticks);
            var delta = Math.Abs(ts.TotalSeconds);

            if (delta < 24 * hour)
                return "Today";

            if (delta < 48 * hour)
                return "Yesterday";

            if (delta < 30 * day)
                return ts.Days + " days ago";

            if (delta < 12 * month) {
                var months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            var years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
        }

        void RenderVersion(ChangelogVersion version) {
            var tempTextString = "";
            
            //version number
            EditorGUILayout.TextArea(version.versionNum, HotReloadWindowStyles.H1TitleStyle);
            
            //general info
            if (version.generalInfo != null) {
                EditorGUILayout.TextArea(version.generalInfo, HotReloadWindowStyles.H3TitleStyle);
            }
            
            //features
            if (version.features != null) {
                EditorGUILayout.TextArea("Features:", HotReloadWindowStyles.H2TitleStyle);
                tempTextString = "";
                foreach (var feature in version.features) {
                    tempTextString += "• " + feature + "\n";
                }
                EditorGUILayout.TextArea(tempTextString, HotReloadWindowStyles.ChangelogPointerStyle);
            }
            
            //improvements
            if (version.improvements != null) {
                EditorGUILayout.TextArea("Improvements:", HotReloadWindowStyles.H2TitleStyle);
                tempTextString = "";
                foreach (var improvement in version.improvements) {
                    tempTextString += "• " + improvement + "\n";
                }
                EditorGUILayout.TextArea(tempTextString, HotReloadWindowStyles.ChangelogPointerStyle);
            }
            
            //fixes
            if (version.fixes != null) {
                EditorGUILayout.TextArea("Fixes:", HotReloadWindowStyles.H2TitleStyle);
                tempTextString = "";
                foreach (var fix in version.fixes) {
                    tempTextString += "• " + fix + "\n";
                }
                EditorGUILayout.TextArea(tempTextString, HotReloadWindowStyles.ChangelogPointerStyle);
            }
            
            //date
            DateTime date;
            if (DateTime.TryParseExact(version.date, "dd/MM/yyyy", null, DateTimeStyles.None, out date)) {
                var relativeDate = GetRelativeDate(date);
                GUILayout.TextArea(relativeDate, HotReloadWindowStyles.H3TitleStyle);
            }
        }

        void RenderChangelog() {
            FetchChangelog().Forget();
            using (new EditorGUILayout.HorizontalScope(HotReloadWindowStyles.SectionInnerBoxWide)) {
                using (new EditorGUILayout.VerticalScope()) {
                    HotReloadPrefs.ShowChangeLog = EditorGUILayout.Foldout(HotReloadPrefs.ShowChangeLog, "Changelog", true, HotReloadWindowStyles.FoldoutStyle);
                    if (!HotReloadPrefs.ShowChangeLog) {
                        return;
                    }
                    // changelog versions                        
                    var maxChangeLogs = 5;
                    var index = 0;
                    foreach (var version in currentState.changelog) {
                        index++;
                        if (index > maxChangeLogs) {
                            break;
                        }

                        using (new EditorGUILayout.HorizontalScope(HotReloadWindowStyles.ChangelogSectionInnerBox)) {
                            using (new EditorGUILayout.VerticalScope()) {
                                RenderVersion(version);
                            }
                        }
                    }
                    // see more button
                    using (new EditorGUILayout.HorizontalScope(HotReloadWindowStyles.ChangelogSectionInnerBox)) {
                        seeMore.OnGUI();
                    }
                }
            }
        }
        
        private Vector2 _aboutTabScrollPos;
        
        HotReloadAboutTabState currentState;
        public override void OnGUI() {
            // HotReloadAboutTabState ensures rendering is consistent between Layout and Repaint calls
            // Without it errors like this happen:
            // ArgumentException: Getting control 2's position in a group with only 2 controls when doing repaint
            // See thread for more context: https://answers.unity.com/questions/17718/argumentexception-getting-control-2s-position-in-a.html
            if (Event.current.type == EventType.Layout) {
                currentState = new HotReloadAboutTabState(
                    logsFodlerExists: Directory.Exists(logsPath),
                    changelog: _changelog,
                    loginRequired: EditorCodePatcher.LoginNotRequired,
                    hasTrialLicense: _window.RunTab.TrialLicense,
                    hasPayedLicense: _window.RunTab.HasPayedLicense
                );
            }
            using (var scope = new EditorGUILayout.ScrollViewScope(_aboutTabScrollPos, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUILayout.MaxHeight(Math.Max(HotReloadWindowStyles.windowScreenHeight, 800)), GUILayout.MaxWidth(Math.Max(HotReloadWindowStyles.windowScreenWidth, 800)))) {
                _aboutTabScrollPos.x = scope.scrollPosition.x;
                _aboutTabScrollPos.y = scope.scrollPosition.y;

                using (new EditorGUILayout.VerticalScope(HotReloadWindowStyles.DynamicSectionHelpTab)) {
                    using (new EditorGUILayout.VerticalScope()) {
                        GUILayout.Space(10);
                        RenderLogButtons();

                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox($" Hot Reload version {PackageConst.Version}. ", MessageType.Info);
                        EditorGUILayout.Space();

                        RenderHelpButtons();

                        GUILayout.Space(15);

                        try {
                            RenderChangelog();
                        } catch {
                            // ignore
                        }
                    }
                }
            }
        }

        void RenderHelpButtons() {
            var labelRect = GUILayoutUtility.GetLastRect();
            using (new EditorGUILayout.HorizontalScope()) {
                using (new EditorGUILayout.VerticalScope()) {
                    var buttonHeight = 19;
                    
                    var bigButtonRect = new Rect(labelRect.x + 3, labelRect.y + 5, labelRect.width - 6, buttonHeight);
                    OpenURLButton.RenderRaw(bigButtonRect, "Documentation", Constants.DocumentationURL, HotReloadWindowStyles.HelpTabButton);
                    
                    var firstLayerX = bigButtonRect.x;
                    var firstLayerY = bigButtonRect.y + buttonHeight + 3;
                    var firstLayerWidth = (int)((bigButtonRect.width / 2) - 3);
                    
                    var secondLayerX = firstLayerX + firstLayerWidth + 5;
                    var secondLayerY = firstLayerY + buttonHeight + 3;
                    var secondLayerWidth = bigButtonRect.width - firstLayerWidth - 5;
                    
                    using (new EditorGUILayout.HorizontalScope()) {
                        OpenURLButton.RenderRaw(new Rect { x = firstLayerX, y = firstLayerY, width = firstLayerWidth, height = buttonHeight }, contactButton.text, contactButton.url, HotReloadWindowStyles.HelpTabButton);
                        OpenURLButton.RenderRaw(new Rect { x = secondLayerX, y = firstLayerY, width = secondLayerWidth, height = buttonHeight }, "Unity Forum", Constants.ForumURL, HotReloadWindowStyles.HelpTabButton);
                    }
                    using (new EditorGUILayout.HorizontalScope()) {
                        OpenDialogueButton.RenderRaw(rect: new Rect { x = firstLayerX, y = secondLayerY, width = firstLayerWidth, height = buttonHeight }, text: reportIssueButton.text, url: reportIssueButton.url, title: reportIssueButton.title, message: reportIssueButton.message, ok: reportIssueButton.ok, cancel: reportIssueButton.cancel, style: HotReloadWindowStyles.HelpTabButton);
                        OpenURLButton.RenderRaw(new Rect { x = secondLayerX, y = secondLayerY, width = secondLayerWidth, height = buttonHeight }, discordButton.text, discordButton.url, HotReloadWindowStyles.HelpTabButton);
                    }
                }
            }
            GUILayout.Space(80);
        }

        void RenderLogButtons() {
            if (currentState.logsFodlerExists) {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Open Log File")) {
                    var mostRecentFile = LogsHelper.FindRecentLog(logsPath);
                    if (mostRecentFile == null) {
                        Log.Info("No logs found");
                    } else {
                        try {
                            Process.Start($"\"{Path.Combine(logsPath, mostRecentFile)}\"");
                        } catch (Win32Exception e) {
                            if (e.Message.Contains("Application not found")) {
                                try {
                                    Process.Start("notepad.exe", $"\"{Path.Combine(logsPath, mostRecentFile)}\"");
                                } catch {
                                    // Fallback to opening folder with all logs
                                    Process.Start($"\"{logsPath}\"");
                                    Log.Info("Failed opening log file.");
                                }
                            }
                        } catch {
                            // Fallback to opening folder with all logs
                            Process.Start($"\"{logsPath}\"");
                            Log.Info("Failed opening log file.");
                        }
                    }
                }
                if (GUILayout.Button("Browse all logs")) {
                    Process.Start($"\"{logsPath}\"");
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
