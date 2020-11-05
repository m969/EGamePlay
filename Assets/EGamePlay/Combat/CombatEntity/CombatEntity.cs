using EGamePlay.Combat.Skill;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public sealed class CombatEntity : Entity
    {
        public CombatListen CombatListen { get; set; }
        public CombatRun CombatRun { get; set; }
        public HealthPoint HealthPoint { get; private set; } = new HealthPoint();
        public CombatNumericBox NumericBox { get; private set; } = new CombatNumericBox();
        public CombatActionTrigger ActionTrigger { get; set; } = new CombatActionTrigger();
        public CombatSkillComponent SkillComponent { get { return GetComponent<CombatSkillComponent>(); } }


        public void Initialize()
        {
            NumericBox.Initialize();
            ActionTrigger.Initialize();
            HealthPoint.SetMaxValue(99_999);
            HealthPoint.Reset();
        }

        public void AddListener(CombatActionType actionType, Action<CombatOperation> action)
        {
            ActionTrigger.AddListener(actionType, action);
        }

        public void RemoveListener(CombatActionType actionType, Action<CombatOperation> action)
        {
            ActionTrigger.RemoveListener(actionType, action);
        }

        public void TriggerAction(CombatActionType actionType, CombatOperation action)
        {
            ActionTrigger.TriggerAction(actionType, action);
        }

        public void ReceiveDamage(CombatOperation combatOperation)
        {
            var damageOperation = combatOperation as DamageOperation;
            HealthPoint.Minus(damageOperation.DamageValue);
        }

        public void ReceiveCure(CombatOperation combatOperation)
        {
            var cureOperation = combatOperation as CureOperation;
            HealthPoint.Add(cureOperation.CureValue);
        }

        public T CreateSkill<T>() where T : SkillEntity, new()
        {
            var skill = SkillComponent.CreateSkill<T>();
            skill.SpellCaster = this;
            return skill;
        }
    }
}