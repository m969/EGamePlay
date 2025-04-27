using System;
using System.Collections.Generic;
using System.IO;
using SingularityGroup.HotReload.DTO;
using SingularityGroup.HotReload.EditorDependencies;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Color = UnityEngine.Color;
using Task = System.Threading.Tasks.Task;
#if UNITY_2019_4_OR_NEWER
using Unity.CodeEditor;
#endif

namespace SingularityGroup.HotReload.Editor {
    internal class ErrorData {
        public string fileName;
        public string error;
        public TextAsset file;
        public int lineNumber;
        public string stacktrace;
        public string linkString;
        private static string[] supportedPaths = new[] { Path.GetFullPath("Assets"), Path.GetFullPath("Plugins") };
        
        public static ErrorData GetErrorData(string errorString) {
            // Get the relevant file name
            string stackTrace = errorString;
            string fileName = null;
            try {
                int csIndex = 0;
                int attempt = 0;
                do {
                    csIndex = errorString.IndexOf(".cs", csIndex + 1, StringComparison.Ordinal);
                    if (csIndex == -1) {
                        break;
                    }
                    int fileNameStartIndex = csIndex - 1;
                    for (; fileNameStartIndex >= 0; fileNameStartIndex--) {
                        if (!char.IsLetter(errorString[fileNameStartIndex])) {
                            if (errorString.Contains("error CS")) {
                                fileName = errorString.Substring(fileNameStartIndex + 1,
                                    csIndex - fileNameStartIndex + ".cs".Length - 1);
                            } else {
                                fileName = errorString.Substring(fileNameStartIndex,
                                    csIndex - fileNameStartIndex + ".cs".Length);
                            }
                            break;
                        }
                    }
                } while (attempt++ < 100 && fileName == null);
            } catch {
                // ignore
            }
            fileName = fileName ?? "Tap to show stacktrace";
            
            // Get the error
            string error = (errorString.Contains("error CS") 
                               ? "Compile error, " 
                               : "Unsupported change detected, ") + "tap here to see more.";
            int endOfError = errorString.IndexOf(". in ", StringComparison.Ordinal);
            string specialChars = "\"'/\\";
            char[] characters = specialChars.ToCharArray();
            int specialChar = errorString.IndexOfAny(characters);
            try {
                if (errorString.Contains("error CS") ) {
                    error = errorString.Substring(errorString.IndexOf("error CS", StringComparison.Ordinal), errorString.Length - errorString.IndexOf("error CS", StringComparison.Ordinal)).Trim();
                    using (StringReader reader = new StringReader(error)) {
                        string line;
                        while ((line = reader.ReadLine()) != null) {
                            error = line;
                            break;
                        }
                    }
                } else if (errorString.StartsWith("errors:", StringComparison.Ordinal) && endOfError > 0) {
                    error = errorString.Substring("errors: ".Length, endOfError - "errors: ".Length).Trim();
                } else if (errorString.StartsWith("errors:", StringComparison.Ordinal) && specialChar > 0) {
                    error = errorString.Substring("errors: ".Length, specialChar - "errors: ".Length).Trim();
                } 
            } catch {
                // ignore
            }

            // Get relative path
            TextAsset file = null;
            try {
                foreach (var path in supportedPaths) {
                    int lastprojectIndex = 0;
                    int attempt = 0;
                    while (attempt++ < 100 && !file) {
                        lastprojectIndex = errorString.IndexOf(path, lastprojectIndex + 1, StringComparison.Ordinal);
                        if (lastprojectIndex == -1) {
                            break;
                        }
                        var fullCsIndex = errorString.IndexOf(".cs", lastprojectIndex, StringComparison.Ordinal);
                        var l = fullCsIndex - lastprojectIndex + ".cs".Length;
                        if (l <= 0) {
                            continue;
                        }
                        var candidateAbsolutePath = errorString.Substring(lastprojectIndex, fullCsIndex - lastprojectIndex + ".cs".Length);
                        var candidateRelativePath = EditorCodePatcher.GetRelativePath(filespec: candidateAbsolutePath, folder: path);
                        file = AssetDatabase.LoadAssetAtPath<TextAsset>(candidateRelativePath);
                    }
                }
            } catch {
                // ignore
            }
            
            // Get the line number
            int lineNumber = 0;
            try {
                int lastIndex = 0;
                int attempt = 0;
                do {
                    lastIndex = errorString.IndexOf(fileName, lastIndex + 1, StringComparison.Ordinal);
                    if (lastIndex == -1) {
                        break;
                    }
                    var part = errorString.Substring(lastIndex + fileName.Length);
                    if (!part.StartsWith(errorString.Contains("error CS") ? "(" : ":", StringComparison.Ordinal) 
                        || part.Length == 1 
                        || !char.IsDigit(part[1])
                       ) {
                        continue;
                    }
                    int y = 1;
                    for (; y < part.Length; y++) {
                        if (!char.IsDigit(part[y])) {
                            break;
                        }
                    }
                    if (int.TryParse(part.Substring(1, errorString.Contains("error CS") ? y - 1 : y), out lineNumber)) {
                        break;
                    }
                } while (attempt++ < 100);
            } catch { 
                //ignore
            }

            return new ErrorData() {
                fileName = fileName,
                error = error,
                file = file,
                lineNumber = lineNumber,
                stacktrace = stackTrace,
                linkString = lineNumber > 0 ? fileName + ":" + lineNumber : fileName
            };
        }
        
    }
    
    internal struct HotReloadRunTabState {
        public readonly bool spinnerActive;
        public readonly string indicationIconPath;
        public readonly bool requestingDownloadAndRun;
        public readonly bool starting;
        public readonly bool stopping;
        public readonly bool running;
        public readonly Tuple<float, string> startupProgress;
        public readonly string indicationStatusText;
        public readonly LoginStatusResponse loginStatus;
        public readonly bool downloadRequired;
        public readonly bool downloadStarted;
        public readonly bool requestingLoginInfo;
        public readonly RedeemStage redeemStage;
        public readonly int suggestionCount;

        public HotReloadRunTabState(
            bool spinnerActive, 
            string indicationIconPath,
            bool requestingDownloadAndRun,
            bool starting,
            bool stopping,
            bool running,
            Tuple<float, string> startupProgress,
            string indicationStatusText,
            LoginStatusResponse loginStatus,
            bool downloadRequired,
            bool downloadStarted,
            bool requestingLoginInfo,
            RedeemStage redeemStage,
            int suggestionCount
        ) {
            this.spinnerActive = spinnerActive;
            this.indicationIconPath = indicationIconPath;
            this.requestingDownloadAndRun = requestingDownloadAndRun;
            this.starting = starting;
            this.stopping = stopping;
            this.running = running;
            this.startupProgress = startupProgress;
            this.indicationStatusText = indicationStatusText;
            this.loginStatus = loginStatus;
            this.downloadRequired = downloadRequired;
            this.downloadStarted = downloadStarted;
            this.requestingLoginInfo = requestingLoginInfo;
            this.redeemStage = redeemStage;
            this.suggestionCount = suggestionCount;
        }

        public static HotReloadRunTabState Current => new HotReloadRunTabState(
            spinnerActive: EditorIndicationState.SpinnerActive,
            indicationIconPath: EditorIndicationState.IndicationIconPath,
            requestingDownloadAndRun: EditorCodePatcher.RequestingDownloadAndRun,
            starting: EditorCodePatcher.Starting,
            stopping: EditorCodePatcher.Stopping,
            running: EditorCodePatcher.Running,
            startupProgress: EditorCodePatcher.StartupProgress,
            indicationStatusText: EditorIndicationState.IndicationStatusText,
            loginStatus: EditorCodePatcher.Status,
            downloadRequired: EditorCodePatcher.DownloadRequired,
            downloadStarted: EditorCodePatcher.DownloadStarted,
            requestingLoginInfo: EditorCodePatcher.RequestingLoginInfo,
            redeemStage: RedeemLicenseHelper.I.RedeemStage,
            suggestionCount: HotReloadTimelineHelper.Suggestions.Count
        );
    }

    internal struct LicenseErrorData {
        public readonly string description;
        public bool showBuyButton;
        public string buyButtonText;
        public readonly bool showLoginButton;
        public readonly string loginButtonText;
        public readonly bool showSupportButton;
        public readonly string supportButtonText;
        public readonly bool showManageLicenseButton;
        public readonly string manageLicenseButtonText;

