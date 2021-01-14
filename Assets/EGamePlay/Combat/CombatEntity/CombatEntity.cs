using EGamePlay.Combat.Ability;
using EGamePlay.Combat.Status;
using EGamePlay.Combat.Skill;
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
        public Dictionary<string, AbilityEntity> NameAbilitys { get; set; } = new Dictionary<string, AbilityEntity>();
        public Dictionary<KeyCode, AbilityEntity> InputAbilitys { get; set; } = new Dictionary<KeyCode, AbilityEntity>();
        public Vector3 Position { get; set; }
        public float Direction { get; set; }
        public CombatContext CombatContext { get; set; }
        public bool CanMove { get; set; }


        public override void Awake()
        {
            AddComponent<AttributeComponent>();
            AddComponent<ActionPointManageComponent>();
            AddComponent<ConditionManageComponent>();
            //AddComponent<MotionComponent>();
            CurrentHealth.SetMaxValue((int)GetComponent<AttributeComponent>().HealthPoint.Value);
            CurrentHealth.Reset();
            CombatContext = (CombatContext)Master.GetTypeChildren<CombatContext>()[0];
        }

        public T CreateCombatAction<T>() where T : CombatAction, new()
        {
            var action = CombatContext.GetComponent<CombatActionManageComponent>().CreateAction<T>(this);
            return action;
        }

        #region 行动点事件
        public void ListenActionPoint(ActionPointType actionPointType, Action<CombatAction> action)
        {
            GetComponent<ActionPointManageComponent>().AddListener(actionPointType, action);
        }

        public void UnListenActionPoint(ActionPointType actionPointType, Action<CombatAction> action)
        {
            GetComponent<ActionPointManageComponent>().RemoveListener(actionPointType, action);
        }

        public void TriggerActionPoint(ActionPointType actionPointType, CombatAction action)
        {
            GetComponent<ActionPointManageComponent>().TriggerActionPoint(actionPointType, action);
        }
        #endregion

        #region 条件事件
        public void ListenerCondition(ConditionType conditionType, Action action, object paramObj = null)
        {
            GetComponent<ConditionManageComponent>().AddListener(conditionType, action, paramObj);
        }

        public void UnListenCondition(ConditionType conditionType, Action action)
        {
            GetComponent<ConditionManageComponent>().RemoveListener(conditionType, action);
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

        /// <summary>
        /// 挂载能力，技能、被动、buff都通过这个接口挂载
        /// </summary>
        /// <param name="configObject"></param>
        public T AttachAbility<T>(object configObject) where T : AbilityEntity, new()
        {
            var ability = EntityFactory.CreateWithParent<T>(this, configObject);
            ability.OnSetParent(this);
            return ability;
        }

        public T AttachSkill<T>(object configObject) where T : SkillAbility, new()
        {
            var skill = AttachAbility<T>(configObject);
            NameAbilitys.Add(skill.SkillConfigObject.Name, skill);
            return skill;
        }

        public T AttachStatus<T>(object configObject) where T : StatusAbility, new()
        {
            var status = AttachAbility<T>(configObject);
            return status;
        }

        public void OnStatusRemove(StatusAbility statusAbilityEntity)
        {
            this.Publish(new StatusRemoveEvent() { CombatEntity = this, Status = statusAbilityEntity, StatusId = statusAbilityEntity.Id });
        }

        public void BindAbilityInput(AbilityEntity abilityEntity, KeyCode keyCode)
        {
            InputAbilitys.Add(keyCode, abilityEntity);
            abilityEntity.TryActivateAbility();
        }
    }

    public class StatusRemoveEvent
    {
        public CombatEntity CombatEntity { get; set; }
        public StatusAbility Status { get; set; }
        public long StatusId { get; set; }
    }
}