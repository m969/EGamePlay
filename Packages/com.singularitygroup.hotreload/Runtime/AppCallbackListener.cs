#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System;
using System.Collections;
using UnityEngine;

namespace SingularityGroup.HotReload {
    class AppCallbackListener : MonoBehaviour {
        /// <summary>
        /// Reliable on Android and in the editor.
        /// </summary>
        /// <remarks>
        /// On iOS, OnApplicationPause is not called at expected moments
        /// if the app has some background modes enabled in PlayerSettings -Troy.
        /// </remarks>
        public static event Action<bool> onApplicationPause;
        
        /// <summary>
        /// Reliable on Android, iOS and in the editor.
        /// </summary>
        public static event Action<bool> onApplicationFocus;
        
        static AppCallbackListener instance;
        public static AppCallbackListener I => instance;
        
        // Must be called early from Unity main thread (before any usages of the singleton I).
        public static AppCallbackListener Init() {
             if(instance) return instance;
             var go = new GameObject("AppCallbackListener");
             go.hideFlags |= HideFlags.HideInHierarchy;
             DontDestroyOnLoad(go);
             return instance = go.AddComponent<AppCallbackListener>();
        }
        
        public bool Paused { get; private set; } = false;

        public void DelayedQuit(float seconds) {
            StartCoroutine(delayedQuitRoutine(seconds));
        }
        
        IEnumerator delayedQuitRoutine(float seconds) {
            yield return new WaitForSeconds(seconds);
            Application.Quit();
        }
        
        void OnApplicationPause(bool paused) {
            Paused = paused;
            onApplicationPause?.Invoke(paused);
        }
        
        void OnApplicationFocus(bool playing) {
            onApplicationFocus?.Invoke(playing);
        }
    }
}
#endif
