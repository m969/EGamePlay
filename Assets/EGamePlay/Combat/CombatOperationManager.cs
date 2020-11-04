using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战斗行动管理器
    /// </summary>
    public static class CombatOperationManager
    {
        private static List<CombatOperation> CombatOperations = new List<CombatOperation>();


        public static DamageOperation CreateDamageOperation(CombatEntity combatEntity)
        {
            var DamageOperation = new DamageOperation();
            DamageOperation.Creator = combatEntity;
            CombatOperations.Add(DamageOperation);
            return DamageOperation;
        }
    }
}