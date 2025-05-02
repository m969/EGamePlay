using EGamePlay.Combat;
using ET;
using System.IO;

namespace GameUtils
{
    public static class AssetUtils
    {
#if UNITY
        public static T LoadObject<T>(string path) where T : UnityEngine.Object
        {
            return UnityEngine.Resources.Load<T>(path);
        }
#else
        public static T LoadObject<T>(string path)
        {
            var text = File.ReadAllText($"../../{path}.json");
            var obj = JsonHelper.FromJson<T>(text);
            return obj;
        }
#endif
    }
}