using UnityEngine;
using System.Reflection;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SingularityGroup.HotReload.Demo")]

namespace SingularityGroup.HotReload.Editor {
    internal class UnitySettingsHelper {
        public static UnitySettingsHelper I = new UnitySettingsHelper();

        private bool initialized;
        private object pref;
        private PropertyInfo prefColorProp;
        private MethodInfo setMethod;
        private Type settingsType;
        private Type prefColorType;
        const string currentPlaymodeTintPrefKey = "Playmode tint";

        internal bool playmodeTintSupported => EditorCodePatcher.config.changePlaymodeTint && EnsureInitialized();

        private UnitySettingsHelper() {
            EnsureInitialized();
        }
        

        private bool EnsureInitialized() {
            if (initialized) {
                return true;
            }
            try {
                // cache members for performance
                settingsType = settingsType ?? (settingsType = typeof(UnityEditor.Editor).Assembly.GetType($"UnityEditor.PrefSettings"));
                prefColorType = prefColorType ?? (prefColorType = typeof(UnityEditor.Editor).Assembly.GetType($"UnityEditor.PrefColor"));
                prefColorProp = prefColorProp ?? (prefColorProp = prefColorType?.GetProperty("Color", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
                pref = pref ?? (pref = GetPref(settingsType: settingsType, prefColorType: prefColorType));
                setMethod = setMethod ?? (setMethod = GetSetMethod(settingsType: settingsType, prefColorType: prefColorType));

                if (prefColorProp == null
                    || pref == null
                    || setMethod == null
                ) {
                    return false;
                }
                
                // clear cache for performance
                settingsType = null;
                prefColorType = null;

                initialized = true;
                return true;
            } catch {
                return false;
            }
        }

        private static MethodInfo GetSetMethod(Type settingsType, Type prefColorType) {
            var setMethodBase = settingsType?.GetMethod("Set", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            return setMethodBase?.MakeGenericMethod(prefColorType);
        }

        private static object GetPref(Type settingsType, Type prefColorType) {
            var prefsMethodBase = settingsType?.GetMethod("Prefs", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            var prefsMethod = prefsMethodBase?.MakeGenericMethod(prefColorType);
            var prefs = (IEnumerable)prefsMethod?.Invoke(null, Array.Empty<object>());
            if (prefs != null) {
                foreach (object kvp in prefs) {
                    var key = kvp.GetType().GetProperty("Key", BindingFlags.Instance | BindingFlags.Public)?.GetMethod.Invoke(kvp, Array.Empty<object>());
                    if (key?.ToString() == currentPlaymodeTintPrefKey) {
                        return kvp.GetType().GetProperty("Value", BindingFlags.Instance | BindingFlags.Public)?.GetMethod.Invoke(kvp, Array.Empty<object>());
                    }

                }
            }
            return null;
        }

        public Color? GetCurrentPlaymodeColor() {
            if (!playmodeTintSupported) {
                return null;
            }
            return (Color)prefColorProp.GetValue(pref);
        }
        
        public void SetPlaymodeTint(Color color) {
            if (!playmodeTintSupported) {
                return;
            }
            prefColorProp.SetValue(pref, color);
            setMethod.Invoke(null, new object[] { currentPlaymodeTintPrefKey, pref });
        }
    }
}

