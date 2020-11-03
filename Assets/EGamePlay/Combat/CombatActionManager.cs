using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战斗行为管理器
    /// </summary>
    public static class CombatActionManager
    {
        private static List<CombatAction> CombatActions = new List<CombatAction>();


        public static DamageAction CreateDamageAction(CombatEntity combatEntity)
        {
            var DamageAction = new DamageAction();
            DamageAction.Creator = combatEntity;
            CombatActions.Add(DamageAction);
            return DamageAction;
        }
    }
}