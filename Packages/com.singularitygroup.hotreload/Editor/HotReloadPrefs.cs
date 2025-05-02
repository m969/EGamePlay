using System;
using System.Globalization;
using System.IO;
using JetBrains.Annotations;
using SingularityGroup.HotReload.Editor.Cli;
using UnityEditor;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    internal static class HotReloadPrefs {
        private const string RemoteServerKey = "HotReloadWindow.RemoteServer";
        private const string RemoteServerHostKey = "HotReloadWindow.RemoteServerHost";
        private const string LicenseEmailKey = "HotReloadWindow.LicenseEmail";
        private const string RenderAuthLoginKey = "HotReloadWindow.RenderAuthLogin";
        private const string FirstLoginCachedKey = "HotReloadWindow.FirstLoginCachedKey";
        [Obsolete]
        private const string ShowOnStartupKey = "HotReloadWindow.ShowOnStartup";
        private const string PasswordCachedKey = "HotReloadWindow.PasswordCached";
        private const string ExposeServerToLocalNetworkKey = "HotReloadWindow.ExposeServerToLocalNetwork";
        private const string ErrorHiddenCachedKey = "HotReloadWindow.ErrorHiddenCachedKey";
        private const string RefreshManuallyTipCachedKey = "HotReloadWindow.RefreshManuallyTipCachedKey";
        private const string ShowLoginCachedKey = "HotReloadWindow.ShowLoginCachedKey";
        private const string ConfigurationKey = "HotReloadWindow.Configuration";
        private const string AvdancedKey = "HotReloadWindow.Avdanced";
        private const string ShowPromoCodesCachedKey = "HotReloadWindow.ShowPromoCodesCached";
        private const string ShowOnDeviceKey = "HotReloadWindow.ShowOnDevice";
        private const string ShowChangelogKey = "HotReloadWindow.ShowChangelog";
        private const string UnsupportedChangesKey = "HotReloadWindow.ShowUnsupportedChanges";
        private const string LoggedBurstHintKey = "HotReloadWindow.LoggedBurstHint";
        private const string ShouldDoAutoRefreshFixupKey = "HotReloadWindow.ShouldDoAutoRefreshFixup";
        private const string ActiveDaysKey = "HotReloadWindow.ActiveDays";
        [Obsolete]
        private const string RateAppShownKey = "HotReloadWindow.RateAppShown";
        private const string PatchesCollapseKey = "HotReloadWindow.PatchesCollapse";
        private const string PatchesGroupAllKey = "HotReloadWindow.PatchesGroupAll";
        private const string LaunchOnEditorStartKey = "HotReloadWindow.LaunchOnEditorStart";
        private const string AutoRecompileUnsupportedChangesKey = "HotReloadWindow.AutoRecompileUnsupportedChanges";
        private const string AutoRecompilePartiallyUnsupportedChangesKey = "HotReloadWindow.AutoRecompilePartiallyUnsupportedChanges";
        private const string DisplayNewMonobehaviourMethodsAsPartiallySupportedKey = "HotReloadWindow.DisplayNewMonobehaviourMethodsAsPartiallySupported";
        private const string ShowNotificationsKey = "HotReloadWindow.ShowNotifications";
        private const string ShowPatchingNotificationsKey = "HotReloadWindow.ShowPatchingNotifications";
        private const string ShowCompilingUnsupportedNotificationsKey = "HotReloadWindow.ShowCompilingUnsupportedNotifications";
        private const string AutoRecompileUnsupportedChangesImmediatelyKey = "HotReloadWindow.AutoRecompileUnsupportedChangesImmediately";
        private const string AutoRecompileUnsupportedChangesOnExitPlayModeKey = "HotReloadWindow.AutoRecompileUnsupportedChangesOnExitPlayMode";
        private const string AutoRecompileUnsupportedChangesInPlayModeKey = "HotReloadWindow.AutoRecompileUnsupportedChangesInPlayMode";
        private const string AllowDisableUnityAutoRefreshKey = "HotReloadWindow.AllowDisableUnityAutoRefresh";
        private const string DefaultAutoRefreshKey = "HotReloadWindow.DefaultAutoRefresh";
        private const string DefaultAutoRefreshModeKey = "HotReloadWindow.DefaultAutoRefreshMode";
        private const string DefaultScriptCompilationKeyKey = "HotReloadWindow.DefaultScriptCompilationKey";
        private const string DefaultEditorTintKey = "HotReloadWindow.DefaultEditorTint";
        private const string AppliedAutoRefreshKey = "HotReloadWindow.AppliedAutoRefresh";
        private const string AppliedScriptCompilationKey = "HotReloadWindow.AppliedScriptCompilation";
        private const string AppliedEditorTintKey = "HotReloadWindow.AppliedEditorTint";
        private const string AllAssetChangesKey = "HotReloadWindow.AllAssetChanges";
        private const string IncludeShaderChangesKey = "HotReloadWindow.IncludeShaderChanges";
        private const string DisableConsoleWindowKey = "HotReloadWindow.DisableConsoleWindow";
        private const string DisableDetailedErrorReportingKey = "HotReloadWindow.DisableDetailedErrorReporting";
        private const string DebuggerCompatibilityEnabledKey = "HotReloadWindow.DebuggerCompatibilityEnabled";
        private const string RedeemLicenseEmailKey = "HotReloadWindow.RedeemLicenseEmail";
        private const string RedeemLicenseInvoiceKey = "HotReloadWindow.RedeemLicenseInvoice";
        private const string RunTabEventsSuggestionsFoldoutKey = "HotReloadWindow.RunTabEventsSuggestionsFoldout";
        private const string RunTabEventsTimelineFoldoutKey = "HotReloadWindow.RunTabEventsTimelineFoldout";
        private const string RunTabUnsupportedChangesFilterKey = "HotReloadWindow.RunTabUnsupportedChangesFilter";
        private const string RunTabCompileErrorFilterKey = "HotReloadWindow.RunTabCompileErrorFilter";
        private const string RunTabPartiallyAppliedPatchesFilterKey = "HotReloadWindow.RunTabPartiallyAppliedPatchesFilter";
        private const string RunTabUndetectedPatchesFilterKey = "HotReloadWindow.RunTabUndetectedPatchesFilter";
        private const string RunTabAppliedPatchesFilterKey = "HotReloadWindow.RunTabAppliedPatchesFilter";
        private const string RecompileDialogueShownKey = "HotReloadWindow.RecompileDialogueShown";
        private const string ApplyFieldInitiailzerEditsToExistingClassInstancesKey = "HotReloadWindow.ApplyFieldInitiailzerEditsToExistingClassInstances";
        private const string LoggedInlinedMethodsDialogueKey = "HotReloadWindow.LoggedInlinedMethodsDialogue";
        private const string OpenedWindowAtLeastOnceKey = "HotReloadWindow.OpenedWindowAtLeastOnce";
        private const string DeactivateHotReloadKey = "HotReloadWindow.DeactivateHotReload";

        public const string DontShowPromptForDownloadKey = "ServerDownloader.DontShowPromptForDownload";

        [Obsolete] public const string AllowHttpSettingCacheKey = "HotReloadWindow.AllowHttpSettingCacheKey";
        [Obsolete] public const string AutoRefreshSettingCacheKey = "HotReloadWindow.AutoRefreshSettingCacheKey";
        [Obsolete] public const string ScriptCompilationSettingCacheKey = "HotReloadWindow.ScriptCompilationSettingCacheKey";
        [Obsolete] public const string ProjectGenerationSettingCacheKey = "HotReloadWindow.ProjectGenerationSettingCacheKey";


        [Obsolete]
        public static bool RemoteServer {
            get { return EditorPrefs.GetBool(RemoteServerKey, false); }
            set { EditorPrefs.SetBool(RemoteServerKey, value); }
        }
        
        public static bool DontShowPromptForDownload {
            get { return EditorPrefs.GetBool(DontShowPromptForDownloadKey, false); }
            set { EditorPrefs.SetBool(DontShowPromptForDownloadKey, value); }
        }

        [Obsolete]
        public static string RemoteServerHost {
            get { return EditorPrefs.GetString(RemoteServerHostKey); }
            set { EditorPrefs.SetString(RemoteServerHostKey, value); }
        }

        public static string LicenseEmail {
            get { return EditorPrefs.GetString(LicenseEmailKey); }
            set { EditorPrefs.SetString(LicenseEmailKey, value); }
        }
        
        public static string LicensePassword {
            get { return EditorPrefs.GetString(PasswordCachedKey); }
            set { EditorPrefs.SetString(PasswordCachedKey, value); }
        }
        
        [Obsolete]
        public static bool RenderAuthLogin { // false = render free trial
            get { return EditorPrefs.GetBool(RenderAuthLoginKey); }
            set { EditorPrefs.SetBool(RenderAuthLoginKey, value); }
        }
        
        [Obsolete]
        public static bool FirstLogin {
            get { return EditorPrefs.GetBool(FirstLoginCachedKey, true); }
            set { EditorPrefs.SetBool(FirstLoginCachedKey, value); }
        }

        [Obsolete]
        public static string ShowOnStartupLegacy { // WindowAutoOpen
            get { return EditorPrefs.GetString(ShowOnStartupKey); }
            set { EditorPrefs.SetString(ShowOnStartupKey, value); }
        }
        
        public static string showOnStartupPath { get; }= Path.Combine(CliUtils.GetAppDataPath(), "showOnStartup.txt");
        static ShowOnStartupEnum? showOnStartup;
        public static ShowOnStartupEnum ShowOnStartup {
            get {
                if (showOnStartup != null) {
                    return showOnStartup.Value;
                }
                if (!File.Exists(showOnStartupPath)) {
                    showOnStartup = ShowOnStartupEnum.Always;
                    return showOnStartup.Value;
                }
                var text = File.ReadAllText(showOnStartupPath);
                ShowOnStartupEnum _showOnStartup;
                if (Enum.TryParse(text, true, out _showOnStartup)) {
                    showOnStartup = _showOnStartup;
                    return showOnStartup.Value;
                }
                showOnStartup = ShowOnStartupEnum.Always;
                return showOnStartup.Value;
            }
            set {
                // ReSharper disable once AssignNullToNotNullAttribute
                Directory.CreateDirectory(Path.GetDirectoryName(showOnStartupPath));
                File.WriteAllText(showOnStartupPath, value.ToString());
                showOnStartup = value;
            }
        }


        public static bool ErrorHidden {
            get { return EditorPrefs.GetBool(ErrorHiddenCachedKey); }
            set { EditorPrefs.SetBool(ErrorHiddenCachedKey, value); }
        }
        
        public static bool ShowLogin {
            get { return EditorPrefs.GetBool(ShowLoginCachedKey, true); }
            set { EditorPrefs.SetBool(ShowLoginCachedKey, value); }
        }

        public static bool ShowConfiguration {
            get { return EditorPrefs.GetBool(ConfigurationKey, true); }
            set { EditorPrefs.SetBool(ConfigurationKey, value); }
        }
        
        public static bool ShowAdvanced {
            get { return EditorPrefs.GetBool(AvdancedKey, false); }
            set { EditorPrefs.SetBool(AvdancedKey, value); }
        }

        public static bool ShowPromoCodes {
            get { return EditorPrefs.GetBool(ShowPromoCodesCachedKey, true); }
            set { EditorPrefs.SetBool(ShowPromoCodesCachedKey, value); }
        }
        
        public static bool ShowOnDevice {
            get { return EditorPrefs.GetBool(ShowOnDeviceKey, true); }
            set { EditorPrefs.SetBool(ShowOnDeviceKey, value); }
        }
        
        public static bool ShowChangeLog {
            get { return EditorPrefs.GetBool(ShowChangelogKey, true); }
            set { EditorPrefs.SetBool(ShowChangelogKey, value); }
        }
        
        public static bool ShowUnsupportedChanges {
            get { return EditorPrefs.GetBool(UnsupportedChangesKey, true); }
            set { EditorPrefs.SetBool(UnsupportedChangesKey, value); }
        }
        
        [Obsolete]
        public static bool RefreshManuallyTip {
            get { return EditorPrefs.GetBool(RefreshManuallyTipCachedKey); }
            set { EditorPrefs.SetBool(RefreshManuallyTipCachedKey, value); }
        }
        
        public static bool LoggedBurstHint {
            get { return EditorPrefs.GetBool(LoggedBurstHintKey); }
            set { EditorPrefs.SetBool(LoggedBurstHintKey, value); }
        }
        
        [Obsolete]
        public static bool ShouldDoAutoRefreshFixup {
            get { return EditorPrefs.GetBool(ShouldDoAutoRefreshFixupKey, true); }
            set { EditorPrefs.SetBool(ShouldDoAutoRefreshFixupKey, value); }
        }
        
        public static string ActiveDays {
            get { return EditorPrefs.GetString(ActiveDaysKey, string.Empty); }
            set { EditorPrefs.SetString(ActiveDaysKey, value); }
        }
        
        [Obsolete]
        public static bool RateAppShownLegacy {
            get { return EditorPrefs.GetBool(RateAppShownKey, false); }
            set { EditorPrefs.SetBool(RateAppShownKey, value); }
        }
        
        static string rateAppPath = Path.Combine(CliUtils.GetAppDataPath(), "ratedApp.txt");
        static bool? rateAppShown;
        public static bool RateAppShown {
            get {
                if (rateAppShown != null) {
                    return rateAppShown.Value;
                }
                rateAppShown = File.Exists(rateAppPath);
                return rateAppShown.Value;
            }
            set {
                // ReSharper disable once AssignNullToNotNullAttribute
                Directory.CreateDirectory(Path.GetDirectoryName(rateAppPath));
                if (value && !File.Exists(rateAppPath)) {
                    using (File.Create(rateAppPath)) { }
                } else if (!value && File.Exists(rateAppPath)) {
                    File.Delete(rateAppPath);
                }
                rateAppShown = value;
            }
        }

        [Obsolete]
        public static bool PatchesGroupAll {
            get { return EditorPrefs.GetBool(PatchesGroupAllKey, false); }
            set { EditorPrefs.SetBool(PatchesGroupAllKey, value); }
        }

        [Obsolete]
        public static bool PatchesCollapse {
            get { return EditorPrefs.GetBool(PatchesCollapseKey, true); }
            set { EditorPrefs.SetBool(PatchesCollapseKey, value); }
        }

        [Obsolete]
        public static ShowOnStartupEnum GetShowOnStartupEnum() {
            ShowOnStartupEnum showOnStartupEnum;
            if (Enum.TryParse(HotReloadPrefs.ShowOnStartupLegacy, true, out showOnStartupEnum)) {
                return showOnStartupEnum;
            }
            return ShowOnStartupEnum.Always;
        }
        
        public static bool ExposeServerToLocalNetwork {
            get { return EditorPrefs.GetBool(ExposeServerToLocalNetworkKey, false); }
            set { EditorPrefs.SetBool(ExposeServerToLocalNetworkKey, value); }
        }
        
        public static bool LaunchOnEditorStart {
            get { return EditorPrefs.GetBool(LaunchOnEditorStartKey, false); }
            set { EditorPrefs.SetBool(LaunchOnEditorStartKey, value); }
        }

        public static bool AutoRecompileUnsupportedChanges {
            get { return EditorPrefs.GetBool(AutoRecompileUnsupportedChangesKey, false); }
            set { EditorPrefs.SetBool(AutoRecompileUnsupportedChangesKey, value); }
        }
        
        public static bool AutoRecompilePartiallyUnsupportedChanges {
            get { return EditorPrefs.GetBool(AutoRecompilePartiallyUnsupportedChangesKey, false); }
            set { EditorPrefs.SetBool(AutoRecompilePartiallyUnsupportedChangesKey, value); }
        }
        
        public static bool DisplayNewMonobehaviourMethodsAsPartiallySupported {
            get { return EditorPrefs.GetBool(DisplayNewMonobehaviourMethodsAsPartiallySupportedKey, false); }
            set { EditorPrefs.SetBool(DisplayNewMonobehaviourMethodsAsPartiallySupportedKey, value); }
        }

        public static bool ShowNotifications {
            get { return EditorPrefs.GetBool(ShowNotificationsKey, true); }
            set { EditorPrefs.SetBool(ShowNotificationsKey, value); }
        }

        public static bool ShowPatchingNotifications {
            get { return EditorPrefs.GetBool(ShowPatchingNotificationsKey, true); }
            set { EditorPrefs.SetBool(ShowPatchingNotificationsKey, value); }
        }

        public static bool ShowCompilingUnsupportedNotifications {
            get { return EditorPrefs.GetBool(ShowCompilingUnsupportedNotificationsKey, true); }
            set { EditorPrefs.SetBool(ShowCompilingUnsupportedNotificationsKey, value); }
        }

        public static bool AutoRecompileUnsupportedChangesImmediately {
            get { return EditorPrefs.GetBool(AutoRecompileUnsupportedChangesImmediatelyKey, false); }
            set { EditorPrefs.SetBool(AutoRecompileUnsupportedChangesImmediatelyKey, value); }
        }
        
        public static bool AutoRecompileUnsupportedChangesOnExitPlayMode {
            get { return EditorPrefs.GetBool(AutoRecompileUnsupportedChangesOnExitPlayModeKey, false); }
            set { EditorPrefs.SetBool(AutoRecompileUnsupportedChangesOnExitPlayModeKey, value); }
        }
        
        public static bool AutoRecompileUnsupportedChangesInPlayMode {
            get { return EditorPrefs.GetBool(AutoRecompileUnsupportedChangesInPlayModeKey, false); }
            set { EditorPrefs.SetBool(AutoRecompileUnsupportedChangesInPlayModeKey, value); }
        }

        public static bool AllowDisableUnityAutoRefresh {
            get { return EditorPrefs.GetBool(AllowDisableUnityAutoRefreshKey, false); }
            set { EditorPrefs.SetBool(AllowDisableUnityAutoRefreshKey, value); }
        }
        
        public static int DefaultAutoRefresh {
            get { return EditorPrefs.GetInt(DefaultAutoRefreshKey, -1); }
            set { EditorPrefs.SetInt(DefaultAutoRefreshKey, value); }
        }
        
        [UsedImplicitly]
        public static int DefaultAutoRefreshMode {
            get { return EditorPrefs.GetInt(DefaultAutoRefreshModeKey, -1); }
            set { EditorPrefs.SetInt(DefaultAutoRefreshModeKey, value); }
        }
        
        public static int DefaultScriptCompilation {
            get { return EditorPrefs.GetInt(DefaultScriptCompilationKeyKey, -1); }
            set { EditorPrefs.SetInt(DefaultScriptCompilationKeyKey, value); }
        }
        
        public static Color? DefaultEditorTint {
            get { return ColorFromString(EditorPrefs.GetString(DefaultEditorTintKey, string.Empty)); }
            set { EditorPrefs.SetString(DefaultEditorTintKey, ColorToString(value)); }
        }
        
        public static bool AppliedAutoRefresh {
            get { return EditorPrefs.GetBool(AppliedAutoRefreshKey); }
            set { EditorPrefs.SetBool(AppliedAutoRefreshKey, value); }
        }
        
        public static bool AppliedScriptCompilation {
            get { return EditorPrefs.GetBool(AppliedScriptCompilationKey); }
            set { EditorPrefs.SetBool(AppliedScriptCompilationKey, value); }
        }
        
        public static Color? AppliedEditorTint {
            get { return ColorFromString(EditorPrefs.GetString(AppliedEditorTintKey, string.Empty)); }
            set { EditorPrefs.SetString(AppliedEditorTintKey, ColorToString(value)); }
        }
        
        public static bool AllAssetChanges {
            get { return EditorPrefs.GetBool(AllAssetChangesKey, false); }
            set { EditorPrefs.SetBool(AllAssetChangesKey, value); }
        }
        
        public static bool IncludeShaderChanges {
            get { return EditorPrefs.GetBool(IncludeShaderChangesKey, false); }
            set { EditorPrefs.SetBool(IncludeShaderChangesKey, value); }
        }
        
        public static bool DisableConsoleWindow {
            get { return EditorPrefs.GetBool(DisableConsoleWindowKey, false); }
            set { EditorPrefs.SetBool(DisableConsoleWindowKey, value); }
        }
        
        public static string RedeemLicenseEmail {
            get { return EditorPrefs.GetString(RedeemLicenseEmailKey); }
            set { EditorPrefs.SetString(RedeemLicenseEmailKey, value); }
        }
        
        public static string RedeemLicenseInvoice {
            get { return EditorPrefs.GetString(RedeemLicenseInvoiceKey); }
            set { EditorPrefs.SetString(RedeemLicenseInvoiceKey, value); }
        }
        
        public static bool RunTabEventsTimelineFoldout {
            get { return EditorPrefs.GetBool(RunTabEventsTimelineFoldoutKey, true); }
            set { EditorPrefs.SetBool(RunTabEventsTimelineFoldoutKey, value); }
        }
        
        public static bool RunTabEventsSuggestionsFoldout {
            get { return EditorPrefs.GetBool(RunTabEventsSuggestionsFoldoutKey, true); }
            set { EditorPrefs.SetBool(RunTabEventsSuggestionsFoldoutKey, value); }
        }
        
        public static bool RunTabUnsupportedChangesFilter {
            get { return EditorPrefs.GetBool(RunTabUnsupportedChangesFilterKey, true); }
            set { EditorPrefs.SetBool(RunTabUnsupportedChangesFilterKey, value); }
        }
        
        public static bool RunTabCompileErrorFilter {
            get { return EditorPrefs.GetBool(RunTabCompileErrorFilterKey, true); }
            set { EditorPrefs.SetBool(RunTabCompileErrorFilterKey, value); }
        }
        
        public static bool RunTabPartiallyAppliedPatchesFilter {
            get { return EditorPrefs.GetBool(RunTabPartiallyAppliedPatchesFilterKey, true); }
            set { EditorPrefs.SetBool(RunTabPartiallyAppliedPatchesFilterKey, value); }
        }
        
        public static bool RunTabUndetectedPatchesFilter {
            get { return EditorPrefs.GetBool(RunTabUndetectedPatchesFilterKey, true); }
            set { EditorPrefs.SetBool(RunTabUndetectedPatchesFilterKey, value); }
        }
        
        public static bool RunTabAppliedPatchesFilter {
            get { return EditorPrefs.GetBool(RunTabAppliedPatchesFilterKey, true); }
            set { EditorPrefs.SetBool(RunTabAppliedPatchesFilterKey, value); }
        }
        
        public static bool RecompileDialogueShown {
            get { return EditorPrefs.GetBool(RecompileDialogueShownKey); }
            set { EditorPrefs.SetBool(RecompileDialogueShownKey, value); }
        }
        
        public static bool OpenedWindowAtLeastOnce {
            get { return EditorPrefs.GetBool(OpenedWindowAtLeastOnceKey); }
            set { EditorPrefs.SetBool(OpenedWindowAtLeastOnceKey, value); }
        }
        
        private const string rgbaDelimiter = ";";
        public static string ColorToString(Color? _color) {
            if (_color == null) {
                return null;
            }
            var color = _color.Value;
            var cultInfo = CultureInfo.InvariantCulture;
            string[] rgbaList = { color.r.ToString(cultInfo), color.g.ToString(cultInfo), color.b.ToString(cultInfo), color.a.ToString(cultInfo)};
            return String.Join(rgbaDelimiter, rgbaList);
        }

        public static Color? ColorFromString(string ser) {
            if (string.IsNullOrEmpty(ser)) {
                return null;
            }
            string[] rgbaParts = ser.Split(rgbaDelimiter.ToCharArray());
            return new Color(float.Parse(rgbaParts[0]), float.Parse(rgbaParts[1]),float.Parse(rgbaParts[2]),float.Parse(rgbaParts[3]));
        }
        
        [Obsolete("was not implemented")]
        public static bool ApplyFieldInitiailzerEditsToExistingClassInstances {
            get { return EditorPrefs.GetBool(ApplyFieldInitiailzerEditsToExistingClassInstancesKey); }
            set { EditorPrefs.SetBool(ApplyFieldInitiailzerEditsToExistingClassInstancesKey, value); }
        }
        
        public static bool LoggedInlinedMethodsDialogue {
            get { return EditorPrefs.GetBool(LoggedInlinedMethodsDialogueKey); }
            set { EditorPrefs.SetBool(LoggedInlinedMethodsDialogueKey, value); }
        }
        
        public static bool DeactivateHotReload {
            get { return EditorPrefs.GetBool(DeactivateHotReloadKey); }
            set { EditorPrefs.SetBool(DeactivateHotReloadKey, value); }
        }
        
        public static bool DisableDetailedErrorReporting {
            get { return EditorPrefs.GetBool(DisableDetailedErrorReportingKey, false); }
            set { EditorPrefs.SetBool(DisableDetailedErrorReportingKey, value); }
        }
        
        public static bool AutoDisableHotReloadWithDebugger {
            get { return EditorPrefs.GetBool(DebuggerCompatibilityEnabledKey, true); }
            set { EditorPrefs.SetBool(DebuggerCompatibilityEnabledKey, value); }
        }
    }
}
