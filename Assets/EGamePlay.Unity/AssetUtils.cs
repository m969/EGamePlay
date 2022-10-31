using UnityEngine;

namespace GameUtils
{
    public static class AssetUtils
    {
        public static T Load<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }
    }
}