        public LicenseErrorData(string description, bool showManageLicenseButton = false, string manageLicenseButtonText = "", string loginButtonText = "", bool showSupportButton = false, string supportButtonText = "", bool showBuyButton = false, string buyButtonText = "", bool showLoginButton = false) {
            this.description = description;
            this.showManageLicenseButton = showManageLicenseButton;
            this.manageLicenseButtonText = manageLicenseButtonText;
            this.loginButtonText = loginButtonText;
            this.showSupportButton = showSupportButton;
            this.supportButtonText = supportButtonText;
            this.showBuyButton = showBuyButton;
            this.buyButtonText = buyButtonText;
            this.showLoginButton = showLoginButton;
        }
    }
    
    internal class HotReloadRunTab : HotReloadTabBase {
        private static string _pendingEmail;
        private static string _pendingPassword;
        private string _pendingPromoCode;
        private bool _requestingActivatePromoCode;

        private static Tuple<string, MessageType> _activateInfoMessage;

        private HotReloadRunTabState currentState => _window.RunTabState;
        // Has Indie or Pro license (even if not currenctly active)
        public bool HasPayedLicense => currentState.loginStatus != null && (currentState.loginStatus.isIndieLicense || currentState.loginStatus.isBusinessLicense);
        public bool TrialLicense => currentState.loginStatus != null && (currentState.loginStatus?.isTrial == true);
        
        private Vector2 _patchedMethodsScrollPos;
        private Vector2 _runTabScrollPos;

        private string promoCodeError;
        private MessageType promoCodeErrorType;
        private bool promoCodeActivatedThisSession;
        
        public HotReloadRunTab(HotReloadWindow window) : base(window, "Run", "forward", "Run and monitor the current Hot Reload session.") { }

        public override void OnGUI() {
            using(new EditorGUILayout.VerticalScope()) {
                OnGUICore();
            }
        }

        internal static bool ShouldRenderConsumption(HotReloadRunTabState currentState) => (currentState.running && !currentState.starting && !currentState.stopping && currentState.loginStatus?.isLicensed != true && currentState.loginStatus?.isFree != true && !EditorCodePatcher.LoginNotRequired) && !(currentState.loginStatus == null || currentState.loginStatus.isFree);
        
        void OnGUICore() {
            using (var scope = new EditorGUILayout.ScrollViewScope(_runTabScrollPos, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUILayout.MaxHeight(Math.Max(HotReloadWindowStyles.windowScreenHeight, 800)), GUILayout.MaxWidth(Math.Max(HotReloadWindowStyles.windowScreenWidth, 800)))) {
                _runTabScrollPos.x = scope.scrollPosition.x;
                _runTabScrollPos.y = scope.scrollPosition.y;
                using (new EditorGUILayout.VerticalScope(HotReloadWindowStyles.DynamiSection)) {
                    if (HotReloadWindowStyles.windowScreenWidth > Constants.UpgradeLicenseNoteHideWidth
                        && HotReloadWindowStyles.windowScreenHeight > Constants.UpgradeLicenseNoteHideHeight
                    ) {
                        RenderUpgradeLicenseNote(currentState, HotReloadWindowStyles.UpgradeLicenseButtonStyle);
                    }

                    RenderIndicationPanel();

                    if (CanRenderBars(currentState)) {
                        RenderBars(currentState);
                        // clear red dot next time button shows
                        HotReloadState.ShowingRedDot = false;
                    }
                }
            }

            // At the end to not fuck up rendering https://answers.unity.com/questions/400454/argumentexception-getting-control-0s-position-in-a-1.html
            var renderStart = !EditorCodePatcher.Running && !EditorCodePatcher.Starting && !currentState.requestingDownloadAndRun && currentState.redeemStage == RedeemStage.None;
            var e = Event.current;
            if (renderStart && e.type == EventType.KeyUp
                && (e.keyCode == KeyCode.Return
                    || e.keyCode == KeyCode.KeypadEnter)
            ) {
                EditorCodePatcher.DownloadAndRun().Forget();
            }
        }

        internal static void RenderUpgradeLicenseNote(HotReloadRunTabState currentState, GUIStyle style) {
            var isIndie = RedeemLicenseHelper.I.RegistrationOutcome == RegistrationOutcome.Indie
                || EditorCodePatcher.licenseType == UnityLicenseType.UnityPersonalPlus;

            if (RedeemLicenseHelper.I.RegistrationOutcome == RegistrationOutcome.Business
                && currentState.loginStatus?.isBusinessLicense != true
                && EditorCodePatcher.Running
                && (PackageConst.IsAssetStoreBuild || HotReloadPrefs.RateAppShown)
            ) {
                // Warn asset store users they need to buy a business license
                // Website users get reminded after using Hot Reload for 5+ days
                RenderBusinessLicenseInfo(style);
            } else if (isIndie
                && HotReloadPrefs.RateAppShown
                && !PackageConst.IsAssetStoreBuild
                && EditorCodePatcher.Running
                && currentState.loginStatus?.isBusinessLicense != true
                && currentState.loginStatus?.isIndieLicense != true
            ) {
                // Reminder users they need to buy an indie license
                RenderIndieLicenseInfo(style);
            }
        }
        
        internal static bool CanRenderBars(HotReloadRunTabState currentState) {
            return HotReloadWindowStyles.windowScreenHeight > Constants.EventsListHideHeight
                && HotReloadWindowStyles.windowScreenWidth > Constants.EventsListHideWidth
                && !currentState.starting
                && !currentState.stopping
                && !currentState.requestingDownloadAndRun
            ;
        }
        
        static Texture2D GetFoldoutIcon(AlertEntry alertEntry) {
            InvertibleIcon alertIcon = InvertibleIcon.FoldoutClosed;
            if (HotReloadTimelineHelper.expandedEntries.Contains(alertEntry)) {
                alertIcon = InvertibleIcon.FoldoutOpen;
            }
            return GUIHelper.GetInvertibleIcon(alertIcon);
        }
        
        static void ToggleEntry(AlertEntry alertEntry) {
            if (HotReloadTimelineHelper.expandedEntries.Contains(alertEntry)) {
                HotReloadTimelineHelper.expandedEntries.Remove(alertEntry);
            } else {
                HotReloadTimelineHelper.expandedEntries.Add(alertEntry);
            }
        }
        
