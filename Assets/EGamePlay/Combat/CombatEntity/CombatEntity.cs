using EGamePlay.Combat.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public sealed class CombatEntity : Entity
    {
        public HealthPoint HealthPoint { get; private set; } = new HealthPoint();
        public CombatNumericBox NumericBox { get; private set; } = new CombatNumericBox();
        public ActionPointEventManager ActionPointEventManager { get; set; } = new ActionPointEventManager();
        public CombatSkillComponent SkillComponent { get { return GetComponent<CombatSkillComponent>(); } }


        public void Initialize()
        {
            NumericBox.Initialize();
            ActionPointEventManager.Initialize();
            HealthPoint.SetMaxValue(99_999);
            HealthPoint.Reset();
        }

        public void AddListener(ActionPointType actionPointType, Action<CombatAction> action)
        {
            ActionPointEventManager.AddListener(actionPointType, action);
        }

        public void RemoveListener(ActionPointType actionPointType, Action<CombatAction> action)
        {
            ActionPointEventManager.RemoveListener(actionPointType, action);
        }

        public void TriggerAction(ActionPointType actionPointType, CombatAction action)
        {
            ActionPointEventManager.TriggerAction(actionPointType, action);
        }

        public void ReceiveDamage(CombatAction combatAction)
        {
            var damageAction = combatAction as DamageAction;
            HealthPoint.Minus(damageAction.DamageValue);
        }

        public void ReceiveCure(CombatAction combatAction)
        {
            var cureAction = combatAction as CureAction;
            HealthPoint.Add(cureAction.CureValue);
        }
    }
}