using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay.Combat.Status;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战斗技能管理器
    /// </summary>
    public static class CombatStatusManager
    {
        private static List<StatusEntity> CombatStatuses = new List<StatusEntity>();


        public static T CreateStatus<T>() where T: StatusEntity, new()
        {
            var status = Entity.Create<T>();
            CombatStatuses.Add(status);
            return status;
        }
    }
}