        static void RenderEntries(TimelineType timelineType) {
            List<AlertEntry> alertEntries;
            
            alertEntries = timelineType == TimelineType.Suggestions ? HotReloadTimelineHelper.Suggestions : HotReloadTimelineHelper.EventsTimeline;

            bool skipChildren = false;
            for (int i = 0; i < alertEntries.Count; i++) {
                var alertEntry = alertEntries[i];
                if (i > HotReloadTimelineHelper.maxVisibleEntries && alertEntry.entryType != EntryType.Child) {
                    break;
                }
                if (timelineType != TimelineType.Suggestions) {
                    if (alertEntry.entryType != EntryType.Child
                        && !enabledFilters.Contains(alertEntry.alertType)
                    ) {
                        skipChildren = true;
                        continue;
                    } else if (alertEntry.entryType == EntryType.Child && skipChildren) {
                        continue;
                    } else {
                        skipChildren = false;
                    }
                }
                
                EntryType entryType = alertEntry.entryType;

                string title = $" {alertEntry.title}{(!string.IsNullOrEmpty(alertEntry.shortDescription) ? $": {alertEntry.shortDescription}": "")}";
                Texture2D icon = null;
                GUIStyle style;
                if (entryType != EntryType.Child) {
                    icon = GUIHelper.GetLocalIcon(HotReloadTimelineHelper.alertIconString[alertEntry.iconType]);
                }
                if (entryType == EntryType.Child) {
                    style = HotReloadWindowStyles.ChildBarStyle;
                } else if (entryType == EntryType.Foldout) {
                    style = HotReloadWindowStyles.FoldoutBarStyle;
                } else {
                    style = HotReloadWindowStyles.BarStyle;
                }

                Rect startRect;
                using (new EditorGUILayout.HorizontalScope()) {
                    GUILayout.Space(0);
                    Rect spaceRect = GUILayoutUtility.GetLastRect();
                    // entry header foldout arrow
                    if (entryType == EntryType.Foldout) {
                        GUI.Label(new Rect(spaceRect.x + 3, spaceRect.y, 20, 20), new GUIContent(GetFoldoutIcon(alertEntry)));
                    } else if (entryType == EntryType.Child) {
                        GUI.Label(new Rect(spaceRect.x + 26, spaceRect.y + 2, 20, 20), new GUIContent(GetFoldoutIcon(alertEntry)));
                    }
                    // a workaround to limit the width of the label
                    GUILayout.Label(new GUIContent(""), style);
                    startRect = GUILayoutUtility.GetLastRect();
                    GUI.Label(startRect, new GUIContent(title, icon), style);
                }

                bool clickableDescription = (alertEntry.title == "Unsupported change" || alertEntry.title == "Compile error" || alertEntry.title == "Failed applying patch to method") && alertEntry.alertData.alertEntryType != AlertEntryType.InlinedMethod;
                
                if (HotReloadTimelineHelper.expandedEntries.Contains(alertEntry) || alertEntry.alertType == AlertType.CompileError) {
                    using (new EditorGUILayout.VerticalScope()) {
                        using (new EditorGUILayout.HorizontalScope()) {
                            using (new EditorGUILayout.VerticalScope(entryType == EntryType.Child ? HotReloadWindowStyles.ChildEntryBoxStyle : HotReloadWindowStyles.EntryBoxStyle)) {
                                if (alertEntry.alertType == AlertType.Suggestion || !clickableDescription) {
                                    GUILayout.Label(alertEntry.description, HotReloadWindowStyles.LabelStyle);
                                }
                                if (alertEntry.actionData != null) {
                                    alertEntry.actionData.Invoke();
                                }
                                GUILayout.Space(5f);
                            }
                        }
                    }
                }
                
                // remove button
                if (timelineType == TimelineType.Suggestions && alertEntry.hasExitButton) {
                    var isClick = GUI.Button(new Rect(startRect.x + startRect.width - 20, startRect.y + 2, 20, 20), new GUIContent(GUIHelper.GetInvertibleIcon(InvertibleIcon.Close)), HotReloadWindowStyles.RemoveIconStyle);
                    if (isClick) {
                        HotReloadTimelineHelper.EventsTimeline.Remove(alertEntry);
                        var kind = HotReloadSuggestionsHelper.FindSuggestionKind(alertEntry);
                        if (kind != null) {
                            HotReloadSuggestionsHelper.SetSuggestionInactive((HotReloadSuggestionKind)kind);
                        }
                        _instantRepaint = true;
                    }
                }

                // Extend background to whole entry
                var endRect = GUILayoutUtility.GetLastRect();
                if (GUI.Button(new Rect(startRect) { height = endRect.y - startRect.y + endRect.height}, new GUIContent(""), HotReloadWindowStyles.BarBackgroundStyle) && (entryType == EntryType.Child || entryType == EntryType.Foldout)) {
                    ToggleEntry(alertEntry);
                }
        
                if (alertEntry.alertType != AlertType.Suggestion && HotReloadWindowStyles.windowScreenWidth > 400 && entryType != EntryType.Child) {
                    using (new EditorGUILayout.HorizontalScope()) {
                        var ago = (DateTime.Now - alertEntry.timestamp);
                        GUI.Label(new Rect(startRect.x + startRect.width - 60, startRect.y, 80, 20), ago.TotalMinutes < 1 ? "now" : $"{(ago.TotalHours > 1 ? $"{Math.Floor(ago.TotalHours)} h " : string.Empty)}{ago.Minutes} min", HotReloadWindowStyles.TimestampStyle);
                    }
                }
                
                GUILayout.Space(1f);
            }
            if (timelineType != TimelineType.Suggestions && HotReloadTimelineHelper.GetRunTabTimelineEventCount() > 40) { 
                GUILayout.Space(3f);
                GUILayout.Label(Constants.Only40EntriesShown, HotReloadWindowStyles.EmptyListText);
            }
        }

        private static List<AlertType> _enabledFilters;
        private static List<AlertType> enabledFilters {
            get {
                if (_enabledFilters == null) {
                    _enabledFilters = new List<AlertType>();
                }
                
                if (HotReloadPrefs.RunTabUnsupportedChangesFilter && !_enabledFilters.Contains(AlertType.UnsupportedChange))
                    _enabledFilters.Add(AlertType.UnsupportedChange);
                if (!HotReloadPrefs.RunTabUnsupportedChangesFilter && _enabledFilters.Contains(AlertType.UnsupportedChange))
                    _enabledFilters.Remove(AlertType.UnsupportedChange);
                
                if (HotReloadPrefs.RunTabCompileErrorFilter && !_enabledFilters.Contains(AlertType.CompileError))
                    _enabledFilters.Add(AlertType.CompileError);
                if (!HotReloadPrefs.RunTabCompileErrorFilter && _enabledFilters.Contains(AlertType.CompileError))
                    _enabledFilters.Remove(AlertType.CompileError);
                
                if (HotReloadPrefs.RunTabPartiallyAppliedPatchesFilter && !_enabledFilters.Contains(AlertType.PartiallySupportedChange))
                    _enabledFilters.Add(AlertType.PartiallySupportedChange);
                if (!HotReloadPrefs.RunTabPartiallyAppliedPatchesFilter && _enabledFilters.Contains(AlertType.PartiallySupportedChange))
                    _enabledFilters.Remove(AlertType.PartiallySupportedChange);
                
                if (HotReloadPrefs.RunTabUndetectedPatchesFilter && !_enabledFilters.Contains(AlertType.UndetectedChange))
                    _enabledFilters.Add(AlertType.UndetectedChange);
                if (!HotReloadPrefs.RunTabUndetectedPatchesFilter && _enabledFilters.Contains(AlertType.UndetectedChange))
                    _enabledFilters.Remove(AlertType.UndetectedChange);
                
                if (HotReloadPrefs.RunTabAppliedPatchesFilter && !_enabledFilters.Contains(AlertType.AppliedChange))
                    _enabledFilters.Add(AlertType.AppliedChange);
                if (!HotReloadPrefs.RunTabAppliedPatchesFilter && _enabledFilters.Contains(AlertType.AppliedChange))
                    _enabledFilters.Remove(AlertType.AppliedChange);
                    
                return _enabledFilters;
            }
        }
        
        private Vector2 suggestionsScroll;
        static GUILayoutOption[] timelineButtonOptions = new[] { GUILayout.Height(27), GUILayout.Width(100) };

        internal static void RenderBars(HotReloadRunTabState currentState) {
            if (currentState.suggestionCount > 0) {
                GUILayout.Space(5f);

                using (new EditorGUILayout.HorizontalScope(HotReloadWindowStyles.Section)) {
                    using (new EditorGUILayout.VerticalScope()) {
                        HotReloadPrefs.RunTabEventsSuggestionsFoldout = EditorGUILayout.Foldout(HotReloadPrefs.RunTabEventsSuggestionsFoldout, "", true, HotReloadWindowStyles.CustomFoldoutStyle);
                        GUILayout.Space(-23);
                        if (GUILayout.Button($"Suggestions ({currentState.suggestionCount.ToString()})", HotReloadWindowStyles.ClickableLabelBoldStyle, GUILayout.Height(27))) {
                            HotReloadPrefs.RunTabEventsSuggestionsFoldout = !HotReloadPrefs.RunTabEventsSuggestionsFoldout;
                        }
                        if (HotReloadPrefs.RunTabEventsSuggestionsFoldout) {
                            using (new EditorGUILayout.VerticalScope(HotReloadWindowStyles.Scroll)) {
                                RenderEntries(TimelineType.Suggestions);
                            }
                        }
                    }
                }
            }
            GUILayout.Space(5f);

            using (new EditorGUILayout.HorizontalScope(HotReloadWindowStyles.Section)) {
                using (new EditorGUILayout.VerticalScope()) {
                    HotReloadPrefs.RunTabEventsTimelineFoldout = EditorGUILayout.Foldout(HotReloadPrefs.RunTabEventsTimelineFoldout, "", true, HotReloadWindowStyles.CustomFoldoutStyle);
                    GUILayout.Space(-23);
                    if (GUILayout.Button("Timeline", HotReloadWindowStyles.ClickableLabelBoldStyle, timelineButtonOptions)) {
                        HotReloadPrefs.RunTabEventsTimelineFoldout = !HotReloadPrefs.RunTabEventsTimelineFoldout;
                    }
                    if (HotReloadPrefs.RunTabEventsTimelineFoldout) {
                        GUILayout.Space(-10);
                        var noteShown = HotReloadTimelineHelper.GetRunTabTimelineEventCount() == 0 || !currentState.running;
                        using (new EditorGUILayout.HorizontalScope()) {
                            if (noteShown) {
                                GUILayout.Space(2f);
                                using (new EditorGUILayout.VerticalScope()) {
                                    GUILayout.Space(2f);
                                    string text;
                                    if (currentState.redeemStage != RedeemStage.None) {
                                        text = "Complete registration before using Hot Reload";
                                    } else if (!currentState.running) {
                                        text = "Use the Start button to activate Hot Reload";
                                    } else if (enabledFilters.Count < 4 && HotReloadTimelineHelper.EventsTimeline.Count != 0) {
                                        text = "Enable filters to see events";
                                    } else {
                                        text = "Make code changes to see events";
                                    }
                                    GUILayout.Label(text, HotReloadWindowStyles.EmptyListText);
                                }
                                GUILayout.FlexibleSpace();
                            } else {
                                GUILayout.FlexibleSpace();
                                if (HotReloadTimelineHelper.EventsTimeline.Count > 0 && GUILayout.Button("Clear")) {
                                    HotReloadTimelineHelper.ClearEntries();
                                    if (HotReloadWindow.Current) {
                                        HotReloadWindow.Current.Repaint();
                                    }
                                }
                                GUILayout.Space(3);
                            }
                        }
                        if (!noteShown) {
                            GUILayout.Space(2f);
                            using (new EditorGUILayout.VerticalScope()) {
                                RenderEntries(TimelineType.Timeline);
                            }
                        }
                    }
                }
            }
        }
            
