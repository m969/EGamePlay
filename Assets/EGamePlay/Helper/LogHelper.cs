using System;

namespace EGamePlay
{
#if !NOT_UNITY
    public static class Log
    {
        public static void Debug(string log)
        {
            UnityEngine.Debug.Log(log);
        }

        public static void Error(string log)
        {
            UnityEngine.Debug.LogError(log);
        }

        public static void Error(Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }
    }
#else
    public static class Log
    {
        public static void Debug(string log)
        {
            ET.Log.Debug(log);
        }

        public static void Error(string log)
        {
            ET.Log.Error(log);
        }

        public static void Error(Exception e)
        {
            ET.Log.Error(e);
        }
    }
#endif
}
