using UnityEditor;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    internal class OpenURLButton : IGUIComponent {
        public readonly string text;
        public readonly string url;
        public OpenURLButton(string text, string url) {
            this.text = text;
            this.url = url;
        }

        public void OnGUI() {
            Render(text, url);
        }

        public static void Render(string text, string url) {
            if (GUILayout.Button(new GUIContent(text.StartsWith(" ") ? text : " " + text))) {
                Application.OpenURL(url);
            }
        }
        
        public static void RenderRaw(Rect rect, string text, string url, GUIStyle style = null) {
            if (GUI.Button(rect, new GUIContent(text.StartsWith(" ") ? text : " " + text), style ?? GUI.skin.button)) {
                Application.OpenURL(url);
            }
        }
    }
}