        internal static void RenderConsumption(LoginStatusResponse loginStatus) {
            if (loginStatus == null) {
                return;
            }
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField($"Hot Reload Limited", HotReloadWindowStyles.H3CenteredTitleStyle);
            EditorGUILayout.Space();
            if (loginStatus.consumptionsUnavailableReason == ConsumptionsUnavailableReason.NetworkUnreachable) {
                EditorGUILayout.HelpBox("Something went wrong. Please check your internet connection.", MessageType.Warning);
            } else if (loginStatus.consumptionsUnavailableReason == ConsumptionsUnavailableReason.UnrecoverableError) {
                EditorGUILayout.HelpBox("Something went wrong. Please contact support if the issue persists.", MessageType.Error);
            } else if (loginStatus.freeSessionFinished) {
                var now = DateTime.UtcNow;
                var sessionRefreshesAt = (now.AddDays(1).Date - now).Add(TimeSpan.FromMinutes(5));
                var sessionRefreshString = $"Next Session: {(sessionRefreshesAt.Hours > 0 ? $"{sessionRefreshesAt.Hours}h " : "")}{sessionRefreshesAt.Minutes}min";
                HotReloadGUIHelper.HelpBox(sessionRefreshString, MessageType.Warning, fontSize: 11);
            } else if (loginStatus.freeSessionRunning && loginStatus.freeSessionEndTime != null) {
                var sessionEndsAt = loginStatus.freeSessionEndTime.Value - DateTime.Now;
                var sessionString = $"Daily Session: {(sessionEndsAt.Hours > 0 ? $"{sessionEndsAt.Hours}h " : "")}{sessionEndsAt.Minutes}min Left";
                HotReloadGUIHelper.HelpBox(sessionString, MessageType.Info, fontSize: 11);
            } else if (loginStatus.freeSessionEndTime == null) {
                HotReloadGUIHelper.HelpBox("Daily Session: Make code changes to start", MessageType.Info, fontSize: 11);
            }
        }

        static bool _repaint;
        static bool _instantRepaint;
        static DateTime _lastRepaint;
        private EditorIndicationState.IndicationStatus _lastStatus;
        public override void Update() {
            if (EditorIndicationState.SpinnerActive) {
                _repaint = true;
            }
            if (EditorCodePatcher.DownloadRequired) {
                _repaint = true;
            }
            if (EditorIndicationState.IndicationIconPath == Spinner.SpinnerIconPath) {
                _repaint = true;
            }
            try {
                // workaround: hovering over non-buttons doesn't repain by default
                if (EditorWindow.mouseOverWindow == HotReloadWindow.Current) {
                    _repaint = true;
                }
                if (EditorWindow.mouseOverWindow
                    && EditorWindow.mouseOverWindow?.GetType() == typeof(PopupWindow)
                    && HotReloadEventPopup.I.open
                ) {
                    _repaint = true;
                }
            } catch (NullReferenceException) {
                // Unity randomly throws nullrefs when EditorWindow.mouseOverWindow gets accessed
            }
            if (_repaint && DateTime.UtcNow - _lastRepaint > TimeSpan.FromMilliseconds(33)) {
                _repaint = false;
                _instantRepaint = true;
            }
            // repaint on status change
            var status = EditorIndicationState.CurrentIndicationStatus;
            if (_lastStatus != status) {
                _lastStatus = status;
                _instantRepaint = true;
            }
            if (_instantRepaint) {
                Repaint();
                HotReloadEventPopup.I.Repaint();
                _instantRepaint = false;
                _repaint = false;
                _lastRepaint = DateTime.UtcNow;
            }
        }
        
        public static void RepaintInstant() {
            _instantRepaint = true;
        }

        private void RenderRecompileButton() {
            string recompileText = HotReloadWindowStyles.windowScreenWidth > Constants.RecompileButtonTextHideWidth ? " Recompile" : "";
            var recompileButton = new GUIContent(recompileText, GUIHelper.GetInvertibleIcon(InvertibleIcon.Recompile));
            if (!GUILayout.Button(recompileButton, HotReloadWindowStyles.RecompileButton)) {
                return;
            }
            RecompileWithChecks();
        }

        public static void RecompileWithChecks() {
            var firstDialoguePass = HotReloadPrefs.RecompileDialogueShown
                || EditorUtility.DisplayDialog(
                    title: "Hot Reload auto-applies changes",
                    message: "Using the Recompile button is only necessary when Hot Reload fails to apply your changes. \n\nDo you wish to proceed?",
                    ok: "Recompile",
                    cancel: "Not now");
            HotReloadPrefs.RecompileDialogueShown = true;
            if (!firstDialoguePass) {
                return;
            }
            if (!ConfirmExitPlaymode("Using the Recompile button will stop Play Mode.\n\nDo you wish to proceed?")) {
                return;
            }
            Recompile();
        }

        #if UNITY_2020_1_OR_NEWER
        public static void SwitchToDebugMode() {
            CompilationPipeline.codeOptimization = CodeOptimization.Debug;
            HotReloadRunTab.Recompile();
            HotReloadSuggestionsHelper.SetSuggestionInactive(HotReloadSuggestionKind.SwitchToDebugModeForInlinedMethods);
        }
        #endif

        public static bool ConfirmExitPlaymode(string message) {
            return !Application.isPlaying
                || EditorUtility.DisplayDialog(
                    title: "Stop Play Mode and Recompile?",
                    message: message,
                    ok: "Stop and Recompile",
                    cancel: "Cancel");
        }

        public static bool recompiling;
        public static void Recompile() {
            recompiling = true;
            EditorApplication.isPlaying = false;

            CompileMethodDetourer.Reset();
            AssetDatabase.Refresh();
            // This forces the recompilation if no changes were made.
            // This is better UX because otherwise the recompile button is unresponsive
            // which can be extra annoying if there are compile error entries in the list
            if (!EditorApplication.isCompiling) {
                CompilationPipeline.RequestScriptCompilation();
            }
        }
        
        private void RenderIndicationButtons() {
            if (currentState.requestingDownloadAndRun || currentState.starting || currentState.stopping || currentState.redeemStage != RedeemStage.None) {
                return;
            }
            
            if (!currentState.running && (currentState.startupProgress?.Item1 ?? 0) == 0) {
                string startText = HotReloadWindowStyles.windowScreenWidth > Constants.StartButtonTextHideWidth ? " Start" : "";
                if (GUILayout.Button(new GUIContent(startText, GUIHelper.GetInvertibleIcon(InvertibleIcon.Start)), HotReloadWindowStyles.StartButton)) {
                    EditorCodePatcher.DownloadAndRun().Forget();
                }
            } else if (currentState.running && !currentState.starting) {
                if (HotReloadWindowStyles.windowScreenWidth > 150) {
                    RenderRecompileButton();
                }
                string stopText = HotReloadWindowStyles.windowScreenWidth > Constants.StartButtonTextHideWidth ? " Stop" : "";
                if (GUILayout.Button(new GUIContent(stopText, GUIHelper.GetInvertibleIcon(InvertibleIcon.Stop)), HotReloadWindowStyles.StopButton)) {
                    if (!EditorCodePatcher.StoppedServerRecently()) {
                        EditorCodePatcher.StopCodePatcher().Forget();
                    }
                }
            }
        }

