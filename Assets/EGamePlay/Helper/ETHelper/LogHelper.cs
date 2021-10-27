using System;

namespace ET
{
#if !SERVER
    public static class Log
    {
        public static void Debug(string log)
        {
            UnityEngine.Debug.Log(log);
        }

        public static void Info(string log)
        {
            UnityEngine.Debug.Log(log);
        }

        public static void Error(string log)
        {
            UnityEngine.Debug.LogError(log);
        }

        public static void Error(Exception exception)
        {
            UnityEngine.Debug.LogError(exception.ToString());
        }
    }
#endif
}