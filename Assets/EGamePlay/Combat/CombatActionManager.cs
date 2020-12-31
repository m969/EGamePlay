using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战斗行动管理器
    /// </summary>
    public static class CombatActionManager
    {
        private static List<CombatAction> CombatActions = new List<CombatAction>();


        public static T CreateAction<T>(CombatEntity combatEntity) where T : CombatAction, new()
        {
            var action = new T();
            action.Creator = combatEntity;
            CombatActions.Add(action);
            return action;
        }
    }
}