        void RenderIndicationPanel() {
            using (new EditorGUILayout.HorizontalScope(HotReloadWindowStyles.SectionInnerBox)) {
                RenderIndication();
                if (HotReloadWindowStyles.windowScreenWidth > Constants.IndicationTextHideWidth) {
                    GUILayout.FlexibleSpace();
                }
                RenderIndicationButtons();
                if (HotReloadWindowStyles.windowScreenWidth <= Constants.IndicationTextHideWidth) {
                    GUILayout.FlexibleSpace();
                }
            }
            if (currentState.requestingDownloadAndRun || currentState.starting) {
                RenderProgressBar();
            }
            if (HotReloadWindowStyles.windowScreenWidth > Constants.ConsumptionsHideWidth
                && HotReloadWindowStyles.windowScreenHeight > Constants.ConsumptionsHideHeight
            ) {
                RenderLicenseInfo(currentState);
            }
        }

        internal static void RenderLicenseInfo(HotReloadRunTabState currentState) {
            var showRedeem = currentState.redeemStage != RedeemStage.None;
            var showConsumptions = ShouldRenderConsumption(currentState);
            if (!showConsumptions && !showRedeem) {
                return;
            }
            using (new EditorGUILayout.VerticalScope()) {
                // space needed only for consumptions because of Stop/Start button's margin
                if (showConsumptions) {
                    GUILayout.Space(6);
                }
                using (new EditorGUILayout.VerticalScope(HotReloadWindowStyles.Section)) {
                    if (showRedeem) {
                        RedeemLicenseHelper.I.RenderStage(currentState);
                    } else {
                        RenderConsumption(currentState.loginStatus);
                        GUILayout.Space(10);
                        RenderLicenseInfo(currentState, currentState.loginStatus);
                        RenderLicenseButtons(currentState);
                        GUILayout.Space(10);
                    }
                }
                GUILayout.Space(6);
            }
        }
        
        private Spinner _spinner = new Spinner(85);
        private void RenderIndication() {
            using (new EditorGUILayout.HorizontalScope(HotReloadWindowStyles.IndicationBox)) {
                // icon box
                if (HotReloadWindowStyles.windowScreenWidth <= Constants.IndicationTextHideWidth) {
                    GUILayout.FlexibleSpace();
                }

                using (new EditorGUILayout.HorizontalScope(HotReloadWindowStyles.IndicationHelpBox)) {
                    var text = HotReloadWindowStyles.windowScreenWidth > Constants.IndicationTextHideWidth ? $"  {currentState.indicationStatusText}" : "";
                    if (currentState.indicationIconPath == Spinner.SpinnerIconPath) {
                        GUILayout.Label(new GUIContent(text, _spinner.GetIcon()), style: HotReloadWindowStyles.IndicationIcon);
                    } else if (currentState.indicationIconPath != null) {
                        var style = HotReloadWindowStyles.IndicationIcon;
                        if (HotReloadTimelineHelper.alertIconString.ContainsValue(currentState.indicationIconPath)) {
                            style = HotReloadWindowStyles.IndicationAlertIcon;
                        }
                        GUILayout.Label(new GUIContent(text, GUIHelper.GetLocalIcon(currentState.indicationIconPath)), style);
                    }
                } 
            }
        }
        
        static GUIStyle _openSettingsStyle;
        static GUIStyle openSettingsStyle => _openSettingsStyle ?? (_openSettingsStyle = new GUIStyle(GUI.skin.button) {
            fontStyle = FontStyle.Normal,
            fixedHeight = 25,
        });
        
        static GUILayoutOption[] _bigButtonHeight;
        public static GUILayoutOption[] bigButtonHeight => _bigButtonHeight ?? (_bigButtonHeight = new [] {GUILayout.Height(25)});
        
        private static GUIContent indieLicenseContent;
        private static GUIContent businessLicenseContent;

        internal static void RenderLicenseStatusInfo(HotReloadRunTabState currentState, LoginStatusResponse loginStatus, bool allowHide = true, bool verbose = false) {
            string message = null;
            MessageType messageType = default(MessageType);
            Action customGUI = null;
            GUIContent content = null;
            if (loginStatus == null) {
                // no info
            } else if (loginStatus.lastLicenseError != null) {
                messageType = !loginStatus.freeSessionFinished ? MessageType.Warning : MessageType.Error;
                message = GetMessageFromError(currentState, loginStatus.lastLicenseError);
            } else if (loginStatus.isTrial && !PackageConst.IsAssetStoreBuild) {
                message = $"Using Trial license, valid until {loginStatus.licenseExpiresAt.ToShortDateString()}";
                messageType = MessageType.Info;
            } else if (loginStatus.isIndieLicense) {
                if (verbose) {
                    message = " Indie license active";
                    messageType = MessageType.Info;
                    customGUI = () => {
                        if (loginStatus.licenseExpiresAt.Date != DateTime.MaxValue.Date) {
                            EditorGUILayout.LabelField($"License will renew on {loginStatus.licenseExpiresAt.ToShortDateString()}.");
                            EditorGUILayout.Space();
                        }
                        using (new GUILayout.HorizontalScope()) {
                            HotReloadAboutTab.manageLicenseButton.OnGUI();
                            HotReloadAboutTab.manageAccountButton.OnGUI();
                        }
                        EditorGUILayout.Space();
                    };
                    if (indieLicenseContent == null) {
                        indieLicenseContent = new GUIContent(message, EditorGUIUtility.FindTexture("TestPassed"));
                    }
                    content = indieLicenseContent;
                }
            } else if (loginStatus.isBusinessLicense) {
                if (verbose) {
                    message = " Business license active";
                    messageType = MessageType.Info;
                    if (businessLicenseContent == null) {
                        businessLicenseContent = new GUIContent(message, EditorGUIUtility.FindTexture("TestPassed"));
                    }
                    content = businessLicenseContent;
                    customGUI = () => {
                        using (new GUILayout.HorizontalScope()) {
                            HotReloadAboutTab.manageLicenseButton.OnGUI();
                            HotReloadAboutTab.manageAccountButton.OnGUI();
                        }
                        EditorGUILayout.Space();
                    };
                }
            }

            if (messageType != MessageType.Info && HotReloadPrefs.ErrorHidden && allowHide) {
                return;
            }
            if (message != null) {
                if (messageType != MessageType.Info) {
                    using(new EditorGUILayout.HorizontalScope()) {
                        EditorGUILayout.HelpBox(message, messageType);
                        var style = HotReloadWindowStyles.HideButtonStyle;
                        if (Event.current.type == EventType.Repaint) {
                            style.fixedHeight = GUILayoutUtility.GetLastRect().height;
                        }
                        if (allowHide) {
                            if (GUILayout.Button("Hide", style)) {
                                HotReloadPrefs.ErrorHidden = true;
                            }
                        }
                    }
                } else if (content != null) {
                    EditorGUILayout.LabelField(content);
                    EditorGUILayout.Space();
                } else {
                    EditorGUILayout.LabelField(message);
                    EditorGUILayout.Space();
                }
                customGUI?.Invoke();
            }
        }

        const string assetStoreProInfo = "Unity Pro/Enterprise users from company with your number of employees require a Business license. Please upgrade your license on our website.";
        internal static void RenderBusinessLicenseInfo(GUIStyle style) {
            GUILayout.Space(8);
            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.HelpBox(assetStoreProInfo, MessageType.Info);
                if (Event.current.type == EventType.Repaint) {
                    style.fixedHeight = GUILayoutUtility.GetLastRect().height;
                }
                if (GUILayout.Button("Upgrade", style)) {
                    Application.OpenURL(Constants.ProductPurchaseBusinessURL);
                }
            }
        }
        
