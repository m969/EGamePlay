#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using UnityEngine;

namespace SingularityGroup.HotReload {
    static class UnityHelper {
        static string m_DataPath;
        public static string DataPath { get { Init(); return m_DataPath; } }
        
        static string m_PersistentDataPath;
        public static string PersistentDataPath { get { Init(); return m_PersistentDataPath; } }
        
        static string m_TemporaryCachePath;
        public static string TemporaryCachePath { get { Init(); return m_TemporaryCachePath; } }
        
        static string m_StreamingAssetsPath;
        public static string StreamingAssetsPath { get { Init(); return m_StreamingAssetsPath; } }
        
        static string m_OperatingSystem;
        public static string OperatingSystem { get { Init(); return m_OperatingSystem; } }
        
        static RuntimePlatform m_Platform;
        public static RuntimePlatform Platform { get { Init(); return m_Platform; } }
        
        static bool m_IsEditor;
        public static bool IsEditor { get { Init(); return m_IsEditor; } }
        
        static bool initialized;
        public static void Init() {
            if(initialized) return;
            m_DataPath = Application.dataPath;
            m_PersistentDataPath = Application.persistentDataPath;
            m_StreamingAssetsPath = Application.streamingAssetsPath;
            m_TemporaryCachePath = Application.temporaryCachePath;
            m_OperatingSystem = SystemInfo.operatingSystem;
            m_Platform = Application.platform;
            m_IsEditor = Application.isEditor;
            
            initialized = true;
        }
    }
}
#endif
