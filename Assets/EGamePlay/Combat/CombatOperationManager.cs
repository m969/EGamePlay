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


        //public static DamageOperation CreateDamageOperation(CombatEntity combatEntity)
        //{
        //    var operation = new DamageOperation();
        //    operation.Creator = combatEntity;
        //    CombatOperations.Add(operation);
        //    return operation;
        //}

        //public static SpellSkillOperation CreateSpellSkillOperation(CombatEntity combatEntity)
        //{
        //    var operation = new SpellSkillOperation();
        //    operation.Creator = combatEntity;
        //    CombatOperations.Add(operation);
        //    return operation;
        //}

        public static T CreateOperation<T>(CombatEntity combatEntity) where T:CombatOperation, new()
        {
            var operation = new T();
            operation.Creator = combatEntity;
            CombatOperations.Add(operation);
            return operation;
        }
    }
}