using System;

namespace EGamePlay
{
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
    }
}