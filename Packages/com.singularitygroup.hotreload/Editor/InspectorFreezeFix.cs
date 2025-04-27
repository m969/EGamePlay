using System.Reflection;
using SingularityGroup.HotReload.Editor;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class InspectorFreezeFix
{
    // Inspector window getting stuck is fixed by calling UnityEditor.InspectorWindow.RefreshInspectors()
    // Below code subscribes to selection changed callback and calls the method if the inspector is actually stuck

    static InspectorFreezeFix()
    {
        Selection.selectionChanged += OnSelectionChanged;
    }
    
    private static int _lastInitialEditorId;

    private static void OnSelectionChanged() {
        if (!EditorCodePatcher.config.enableInspectorFreezeFix) {
            return;
        }
        try {
            // Most of stuff is internal so we use reflection here
            var inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");

            foreach (var inspector in Resources.FindObjectsOfTypeAll(inspectorType)) {
                
                object isLockedValue = inspectorType.GetProperty("isLocked")?.GetValue(inspector);
                if (isLockedValue == null) {
                    continue;
                }
                
                // If inspector window is locked deliberately by user (via the lock icon on top-right), we don't need to refresh
                var isLocked = (bool)isLockedValue;
                if (isLocked) {
                    continue;
                }
                
                // Inspector getting stuck is due to ActiveEditorTracker of that window getting stuck internally.
                // The tracker starts returning same values from m_Tracker.activeEditors property.
                // (Root of cause of this is out of my reach as the tracker code is mainly native code)

                // We detect that by checking first element of activeEditors array
                // We do the check because we don't want to RefreshInspectors every selection change, RefreshInspectors is expensive
                var tracker = inspectorType.GetField("m_Tracker", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(inspector);
                if (tracker == null) {
                    continue;
                }
                var activeEditors = tracker.GetType().GetProperty("activeEditors");
                if (activeEditors == null) {
                    continue;
                }
                var editors = (Editor[])activeEditors.GetValue(tracker);
                if (editors.Length == 0) {
                    continue;
                }
                
                var first = editors[0].GetInstanceID();
                if (_lastInitialEditorId == first) {
                    // This forces the tracker to be rebuilt
                    var m = inspectorType.GetMethod("RefreshInspectors", BindingFlags.Static | BindingFlags.NonPublic);
                    if (m == null) {
                        // support for older versions
                        RefreshInspectors(inspectorType);
                    } else {
                        m.Invoke(null, null);
                    }
                }
                _lastInitialEditorId = first;
                // Calling RefreshInspectors once refreshes all the editors
                break;
            }
        } catch {
            // ignore, we don't want to make user experience worse by displaying a warning in this case
        }
    }

    static void RefreshInspectors(System.Type inspectorType) {
        var allInspectorsField = inspectorType.GetField("m_AllInspectors", BindingFlags.NonPublic | BindingFlags.Static);
        
        if (allInspectorsField == null) {
            return;
        }
        var allInspectors = allInspectorsField.GetValue(null) as System.Collections.IEnumerable;
        if (allInspectors == null) {
            return;
        }
        
        foreach (var inspector in allInspectors) {
            var trackerField = FindFieldInHierarchy(inspector.GetType(), "tracker");

            if (trackerField == null) {
                continue;
            }
            var tracker = trackerField.GetValue(inspector);
            var forceRebuildMethod = tracker.GetType().GetMethod("ForceRebuild", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (forceRebuildMethod == null) {
                
                continue;
            }
            forceRebuildMethod.Invoke(tracker, null);
        }
    }

    static PropertyInfo FindFieldInHierarchy(System.Type type, string fieldName) {
        PropertyInfo field = null;

        while (type != null && field == null) {
            field = type.GetProperty(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            type = type.BaseType;
        }

        return field;
    }
}

