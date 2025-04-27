
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using SingularityGroup.HotReload.DTO;
using SingularityGroup.HotReload.Editor.Cli;
using SingularityGroup.HotReload.Editor.Semver;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

[assembly: InternalsVisibleTo("SingularityGroup.HotReload.EditorSamples")]

namespace SingularityGroup.HotReload.Editor {
    class HotReloadWindow : EditorWindow {
        public static HotReloadWindow Current { get; private set; }

        List<HotReloadTabBase> tabs;
        List<HotReloadTabBase> Tabs => tabs ?? (tabs = new List<HotReloadTabBase> {
            RunTab,
            SettingsTab,
            AboutTab,
        });
        int selectedTab;

        internal static Vector2 scrollPos;
        
        static Timer timer; 


        HotReloadRunTab runTab;
        internal HotReloadRunTab RunTab => runTab ?? (runTab = new HotReloadRunTab(this));
        HotReloadSettingsTab settingsTab;
        internal HotReloadSettingsTab SettingsTab => settingsTab ?? (settingsTab = new HotReloadSettingsTab(this));
        HotReloadAboutTab aboutTab;
        internal HotReloadAboutTab AboutTab => aboutTab ?? (aboutTab = new HotReloadAboutTab(this));

        static ShowOnStartupEnum _showOnStartupOption;

        /// <summary>
        /// This token is cancelled when the EditorWindow is disabled.
        /// </summary>
        /// <remarks>
        /// Use it for all tasks.
        /// When token is cancelled, scripts are about to be recompiled and this will cause tasks to fail for weird reasons.
        /// </remarks>
        public CancellationToken cancelToken;
        CancellationTokenSource cancelTokenSource;

        static readonly PackageUpdateChecker packageUpdateChecker = new PackageUpdateChecker();

        [MenuItem("Window/Hot Reload/Open &#H")]
        internal static void Open() {
            // opening the window on CI systems was keeping Unity open indefinitely
            if (EditorWindowHelper.IsHumanControllingUs()) {
                if (Current) {
                    Current.Show();
                    Current.Focus();
                } else {
                    Current = GetWindow<HotReloadWindow>();
                }
            }
        }
        
        [MenuItem("Window/Hot Reload/Recompile")]
        internal static void Recompile() {
            HotReloadRunTab.Recompile();
        }

        void OnInterval(object o) {
            HotReloadRunTab.RepaintInstant();
        }

        void OnEnable() {
            if (timer == null) {
                timer = new Timer(OnInterval, null, 20 * 1000, 20 * 1000);
            }
            Current = this;
            if (cancelTokenSource != null) {
                cancelTokenSource.Cancel();
            }
            // Set min size initially so that full UI is visible
            if (!HotReloadPrefs.OpenedWindowAtLeastOnce) {
                this.minSize = new Vector2(Constants.RecompileButtonTextHideWidth + 1, Constants.EventsListHideHeight + 70);
                HotReloadPrefs.OpenedWindowAtLeastOnce = true;
            }
            cancelTokenSource = new CancellationTokenSource();
            cancelToken = cancelTokenSource.Token;
            
            this.titleContent = new GUIContent(" Hot Reload", GUIHelper.GetInvertibleIcon(InvertibleIcon.Logo));
            _showOnStartupOption = HotReloadPrefs.ShowOnStartup;

            packageUpdateChecker.StartCheckingForNewVersion();
        }

        void Update() {
            foreach (var tab in Tabs) {
                tab.Update();
            }
        }

        void OnDisable() {
            if (cancelTokenSource != null) {
                cancelTokenSource.Cancel();
                cancelTokenSource = null;
            }

            if (Current == this) {
                Current = null;
            }
            timer.Dispose();
            timer = null;
        }

        internal void SelectTab(Type tabType) {
            selectedTab = Tabs.FindIndex(x => x.GetType() == tabType);
        }
        
