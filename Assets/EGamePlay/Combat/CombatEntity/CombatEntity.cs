using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public class CombatEntity : Entity
    {
        public CombatListen CombatListen { get; set; }
        public CombatRun CombatRun { get; set; }
        public HealthPoint HealthPoint { get; private set; } = new HealthPoint();
        public CombatNumericBox NumericBox { get; private set; } = new CombatNumericBox();
        public CombatActionTrigger ActionTrigger { get; set; } = new CombatActionTrigger();


        public void Initialize()
        {
            NumericBox.Initialize();
            ActionTrigger.Initialize();
            HealthPoint.SetMaxValue(99_999);
            HealthPoint.Reset();
        }

        public void AddListener(CombatActionType actionType, Action<CombatAction> action)
        {
            ActionTrigger.AddListener(actionType, action);
        }

        public void RemoveListener(CombatActionType actionType, Action<CombatAction> action)
        {
            ActionTrigger.RemoveListener(actionType, action);
        }

        public void CallAction(CombatActionType actionType, CombatAction action)
        {
            ActionTrigger.CallAction(actionType, action);
        }

        public void ReceiveDamage(CombatAction combatAction)
        {
            var damageAction = combatAction as DamageAction;
            HealthPoint.Minus(damageAction.DamageValue);
            ActionTrigger.CallAction(CombatActionType.CauseDamage, combatAction);
        }

        public void ReceiveCure(int cure)
        {
            HealthPoint.Add(cure);
        }
    }
}