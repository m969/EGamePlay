using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay.Combat.Status;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战斗状态管理器
    /// </summary>
    public static class CombatStatusManager
    {
        private static List<StatusAbilityEntity> CombatStatuses = new List<StatusAbilityEntity>();


        public static T CreateStatus<T>(CombatEntity combatEntity) where T: StatusAbilityEntity, new()
        {
            var status = EntityFactory.CreateWithParent<T>(combatEntity);
            CombatStatuses.Add(status);
            return status;
        }
    }
}