        public HotReloadRunTabState RunTabState { get; private set; }
        void OnGUI() {
            // TabState ensures rendering is consistent between Layout and Repaint calls
            // Without it errors like this happen:
            // ArgumentException: Getting control 2's position in a group with only 2 controls when doing repaint
            // See thread for more context: https://answers.unity.com/questions/17718/argumentexception-getting-control-2s-position-in-a.html
            if (Event.current.type == EventType.Layout) {
                RunTabState = HotReloadRunTabState.Current;
            }
            using(var scope = new EditorGUILayout.ScrollViewScope(scrollPos, false, false)) {
                scrollPos = scope.scrollPosition;
                // RenderDebug();
                RenderTabs();
            }
            GUILayout.FlexibleSpace(); // GUI below will be rendered on the bottom
            if (HotReloadWindowStyles.windowScreenHeight > 90)
                RenderBottomBar();
        }

        void RenderDebug() {
            if (GUILayout.Button("RESET WINDOW")) {
                OnDisable();

                RequestHelper.RequestLogin("test", "test", 1).Forget();

                HotReloadPrefs.LicenseEmail = null;
                HotReloadPrefs.ExposeServerToLocalNetwork = true;
                HotReloadPrefs.LicensePassword = null;
                HotReloadPrefs.LoggedBurstHint = false;
                HotReloadPrefs.DontShowPromptForDownload = false;
                HotReloadPrefs.RateAppShown = false;
                HotReloadPrefs.ActiveDays = string.Empty;
                HotReloadPrefs.LaunchOnEditorStart = false;
                HotReloadPrefs.ShowUnsupportedChanges = true;
                HotReloadPrefs.RedeemLicenseEmail = null;
                HotReloadPrefs.RedeemLicenseInvoice = null;
                OnEnable();
                File.Delete(EditorCodePatcher.serverDownloader.GetExecutablePath(HotReloadCli.controller));
                InstallUtility.DebugClearInstallState();
                InstallUtility.CheckForNewInstall();
                EditorPrefs.DeleteKey(Attribution.LastLoginKey);
                File.Delete(RedeemLicenseHelper.registerOutcomePath);

                CompileMethodDetourer.Reset();
                AssetDatabase.Refresh();
            }
        }

        internal static void RenderLogo(int width = 243) {
            var isDarkMode = HotReloadWindowStyles.IsDarkMode;
            var tex = Resources.Load<Texture>(isDarkMode ? "Logo_HotReload_DarkMode" : "Logo_HotReload_LightMode");
            //Can happen during player builds where Editor Resources are unavailable
            if(tex == null) {
                return;
            }
            var targetWidth = width;
            var targetHeight = 44;
            GUILayout.Space(4f);
            // background padding top and bottom
            float padding = 5f;
            // reserve layout space for the texture
            var backgroundRect = GUILayoutUtility.GetRect(targetWidth + padding, targetHeight + padding, HotReloadWindowStyles.LogoStyle);
            // draw the texture into that reserved space. First the bg then the logo.
            if (isDarkMode) {
                GUI.DrawTexture(backgroundRect, EditorTextures.DarkGray17, ScaleMode.StretchToFill);
            } else {
                GUI.DrawTexture(backgroundRect, EditorTextures.LightGray238, ScaleMode.StretchToFill);
            }
            
            var foregroundRect = backgroundRect;
            foregroundRect.yMin += padding;
            foregroundRect.yMax -= padding;
            // during player build (EditorWindow still visible), Resources.Load returns null
            if (tex) {
                GUI.DrawTexture(foregroundRect, tex, ScaleMode.ScaleToFit);
            }
        }

