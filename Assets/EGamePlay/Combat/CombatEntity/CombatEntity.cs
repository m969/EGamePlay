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
        public Dictionary<string, AbilityEntity> NameAbilitys { get; set; } = new Dictionary<string, AbilityEntity>();
        public Dictionary<KeyCode, AbilityEntity> InputAbilitys { get; set; } = new Dictionary<KeyCode, AbilityEntity>();
        public Vector3 Position { get; set; }


        public void Initialize()
        {
            NumericBox.Initialize();
            ActionPointManager.Initialize();
            AddComponent<ConditionEventManagerComponent>();
            CurrentHealth.SetMaxValue(NumericBox.HealthPoint_I.Value);
            CurrentHealth.Reset();
        }

        #region 行动点事件
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
        #endregion

        #region 条件事件
        public void AddListener(ConditionType conditionType, Action action, object paramObj = null)
        {
            GetComponent<ConditionEventManagerComponent>().AddListener(conditionType, action, paramObj);
        }
        #endregion

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

        public void GrantAbility(AbilityEntity abilityEntity)
        {
            NameAbilitys.Add(abilityEntity.SkillConfigObject.Name, abilityEntity);
            abilityEntity.OnSetParent(this);
        }

        public void BindAbilityInput(AbilityEntity abilityEntity, KeyCode keyCode)
        {
            InputAbilitys.Add(keyCode, abilityEntity);
            abilityEntity.TryActivateAbility();
        }
    }
}