#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using UnityEngine;

namespace SingularityGroup.HotReload.Demo {
    public interface IDemo {
        bool IsServerRunning();
        void OpenHotReloadWindow();
        void OpenScriptFile(TextAsset textAsset, int line, int column);
    }
    
    public static class Demo {
        public static IDemo I = new PlayerDemo();
    }
    
    public class PlayerDemo : IDemo {
        public bool IsServerRunning() {
            return ServerHealthCheck.I.IsServerHealthy;
        }

        public void OpenHotReloadWindow() {
            //no-op
        }

        public void OpenScriptFile(TextAsset textAsset, int line, int column) {
            //no-op
        }
    }
}
#endif
