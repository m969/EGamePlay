using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public class CombatBlackBoard : BlackBoard
    {
        private static CombatBlackBoard Instance;

        public static new void SetValue(string key, object v)
        {
            if (Instance == null) Instance = new CombatBlackBoard();
            (Instance as BlackBoard).SetValue(key, v);
        }

        public static new T GetValue<T>(string key, T defaultValue)
        {
            if (Instance == null) Instance = new CombatBlackBoard();
            return (Instance as BlackBoard).GetValue<T>(key, defaultValue);
        }
    }
}