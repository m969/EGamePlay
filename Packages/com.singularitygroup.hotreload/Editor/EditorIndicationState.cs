using System;
using System.Collections.Generic;
using System.Linq;
using SingularityGroup.HotReload.DTO;

namespace SingularityGroup.HotReload.Editor {
    internal static class EditorIndicationState {
        internal enum IndicationStatus {  
            Stopped,
            Started,
            Stopping,
            Installing,
            Starting,
            Reloaded,
            PartiallySupported,
            Unsupported,
            Patching,
            Loading,
            Compiling,
            CompileErrors,
            ActivationFailed,
            FinishRegistration,
            Undetected,
        }

        internal static readonly string greyIconPath = "grey";
        internal static readonly string greenIconPath = "green";
        internal static readonly string redIconPath = "red";
        private static readonly Dictionary<IndicationStatus, string> IndicationIcon = new Dictionary<IndicationStatus, string> {
            // grey icon:
            { IndicationStatus.FinishRegistration, greyIconPath },
            { IndicationStatus.Stopped, greyIconPath },
            // green icon:
            { IndicationStatus.Started, greenIconPath },
            // log icons:
            { IndicationStatus.Reloaded, HotReloadTimelineHelper.alertIconString[AlertType.AppliedChange] },
            { IndicationStatus.Unsupported, HotReloadTimelineHelper.alertIconString[AlertType.UnsupportedChange] },
            { IndicationStatus.Undetected, HotReloadTimelineHelper.alertIconString[AlertType.UndetectedChange] },
            { IndicationStatus.PartiallySupported, HotReloadTimelineHelper.alertIconString[AlertType.PartiallySupportedChange] },
            { IndicationStatus.CompileErrors, HotReloadTimelineHelper.alertIconString[AlertType.CompileError] },
            // spinner:
            { IndicationStatus.Stopping, Spinner.SpinnerIconPath },
            { IndicationStatus.Starting, Spinner.SpinnerIconPath },
            { IndicationStatus.Patching, Spinner.SpinnerIconPath },
            { IndicationStatus.Loading, Spinner.SpinnerIconPath },
            { IndicationStatus.Compiling, Spinner.SpinnerIconPath },
            { IndicationStatus.Installing, Spinner.SpinnerIconPath },
            // red icon:
            { IndicationStatus.ActivationFailed, redIconPath },
        };
        
        private static readonly IndicationStatus[] SpinnerIndications = IndicationIcon
            .Where(kvp => kvp.Value == Spinner.SpinnerIconPath)
            .Select(kvp => kvp.Key)
            .ToArray();
        
        // NOTE: if you add longer text, make sure UI is wide enough for it
        public static readonly Dictionary<IndicationStatus, string> IndicationText = new Dictionary<IndicationStatus, string> {
            { IndicationStatus.FinishRegistration, "Finish Registration" },
            { IndicationStatus.Started, "Waiting for code changes" },
            { IndicationStatus.Stopping, "Stopping Hot Reload" },
            { IndicationStatus.Stopped, "Hot Reload inactive" },
            { IndicationStatus.Installing, "Installing" },
            { IndicationStatus.Starting, "Starting Hot Reload" },
            { IndicationStatus.Reloaded, "Reload finished" },
            { IndicationStatus.PartiallySupported, "Changes partially applied" },
            { IndicationStatus.Unsupported, "Finished with warnings" },
            { IndicationStatus.Patching, "Reloading" },
            { IndicationStatus.Compiling, "Compiling" },
            { IndicationStatus.CompileErrors, "Scripts have compile errors" },
            { IndicationStatus.ActivationFailed, "Activation failed" },
            { IndicationStatus.Loading, "Loading" },
            { IndicationStatus.Undetected, "No changes applied"},
        };

