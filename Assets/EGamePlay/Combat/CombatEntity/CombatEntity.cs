using EGamePlay.Combat.Ability;
using EGamePlay.Combat.Status;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战斗实体
    /// </summary>
    public sealed class CombatEntity : Entity
    {
        public HealthPoint CurrentHealth { get; private set; } = new HealthPoint();
        public CombatNumericBox NumericBox { get; private set; } = new CombatNumericBox();
        public ActionPointManager ActionPointManager { get; set; } = new ActionPointManager();
        public Dictionary<int, AbilityEntity> IndexAbilitys { get; set; } = new Dictionary<int, AbilityEntity>();
        public Vector3 Position { get; set; }


        public void Initialize()
        {
            NumericBox.Initialize();
            ActionPointManager.Initialize();
            CurrentHealth.SetMaxValue(NumericBox.HealthPoint_I.Value);
            CurrentHealth.Reset();
        }

        public void AddListener(ActionPointType actionPointType, Action<CombatAction> action)
        {
            ActionPointManager.AddListener(actionPointType, action);
        }

        public void RemoveListener(ActionPointType actionPointType, Action<CombatAction> action)
        {
            ActionPointManager.RemoveListener(actionPointType, action);
        }

        public void TriggerActionPoint(ActionPointType actionPointType, CombatAction action)
        {
            ActionPointManager.TriggerActionPoint(actionPointType, action);
        }

        public void ReceiveDamage(CombatAction combatAction)
        {
            var damageAction = combatAction as DamageAction;
            CurrentHealth.Minus(damageAction.DamageValue);
        }

        public void ReceiveCure(CombatAction combatAction)
        {
            var cureAction = combatAction as CureAction;
            CurrentHealth.Add(cureAction.CureValue);
        }
    }
}