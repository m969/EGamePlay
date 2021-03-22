using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战斗行动管理组件
    /// </summary>
    public class CombatActionManageComponent : Component
    {
        private List<CombatAction> CombatActions = new List<CombatAction>();


        public T CreateAction<T>(CombatEntity combatEntity) where T : CombatAction
        {
            var action = Entity.CreateWithParent<T>(Entity);
            action.Creator = combatEntity;
            //CombatActions.Add(action);
            return action;
        }

        public void ClearAllActions()
        {
            foreach (var item in CombatActions)
            {
                Entity.Destroy(item);
            }
            CombatActions.Clear();
        }
    }
}