        int? collapsedTab;
        void RenderTabs() {
            using(new EditorGUILayout.VerticalScope(HotReloadWindowStyles.BoxStyle)) {
                if (HotReloadWindowStyles.windowScreenHeight > 210 && HotReloadWindowStyles.windowScreenWidth > 375) {
                    selectedTab = GUILayout.Toolbar(
                        selectedTab,
                        Tabs.Select(t =>
                            new GUIContent(t.Title.StartsWith(" ", StringComparison.Ordinal) ? t.Title : " " + t.Title,
                                t.Icon, t.Tooltip)).ToArray(),
                        GUILayout.Height(22f) // required, otherwise largest icon height determines toolbar height
                    );
                    if (collapsedTab != null) {
                        selectedTab = collapsedTab.Value;
                        collapsedTab = null;
                    }
                } else {
                    if (collapsedTab == null) {
                        collapsedTab = selectedTab;
                    }
                    // When window is super small, we pretty much can only show run tab
                    SelectTab(typeof(HotReloadRunTab));
                }

                if (HotReloadWindowStyles.windowScreenHeight > 250 && HotReloadWindowStyles.windowScreenWidth > 275) {
                    RenderLogo();
                }

                Tabs[selectedTab].OnGUI();
            }
        }

        void RenderBottomBar() {
            SemVersion newVersion;
            var updateAvailable = packageUpdateChecker.TryGetNewVersion(out newVersion);

            if (HotReloadWindowStyles.windowScreenWidth > Constants.RateAppHideWidth
                && HotReloadWindowStyles.windowScreenHeight > Constants.RateAppHideHeight
            ) {
                RenderRateApp();
            }

            if (updateAvailable) {
                RenderUpdateButton(newVersion);
            }
            
            using(new EditorGUILayout.HorizontalScope("ProjectBrowserBottomBarBg", GUILayout.ExpandWidth(true), GUILayout.Height(25f))) {
                RenderBottomBarCore();
            }
        }

        static GUIStyle _renderAppBoxStyle;
        static GUIStyle renderAppBoxStyle => _renderAppBoxStyle ?? (_renderAppBoxStyle = new GUIStyle(GUI.skin.box) {
            padding = new RectOffset(10, 10, 0, 0)
        });
        
        static GUILayoutOption[] _nonExpandable;
        public static GUILayoutOption[] NonExpandableLayout => _nonExpandable ?? (_nonExpandable = new [] {GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true)});
        
        internal static void RenderRateApp() {
            if (!ShouldShowRateApp()) {
                return;
            }
            using (new EditorGUILayout.VerticalScope(renderAppBoxStyle)) {
                using (new EditorGUILayout.HorizontalScope()) {
                    HotReloadGUIHelper.HelpBox("Are you enjoying using Hot Reload?", MessageType.Info, 11);
                    if (GUILayout.Button("Hide", NonExpandableLayout)) {
                        RequestHelper.RequestEditorEventWithRetry(new Stat(StatSource.Client, StatLevel.Debug, StatFeature.RateApp), new EditorExtraData { { "dismissed", true } }).Forget();
                        HotReloadPrefs.RateAppShown = true;
                    }
                }
                using (new EditorGUILayout.HorizontalScope()) {
                    if (GUILayout.Button("Yes")) {
                        var openedUrl = PackageConst.IsAssetStoreBuild && EditorUtility.DisplayDialog("Rate Hot Reload", "Thank you for using Hot Reload!\n\nPlease consider leaving a review on the Asset Store to support us.", "Open in browser", "Cancel");
                        if (openedUrl) {
                            Application.OpenURL(Constants.UnityStoreRateAppURL);
                        }
                        HotReloadPrefs.RateAppShown = true;
                        var data = new EditorExtraData();
                        if (PackageConst.IsAssetStoreBuild) {
                            data.Add("opened_url", openedUrl);
                        }
                        data.Add("enjoy_app", true);
                        RequestHelper.RequestEditorEventWithRetry(new Stat(StatSource.Client, StatLevel.Debug, StatFeature.RateApp), data).Forget();
                    }
                    if (GUILayout.Button("No")) {
                        HotReloadPrefs.RateAppShown = true;
                        var data = new EditorExtraData();
                        data.Add("enjoy_app", false);
                        RequestHelper.RequestEditorEventWithRetry(new Stat(StatSource.Client, StatLevel.Debug, StatFeature.RateApp), data).Forget();
                    }
                }
            }
        }

        internal static bool ShouldShowRateApp() {
            if (HotReloadPrefs.RateAppShown) {
                return false;
            }
            var activeDays = EditorCodePatcher.GetActiveDaysForRateApp();
            if (activeDays.Count < Constants.DaysToRateApp) {
                return false;
            }
            return true;
        }

