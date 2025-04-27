using System;
using System.Globalization;
using SingularityGroup.HotReload.DTO;
using UnityEditor;
using UnityEditor.VSAttribution.HotReload;
using UnityEngine;
using UnityEngine.Analytics;

namespace SingularityGroup.HotReload.Editor {
    internal static class Attribution {
         internal const string LastLoginKey = "HotReload.Attribution.LastAttributionEventAt";
         
         //Resend attribution event every 12 hours to be safe
         static readonly TimeSpan resendPeriod = TimeSpan.FromHours(12);
         
         //The last time the attribution event was sent.
         //Returns unix epoch in case it has never been sent before.
         static DateTime LastAttributionEventAt {
             get {
                 if(EditorPrefs.HasKey(LastLoginKey)) {
                     return DateTime.ParseExact(EditorPrefs.GetString(LastLoginKey), "o", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                 }
                 return DateTimeOffset.FromUnixTimeSeconds(0).UtcDateTime;
             }
             set {
                 EditorPrefs.SetString(LastLoginKey, value.ToUniversalTime().ToString("o"));
             }
         }
         
         
         const string actionName = "Login";
         const string partnerName = "The Naughty Cult Ltd.";
         
         public static void RegisterLogin(LoginStatusResponse response) {
             //Licensing might not be initialized yet.
             //The hwId should be set eventually.
             if(response?.hardwareId == null) {
                 return;
             }
             //Only forward attribution if this is an asset store build.
             //We will still distribute this package outside of the asset store (i.e via our website).
             if (!PackageConst.IsAssetStoreBuild) {
                 return;
             }
             
             var now = DateTime.UtcNow;
             //If we sent an attribution event in the last 12 hours we should already be good.
             if (now - LastAttributionEventAt < resendPeriod) {
                 return;
             }
             
             var result = VSAttribution.SendAttributionEvent(actionName, partnerName, response.hardwareId);
             
             //Retry on transient errors
             if (result == AnalyticsResult.NotInitialized) {
                 return;
             }
             LastAttributionEventAt = now;
         }
    }
}