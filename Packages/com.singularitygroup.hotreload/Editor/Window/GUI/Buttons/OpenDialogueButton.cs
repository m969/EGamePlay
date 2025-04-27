using UnityEditor;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    internal class OpenDialogueButton : IGUIComponent {
        public readonly string text;
        public readonly string url;
        public readonly string title;
        public readonly string message;
        public readonly string ok;
        public readonly string cancel;
        
        public OpenDialogueButton(string text, string url, string title, string message, string ok, string cancel) {
            this.text = text;
            this.url = url;
            this.title = title;
            this.message = message;
            this.ok = ok;
            this.cancel = cancel;
        }

        public void OnGUI() {
             Render(text, url, title, message, ok, cancel);
        }

        public static void Render(string text, string url, string title, string message, string ok, string cancel) {
            if (GUILayout.Button(new GUIContent(text.StartsWith(" ") ? text : " " + text))) {
                if (EditorUtility.DisplayDialog(title, message, ok, cancel)) {
                    Application.OpenURL(url);
                }
            }
        }
        
        public static void RenderRaw(Rect rect, string text, string url, string title, string message, string ok, string cancel, GUIStyle style = null) {
            if (GUI.Button(rect, new GUIContent(text.StartsWith(" ") ? text : " " + text), style ?? GUI.skin.button)) {
                if (EditorUtility.DisplayDialog(title, message, ok, cancel)) {
                    Application.OpenURL(url);
                }
            }
        }
    }
}