        void RenderUpdateButton(SemVersion newVersion) {
            if (GUILayout.Button($"Update To v{newVersion}", HotReloadWindowStyles.UpgradeButtonStyle)) {
                packageUpdateChecker.UpdatePackageAsync(newVersion).Forget(CancellationToken.None);
            }
        }
        
        internal static void RenderShowOnStartup() {
            var prevLabelWidth = EditorGUIUtility.labelWidth;
            try {
                EditorGUIUtility.labelWidth = 105f;
                using (new GUILayout.VerticalScope()) {
                    using (new GUILayout.HorizontalScope()) {
                        GUILayout.Label("Show On Startup");
                        Rect buttonRect = GUILayoutUtility.GetLastRect();
                        if (EditorGUILayout.DropdownButton(new GUIContent(Regex.Replace(_showOnStartupOption.ToString(), "([a-z])([A-Z])", "$1 $2")), FocusType.Passive, GUILayout.Width(110f))) {
                            GenericMenu menu = new GenericMenu();
                            foreach (ShowOnStartupEnum option in Enum.GetValues(typeof(ShowOnStartupEnum))) {
                                menu.AddItem(new GUIContent(Regex.Replace(option.ToString(), "([a-z])([A-Z])", "$1 $2")), false, () => {
                                    if (_showOnStartupOption != option) {
                                        _showOnStartupOption = option;
                                        HotReloadPrefs.ShowOnStartup = _showOnStartupOption;
                                    }
                                });
                            }
                            menu.DropDown(new Rect(buttonRect.x, buttonRect.y, 100, 0));
                        }
                    }
                }
            } finally {
                EditorGUIUtility.labelWidth = prevLabelWidth;
            }
        }
        
        internal static readonly OpenURLButton autoRefreshTroubleshootingBtn = new OpenURLButton("Troubleshooting", Constants.TroubleshootingURL);
        void RenderBottomBarCore() {
            bool troubleshootingShown = EditorCodePatcher.Started && HotReloadWindowStyles.windowScreenWidth >= 400;
            bool alertsShown = EditorCodePatcher.Started && HotReloadWindowStyles.windowScreenWidth > Constants.EventFiltersShownHideWidth;
            using (new EditorGUILayout.VerticalScope()) {
                using (new EditorGUILayout.HorizontalScope(HotReloadWindowStyles.FooterStyle)) {
                    if (!troubleshootingShown) {
                        GUILayout.FlexibleSpace();
                        if (alertsShown) {
                            GUILayout.Space(-20);
                        }
                    } else {
                        GUILayout.Space(21);
                    }
                    GUILayout.Space(0);
                    var lastRect = GUILayoutUtility.GetLastRect();
                    // show events button when scrolls are hidden
                    if (!HotReloadRunTab.CanRenderBars(RunTabState) && !RunTabState.starting) {
                        using (new EditorGUILayout.VerticalScope()) {
                            GUILayout.FlexibleSpace();
                            var icon = HotReloadState.ShowingRedDot ? InvertibleIcon.EventsNew : InvertibleIcon.Events;
                            if (GUILayout.Button(new GUIContent("", GUIHelper.GetInvertibleIcon(icon)))) {
                                PopupWindow.Show(new Rect(lastRect.x, lastRect.y, 0, 0), HotReloadEventPopup.I);
                            }
                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.Space(3f);
                    }
                    if (alertsShown) {
                        using (new EditorGUILayout.VerticalScope()) {
                            GUILayout.FlexibleSpace();
                            HotReloadTimelineHelper.RenderAlertFilters();
                            GUILayout.FlexibleSpace();
                        }
                    }

                    GUILayout.FlexibleSpace();
                    if (troubleshootingShown) {
                        using (new EditorGUILayout.VerticalScope()) {
                            GUILayout.FlexibleSpace();
                            autoRefreshTroubleshootingBtn.OnGUI();
                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.Space(21);
                    }
                }
            }
        }
    }
}