        private const int MinSpinnerDuration = 200;
        private static DateTime spinnerStartedAt;
        private static IndicationStatus latestStatus;
        private static bool SpinnerCompletedMinDuration => DateTime.UtcNow - spinnerStartedAt > TimeSpan.FromMilliseconds(MinSpinnerDuration);
        private static IndicationStatus GetIndicationStatus() {
            var status = GetIndicationStatusCore();
            
            // Note: performance sensitive code, don't use Link
            bool newStatusIsSpinner = false;
            for (var i = 0; i < SpinnerIndications.Length; i++) {
                if (SpinnerIndications[i] == status) {
                    newStatusIsSpinner = true;
                }
            }
            bool latestStatusIsSpinner = false;
            for (var i = 0; i < SpinnerIndications.Length; i++) {
                if (SpinnerIndications[i] == latestStatus) {
                    newStatusIsSpinner = true;
                }
            }
            
            if (status == latestStatus) {
                return status;
            } else if (latestStatusIsSpinner) {
                if (newStatusIsSpinner) {
                    return status;
                } else if (SpinnerCompletedMinDuration) {
                    latestStatus = status;
                    return status;
                } else {
                    return latestStatus;
                }
            } else if (newStatusIsSpinner) {
                spinnerStartedAt = DateTime.UtcNow;
                latestStatus = status;
                return status;    
            } else {
                spinnerStartedAt = DateTime.UtcNow;
                latestStatus = IndicationStatus.Loading;
                return status;
            }
        }
        
        private static IndicationStatus GetIndicationStatusCore() {
            if (RedeemLicenseHelper.I.RegistrationRequired)
                return IndicationStatus.FinishRegistration;
            if (EditorCodePatcher.DownloadRequired && EditorCodePatcher.DownloadStarted || EditorCodePatcher.RequestingDownloadAndRun && !EditorCodePatcher.Starting && !EditorCodePatcher.Stopping)
                return IndicationStatus.Installing;
            if (EditorCodePatcher.Stopping)
                return IndicationStatus.Stopping;
            if (EditorCodePatcher.Compiling && !EditorCodePatcher.Stopping && !EditorCodePatcher.Starting && EditorCodePatcher.Running)
                return IndicationStatus.Compiling;
            if (EditorCodePatcher.Starting && !EditorCodePatcher.Stopping)
                return IndicationStatus.Starting;
            if (!EditorCodePatcher.Running)
                return IndicationStatus.Stopped;
            if (EditorCodePatcher.Status?.isLicensed != true && EditorCodePatcher.Status?.isFree != true && EditorCodePatcher.Status?.freeSessionFinished == true)
                return IndicationStatus.ActivationFailed;
            if (EditorCodePatcher.compileError)
                return IndicationStatus.CompileErrors;

            // fallback on patch status
            if (!EditorCodePatcher.Started && !EditorCodePatcher.Running) {
                return IndicationStatus.Stopped;
            }
            switch (EditorCodePatcher.patchStatus) {
                case PatchStatus.Idle:
                    if (!EditorCodePatcher.Compiling && !EditorCodePatcher.firstPatchAttempted && !EditorCodePatcher.compileError) {
                        return IndicationStatus.Started;
                    }
                    if (EditorCodePatcher._applyingFailed) {
                        return IndicationStatus.Unsupported;
                    }
                    if (EditorCodePatcher._appliedPartially) {
                        return IndicationStatus.PartiallySupported;
                    }
                    if (EditorCodePatcher._appliedUndetected) {
                        return IndicationStatus.Undetected;
                    }
                    return IndicationStatus.Reloaded;
                case PatchStatus.Patching:     return IndicationStatus.Patching;
                case PatchStatus.Unsupported:  return IndicationStatus.Unsupported;
                case PatchStatus.Compiling:    return IndicationStatus.Compiling;
                case PatchStatus.CompileError: return IndicationStatus.CompileErrors;
                case PatchStatus.None:
                default:                       return IndicationStatus.Reloaded;
            }
        }

        internal static IndicationStatus CurrentIndicationStatus => GetIndicationStatus();
        internal static bool SpinnerActive => SpinnerIndications.Contains(CurrentIndicationStatus);
        internal static string IndicationIconPath => IndicationIcon[CurrentIndicationStatus];
        internal static string IndicationStatusText {
            get {
                var indicationStatus = CurrentIndicationStatus;
                string txt;
                if (indicationStatus == IndicationStatus.Starting && EditorCodePatcher.StartupProgress != null) {
                    txt = EditorCodePatcher.StartupProgress.Item2;
                } else if (!IndicationText.TryGetValue(indicationStatus, out txt)) {
                    Log.Warning($"Indication text not found for status {indicationStatus}");
                } else {
                    txt = IndicationText[indicationStatus];
                }
                return txt;
            }
        }
    }
}