        internal static void RenderIndieLicenseInfo(GUIStyle style) {
            string message;
            if (EditorCodePatcher.licenseType == UnityLicenseType.UnityPersonalPlus) {
                message = "Unity Plus users require an Indie license. Please upgrade your license on our website.";
            } else if (EditorCodePatcher.licenseType == UnityLicenseType.UnityPro) {
                message = "Unity Pro/Enterprise users from company with your number of employees require an Indie license. Please upgrade your license on our website.";
            } else {
                return;
            }
            GUILayout.Space(8);
            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.HelpBox(message, MessageType.Info);
                if (Event.current.type == EventType.Repaint) {
                    style.fixedHeight = GUILayoutUtility.GetLastRect().height;
                }
                if (GUILayout.Button("Upgrade", style)) {
                    Application.OpenURL(Constants.ProductPurchaseURL);
                }
            }
        }

        const string GetLicense = "Get License";
        const string ContactSupport = "Contact Support";
        const string UpgradeLicense = "Upgrade License";
        const string ManageLicense = "Manage License";
        internal static Dictionary<string, LicenseErrorData> _licenseErrorData;
        internal static Dictionary<string, LicenseErrorData> LicenseErrorData => _licenseErrorData ?? (_licenseErrorData = new Dictionary<string, LicenseErrorData> {
            { "DeviceNotLicensedException", new LicenseErrorData(description: "Another device is using your license. Please reach out to customer support for assistance.", showSupportButton: true, supportButtonText: ContactSupport) },
            { "DeviceBlacklistedException", new LicenseErrorData(description: "You device has been blacklisted.") },
            { "DateHeaderInvalidException", new LicenseErrorData(description: $"Your license is not working because your computer's clock is incorrect. Please set the clock to the correct time to restore your license.") },
            { "DateTimeCheatingException", new LicenseErrorData(description: $"Your license is not working because your computer's clock is incorrect. Please set the clock to the correct time to restore your license.") },
            { "LicenseActivationException", new LicenseErrorData(description: "An error has occured while activating your license. Please contact customer support for assistance.", showSupportButton: true, supportButtonText: ContactSupport) },
            { "LicenseDeletedException", new LicenseErrorData(description: $"Your license has been deleted. Please contact customer support for assistance.", showBuyButton: true, buyButtonText: GetLicense, showSupportButton: true, supportButtonText: ContactSupport) },
            { "LicenseDisabledException", new LicenseErrorData(description: $"Your license has been disabled. Please contact customer support for assistance.", showBuyButton: true, buyButtonText: GetLicense, showSupportButton: true, supportButtonText: ContactSupport) },
            { "LicenseExpiredException", new LicenseErrorData(description: $"Your license has expired. Please renew your license subscription using the 'Upgrade License' button below and login with your email/password to activate your license.", showBuyButton: true, buyButtonText: UpgradeLicense, showManageLicenseButton: true, manageLicenseButtonText: ManageLicense) },
            { "LicenseInactiveException", new LicenseErrorData(description: $"Your license is currenty inactive. Please login with your email/password to activate your license.") },
            { "LocalLicenseException", new LicenseErrorData(description: $"Your license file was damaged or corrupted. Please login with your email/password to refresh your license file.") },
            // Note: obsolete
            { "MissingParametersException", new LicenseErrorData(description: "An account already exists for this device. Please login with your existing email/password.", showBuyButton: true, buyButtonText: GetLicense) },
            { "NetworkException", new LicenseErrorData(description: "There is an issue connecting to our servers. Please check your internet connection or contact customer support if the issue persists.", showSupportButton: true, supportButtonText: ContactSupport) },
            { "TrialLicenseExpiredException", new LicenseErrorData(description: $"Your trial has expired. Activate a license with unlimited usage or continue using the Free version. View available plans on our website.", showBuyButton: true, buyButtonText: UpgradeLicense) },
            { "InvalidCredentialException", new LicenseErrorData(description: "Incorrect email/password. You can find your initial password in the sign-up email.") },
            // Note: activating free trial with email is not supported anymore. This error shouldn't happen which is why we should rather user the fallback
            // { "LicenseNotFoundException", new LicenseErrorData(description: "The account you're trying to access doesn't seem to exist yet. Please enter your email address to create a new account and receive a trial license.", showLoginButton: true, loginButtonText: CreateAccount) },
            { "LicenseIncompatibleException", new LicenseErrorData(description: "Please upgrade your license to continue using hotreload with Unity Pro.", showManageLicenseButton: true, manageLicenseButtonText: ManageLicense) },
        });
        internal static LicenseErrorData defaultLicenseErrorData = new LicenseErrorData(description: "We apologize, an error happened while verifying your license. Please reach out to customer support for assistance.", showSupportButton: true, supportButtonText: ContactSupport);

        internal static string GetMessageFromError(HotReloadRunTabState currentState, string error) {
            if (PackageConst.IsAssetStoreBuild && error == "TrialLicenseExpiredException") {
                return assetStoreProInfo;
            }
            return GetLicenseErrorDataOrDefault(currentState, error).description;
        }
        
        internal static LicenseErrorData GetLicenseErrorDataOrDefault(HotReloadRunTabState currentState, string error) {
            if (currentState.loginStatus?.isFree == true) {
                return default(LicenseErrorData);
            }
            if (currentState.loginStatus == null || string.IsNullOrEmpty(error) && (!currentState.loginStatus.isLicensed || currentState.loginStatus.isTrial)) {
                return new LicenseErrorData(null, showBuyButton: true, buyButtonText: GetLicense);
            }
            if (string.IsNullOrEmpty(error)) {
                return default(LicenseErrorData);
            }
            if (!LicenseErrorData.ContainsKey(error)) {
                return defaultLicenseErrorData;
            }
            return LicenseErrorData[error];
        }

        internal static void RenderBuyLicenseButton(string buyLicenseButton) {
            OpenURLButton.Render(buyLicenseButton, Constants.ProductPurchaseURL);
        }

        static void RenderLicenseActionButtons(HotReloadRunTabState currentState) {
            var errInfo = GetLicenseErrorDataOrDefault(currentState, currentState.loginStatus?.lastLicenseError);
            if (errInfo.showBuyButton || errInfo.showManageLicenseButton) {
                using(new EditorGUILayout.HorizontalScope()) {
                    if (errInfo.showBuyButton) {
                        RenderBuyLicenseButton(errInfo.buyButtonText);
                    }
                    if (errInfo.showManageLicenseButton && !HotReloadPrefs.ErrorHidden) {
                        OpenURLButton.Render(errInfo.manageLicenseButtonText, Constants.ManageLicenseURL);
                    }
                }
            }
            if (errInfo.showLoginButton && GUILayout.Button(errInfo.loginButtonText, openSettingsStyle)) {
                // show license section
                HotReloadWindow.Current.SelectTab(typeof(HotReloadSettingsTab));
                HotReloadWindow.Current.SettingsTab.FocusLicenseFoldout();
            }
            if (errInfo.showSupportButton && !HotReloadPrefs.ErrorHidden) {
                OpenURLButton.Render(errInfo.supportButtonText, Constants.ContactURL);
            }
            if (currentState.loginStatus?.lastLicenseError != null) {
                HotReloadAboutTab.reportIssueButton.OnGUI();
            }
        }
        
        internal static void RenderLicenseInfo(HotReloadRunTabState currentState, LoginStatusResponse loginStatus, bool verbose = false, bool allowHide = true, string overrideActionButton = null, bool showConsumptions = false) {
            HotReloadPrefs.ShowLogin = EditorGUILayout.Foldout(HotReloadPrefs.ShowLogin, "Hot Reload License", true, HotReloadWindowStyles.FoldoutStyle);
            if (HotReloadPrefs.ShowLogin) {
                EditorGUILayout.Space();
                if ((loginStatus?.isLicensed != true && showConsumptions) && !(loginStatus == null || loginStatus.isFree)) {
                    RenderConsumption(loginStatus);
                }
                RenderLicenseStatusInfo(currentState, loginStatus: loginStatus, allowHide: allowHide, verbose: verbose);

                RenderLicenseInnerPanel(currentState, overrideActionButton: overrideActionButton);
                
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
        }

        internal void RenderPromoCodes() {
            HotReloadPrefs.ShowPromoCodes = EditorGUILayout.Foldout(HotReloadPrefs.ShowPromoCodes, "Promo Codes", true, HotReloadWindowStyles.FoldoutStyle);
            if (!HotReloadPrefs.ShowPromoCodes) {
                return;
            }
            if (promoCodeActivatedThisSession) {
                EditorGUILayout.HelpBox($"Your promo code has been successfully activated. Free trial has been extended by 3 months.", MessageType.Info);
            } else {
                if (promoCodeError != null && promoCodeErrorType != MessageType.None) {
                    EditorGUILayout.HelpBox(promoCodeError, promoCodeErrorType);
                }
                EditorGUILayout.LabelField("Promo code");
                _pendingPromoCode = EditorGUILayout.TextField(_pendingPromoCode);
                EditorGUILayout.Space();

                using (new EditorGUI.DisabledScope(_requestingActivatePromoCode)) {
                    if (GUILayout.Button("Activate promo code", HotReloadRunTab.bigButtonHeight)) {
                        RequestActivatePromoCode().Forget();
                    }
                }
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
        
        private async Task RequestActivatePromoCode() {
            _requestingActivatePromoCode = true;
            try {
                var resp = await RequestHelper.RequestActivatePromoCode(_pendingPromoCode);
                if (resp != null && resp.error == null) {
                    promoCodeActivatedThisSession = true;
                } else {
                    var requestError = resp?.error ?? "Network error";
                    var errorType = ToErrorType(requestError);
                    promoCodeError = ToPrettyErrorMessage(errorType);
                    promoCodeErrorType = ToMessageType(errorType);
                }
            } finally {
                _requestingActivatePromoCode = false;
            }
        }

        PromoCodeErrorType ToErrorType(string error) {
            switch (error) {
                case "Input is missing":           return PromoCodeErrorType.MISSING_INPUT;
                case "only POST is supported":     return PromoCodeErrorType.INVALID_HTTP_METHOD;
                case "body is not a valid json":   return PromoCodeErrorType.BODY_INVALID;
                case "Promo code is not found":    return PromoCodeErrorType.PROMO_CODE_NOT_FOUND;
                case "Promo code already claimed": return PromoCodeErrorType.PROMO_CODE_CLAIMED;
                case "Promo code expired":         return PromoCodeErrorType.PROMO_CODE_EXPIRED;
                case "License not found":          return PromoCodeErrorType.LICENSE_NOT_FOUND;
                case "License is not a trial":     return PromoCodeErrorType.LICENSE_NOT_TRIAL;
                case "License already extended":   return PromoCodeErrorType.LICENSE_ALREADY_EXTENDED;
                case "conditionalCheckFailed":     return PromoCodeErrorType.CONDITIONAL_CHECK_FAILED;
            }
            if (error.Contains("Updating License Failed with error")) {
                return PromoCodeErrorType.UPDATING_LICENSE_FAILED;
            } else if (error.Contains("Unknown exception")) {
                return PromoCodeErrorType.UNKNOWN_EXCEPTION;
            } else if (error.Contains("Unsupported path")) {
                return PromoCodeErrorType.UNSUPPORTED_PATH;
            }
            return PromoCodeErrorType.NONE;
        }

        string ToPrettyErrorMessage(PromoCodeErrorType errorType) {
            var defaultMsg = "We apologize, an error happened while activating your promo code. Please reach out to customer support for assistance.";
            switch (errorType) {
                case PromoCodeErrorType.MISSING_INPUT:
                case PromoCodeErrorType.INVALID_HTTP_METHOD:
                case PromoCodeErrorType.BODY_INVALID:
                case PromoCodeErrorType.UNKNOWN_EXCEPTION:
                case PromoCodeErrorType.UNSUPPORTED_PATH:
                case PromoCodeErrorType.LICENSE_NOT_FOUND:
                case PromoCodeErrorType.UPDATING_LICENSE_FAILED:
                case PromoCodeErrorType.LICENSE_NOT_TRIAL:
                    return defaultMsg;
                case PromoCodeErrorType.PROMO_CODE_NOT_FOUND:     return "Your promo code is invalid. Please ensure that you have entered the correct promo code.";
                case PromoCodeErrorType.PROMO_CODE_CLAIMED:       return "Your promo code has already been used.";
                case PromoCodeErrorType.PROMO_CODE_EXPIRED:       return "Your promo code has expired.";
                case PromoCodeErrorType.LICENSE_ALREADY_EXTENDED: return "Your license has already been activated with a promo code. Only one promo code activation per license is allowed.";
                case PromoCodeErrorType.CONDITIONAL_CHECK_FAILED: return "We encountered an error while activating your promo code. Please try again. If the issue persists, please contact our customer support team for assistance.";
                case PromoCodeErrorType.NONE:                     return "There is an issue connecting to our servers. Please check your internet connection or contact customer support if the issue persists.";
                default:                                          return defaultMsg;
            }
        }

        MessageType ToMessageType(PromoCodeErrorType errorType) {
            switch (errorType) {
                case PromoCodeErrorType.MISSING_INPUT:            return MessageType.Error;
                case PromoCodeErrorType.INVALID_HTTP_METHOD:      return MessageType.Error;
                case PromoCodeErrorType.BODY_INVALID:             return MessageType.Error;
                case PromoCodeErrorType.PROMO_CODE_NOT_FOUND:     return MessageType.Warning;
                case PromoCodeErrorType.PROMO_CODE_CLAIMED:       return MessageType.Warning;
                case PromoCodeErrorType.PROMO_CODE_EXPIRED:       return MessageType.Warning;
                case PromoCodeErrorType.LICENSE_NOT_FOUND:        return MessageType.Error;
                case PromoCodeErrorType.LICENSE_NOT_TRIAL:        return MessageType.Error;
                case PromoCodeErrorType.LICENSE_ALREADY_EXTENDED: return MessageType.Warning;
                case PromoCodeErrorType.UPDATING_LICENSE_FAILED:  return MessageType.Error;
                case PromoCodeErrorType.CONDITIONAL_CHECK_FAILED: return MessageType.Error;
                case PromoCodeErrorType.UNKNOWN_EXCEPTION:        return MessageType.Error;
                case PromoCodeErrorType.UNSUPPORTED_PATH:         return MessageType.Error;
                case PromoCodeErrorType.NONE:                     return MessageType.Error;
                default:                                          return MessageType.Error;
            }
        }

        public static void RenderLicenseButtons(HotReloadRunTabState currentState) {
            RenderLicenseActionButtons(currentState);
        }

        internal static void RenderLicenseInnerPanel(HotReloadRunTabState currentState, string overrideActionButton = null, bool renderLogout = true) {
            EditorGUILayout.LabelField("Email");
            GUI.SetNextControlName("email");
            _pendingEmail = EditorGUILayout.TextField(string.IsNullOrEmpty(_pendingEmail) ? HotReloadPrefs.LicenseEmail : _pendingEmail);
            _pendingEmail = _pendingEmail.Trim();

            EditorGUILayout.LabelField("Password");
            GUI.SetNextControlName("password");
            _pendingPassword = EditorGUILayout.PasswordField(string.IsNullOrEmpty(_pendingPassword) ? HotReloadPrefs.LicensePassword : _pendingPassword);
            
            RenderSwitchAuthMode();
            
            var e = Event.current;
            using(new EditorGUI.DisabledScope(currentState.requestingLoginInfo)) {
                var btnLabel = overrideActionButton;
                if (String.IsNullOrEmpty(overrideActionButton)) {
                    btnLabel = "Login";
                }
                using (new EditorGUILayout.HorizontalScope()) {
                    var focusedControl = GUI.GetNameOfFocusedControl();
                    if (GUILayout.Button(btnLabel, bigButtonHeight)
                        || (focusedControl == "email" 
                            || focusedControl == "password") 
                        && e.type == EventType.KeyUp 
                        && (e.keyCode == KeyCode.Return 
                            || e.keyCode == KeyCode.KeypadEnter)
                    ) {
                        var error = ValidateEmail(_pendingEmail);
                        if (!string.IsNullOrEmpty(error)) {
                            _activateInfoMessage = new Tuple<string, MessageType>(error, MessageType.Warning);
                        } else if (string.IsNullOrEmpty(_pendingPassword)) {
                            _activateInfoMessage = new Tuple<string, MessageType>("Please enter your password.", MessageType.Warning);
                        } else {
                            HotReloadWindow.Current.SelectTab(typeof(HotReloadRunTab));

                            _activateInfoMessage = null;
                            if (RedeemLicenseHelper.I.RedeemStage == RedeemStage.Login) {
                                RedeemLicenseHelper.I.FinishRegistration(RegistrationOutcome.Indie);
                            }
                            if (!EditorCodePatcher.RequestingDownloadAndRun && !EditorCodePatcher.Running) {
                                LoginOnDownloadAndRun(new LoginData(email: _pendingEmail, password: _pendingPassword)).Forget();
                            } else {
                                EditorCodePatcher.RequestLogin(_pendingEmail, _pendingPassword).Forget();
                            }
                        }
                    }
                    if (renderLogout) {
                        RenderLogout(currentState);
                    }
                }
            }
            if (_activateInfoMessage != null && (e.type == EventType.Layout || e.type == EventType.Repaint)) {
                EditorGUILayout.HelpBox(_activateInfoMessage.Item1, _activateInfoMessage.Item2);
            }
        }

        public static string ValidateEmail(string email) {
            if (string.IsNullOrEmpty(email)) {
                return "Please enter your email address.";
            } else if (!EditorWindowHelper.IsValidEmailAddress(email)) {
                return "Please enter a valid email address.";
            } else if (email.Contains("+")) {
                return "Mail extensions (in a form of 'username+suffix@example.com') are not supported yet. Please provide your original email address (such as 'username@example.com' without '+suffix' part) as we're working on resolving this issue.";
            }
            return null;
        }

        public static void RenderLogout(HotReloadRunTabState currentState) {
            if (currentState.loginStatus?.isLicensed != true) {
                return;
            }
            if (GUILayout.Button("Logout", bigButtonHeight)) {
                HotReloadWindow.Current.SelectTab(typeof(HotReloadRunTab));
                if (!EditorCodePatcher.RequestingDownloadAndRun && !EditorCodePatcher.Running) {
                    LogoutOnDownloadAndRun().Forget();
                } else {
                    RequestLogout().Forget();
                }
            }
        }
        
        async static Task LoginOnDownloadAndRun(LoginData loginData = null) {
            var ok = await EditorCodePatcher.DownloadAndRun(loginData);
            if (ok && loginData != null) {
                HotReloadPrefs.ErrorHidden = false;
                HotReloadPrefs.LicenseEmail = loginData.email;
                HotReloadPrefs.LicensePassword = loginData.password;
            }
        }

        async static Task LogoutOnDownloadAndRun() {
            var ok = await EditorCodePatcher.DownloadAndRun();
            if (!ok) {
                return;
            }
            await RequestLogout();
        }

        private async static Task RequestLogout() {
            int i = 0;
            while (!EditorCodePatcher.Running && i < 100) {
                await Task.Delay(100);
                i++;
            }
            var resp = await RequestHelper.RequestLogout();
            if (!EditorCodePatcher.RequestingLoginInfo && resp != null) {
                EditorCodePatcher.HandleStatus(resp);
            }
        }

        private static void RenderSwitchAuthMode() {
            var color = EditorGUIUtility.isProSkin ? new Color32(0x3F, 0x9F, 0xFF, 0xFF) : new Color32(0x0F, 0x52, 0xD7, 0xFF); 
            if (HotReloadGUIHelper.LinkLabel("Forgot password?", 12, FontStyle.Normal, TextAnchor.MiddleLeft, color)) {
                if (EditorUtility.DisplayDialog("Recover password", "Use company code 'naughtycult' and the email you signed up with in order to recover your account.", "Open in browser", "Cancel")) {
                    Application.OpenURL(Constants.ForgotPasswordURL);
                }
            }
        }
        
        Texture2D _greenTextureLight;
        Texture2D _greenTextureDark;
        Texture2D GreenTexture => EditorGUIUtility.isProSkin 
            ? _greenTextureDark ? _greenTextureDark : (_greenTextureDark = MakeTexture(0.5f))
            : _greenTextureLight ? _greenTextureLight : (_greenTextureLight = MakeTexture(0.85f));
        
        private void RenderProgressBar() {
            if (currentState.downloadRequired && !currentState.downloadStarted) {
                return;
            }
            
            using(var scope = new EditorGUILayout.VerticalScope(HotReloadWindowStyles.MiddleCenterStyle)) {
                float progress;
                var bg = HotReloadWindowStyles.ProgressBarBarStyle.normal.background;
                try {
                    HotReloadWindowStyles.ProgressBarBarStyle.normal.background = GreenTexture;
                    var barRect = scope.rect;

                    barRect.height = 25;
                    if (currentState.downloadRequired) {
                        barRect.width = barRect.width - 65;
                        using (new EditorGUILayout.HorizontalScope()) {
                            progress = EditorCodePatcher.DownloadProgress;
                            EditorGUI.ProgressBar(barRect, Mathf.Clamp(progress, 0f, 1f), "");
                            if (GUI.Button(new Rect(barRect) { x = barRect.x + barRect.width + 5, height = barRect.height, width = 60 }, new GUIContent(" Info", GUIHelper.GetLocalIcon("alert_info")))) {
                                Application.OpenURL(Constants.AdditionalContentURL);
                            }
                        }
                    } else {
                        progress = EditorCodePatcher.Stopping ? 1 : Mathf.Clamp(EditorCodePatcher.StartupProgress?.Item1 ?? 0f, 0f, 1f);
                        EditorGUI.ProgressBar(barRect, progress, "");
                    }
                    GUILayout.Space(barRect.height);
                } finally {
                    HotReloadWindowStyles.ProgressBarBarStyle.normal.background = bg;
                }
            }
        }

        private Texture2D MakeTexture(float maxHue) {
            var width = 11;
            var height = 11;
            Color[] pix = new Color[width * height];
            for (int y = 0; y < height; y++) {
                var middle = Math.Ceiling(height / (double)2);
                var maxGreen = maxHue;
                var yCoord = y + 1;
                var green = maxGreen - Math.Abs(yCoord - middle) * 0.02;
                for (int x = 0; x < width; x++) {
                    pix[y * width + x] = new Color(0.1f, (float)green, 0.1f, 1.0f);
                }
            }
            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
        
        
        /*
        [MenuItem("codepatcher/restart")]
        public static void TestRestart() {
            CodePatcherCLI.Restart(Application.dataPath, false);
        }
        */
        
    }

    internal static class HotReloadGUIHelper {
        public static bool LinkLabel(string labelText, int fontSize, FontStyle fontStyle, TextAnchor alignment, Color? color = null) {
            var stl = EditorStyles.label;

            // copy
            var origSize = stl.fontSize;
            var origStyle = stl.fontStyle;
            var origAnchor = stl.alignment;
            var origColor = stl.normal.textColor;

            // temporarily modify the built-in style
            stl.fontSize = fontSize;
            stl.fontStyle = fontStyle;
            stl.alignment = alignment;
            stl.normal.textColor = color ?? origColor;
            stl.active.textColor = color ?? origColor;
            stl.focused.textColor = color ?? origColor;
            stl.hover.textColor = color ?? origColor;

            try {
                return GUILayout.Button(labelText, stl);
            }  finally{
                // set the editor style (stl) back to normal
                stl.fontSize = origSize;
                stl.fontStyle = origStyle;
                stl.alignment = origAnchor;
                stl.normal.textColor = origColor;
                stl.active.textColor = origColor;
                stl.focused.textColor = origColor;
                stl.hover.textColor = origColor;
            }
        }

        public static void HelpBox(string message, MessageType type, int fontSize) {
            var _fontSize = EditorStyles.helpBox.fontSize;
            try {
                EditorStyles.helpBox.fontSize = fontSize;
                EditorGUILayout.HelpBox(message, type);
            } finally {
                EditorStyles.helpBox.fontSize = _fontSize;
            }
        }
    }

    internal enum PromoCodeErrorType {
        NONE,
        MISSING_INPUT,
        INVALID_HTTP_METHOD,
        BODY_INVALID,
        PROMO_CODE_NOT_FOUND,
        PROMO_CODE_CLAIMED,
        PROMO_CODE_EXPIRED,
        LICENSE_NOT_FOUND,
        LICENSE_NOT_TRIAL,
        LICENSE_ALREADY_EXTENDED,
        UPDATING_LICENSE_FAILED,
        CONDITIONAL_CHECK_FAILED,
        UNKNOWN_EXCEPTION,
        UNSUPPORTED_PATH,
    }

    internal class LoginData {
        public readonly string email;
        public readonly string password;

        public LoginData(string email, string password) {
            this.email = email;
            this.password = password;
        }
    }
}

