
using UnityEditor;
using UnityEngine;

namespace SingularityGroup.HotReload.Editor {
    internal abstract class HotReloadTabBase : IGUIComponent {
        protected readonly HotReloadWindow _window;

        public string Title { get; }
        public Texture Icon { get; }
        public string Tooltip { get; }

        public HotReloadTabBase(HotReloadWindow window, string title, Texture iconImage, string tooltip) {
            _window = window;

            Title = title;
            Icon = iconImage;
            Tooltip = tooltip;
        }

        public HotReloadTabBase(HotReloadWindow window, string title, string iconName, string tooltip) :
            this(window, title, EditorGUIUtility.IconContent(iconName).image, tooltip) {
        }

        protected void Repaint() {
            _window.Repaint();
        }

        public virtual void Update() { }

        public abstract void OnGUI();
    }
}
