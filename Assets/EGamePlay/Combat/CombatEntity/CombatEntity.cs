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
        public GameObject ModelObject { get; set; }
        public HealthPoint CurrentHealth { get; private set; } = new HealthPoint();

        public Dictionary<Type, ActionAbility> TypeActions { get; set; } = new Dictionary<Type, ActionAbility>();
        public SpellActionAbility SpellActionAbility { get; private set; }
        public MotionActionAbility MotionActionAbility { get; private set; }
        public DamageActionAbility DamageActionAbility { get; private set; }
        public CureActionAbility CureActionAbility { get; private set; }
        public AttackActionAbility AttackActionAbility { get; private set; }
        public AssignEffectActionAbility AssignEffectActionAbility { get; private set; }
        public TurnActionAbility TurnActionAbility { get; private set; }
        public JumpToActionAbility JumpToActionAbility { get; private set; }

        public AttackAbility AttackAbility { get; set; }
        public SkillExecution CurrentSkillExecution { get; set; }
        public Dictionary<string, SkillAbility> NameSkills { get; set; } = new Dictionary<string, SkillAbility>();
        public Dictionary<KeyCode, SkillAbility> InputSkills { get; set; } = new Dictionary<KeyCode, SkillAbility>();
        public Dictionary<string, List<StatusAbility>> TypeIdStatuses { get; set; } = new Dictionary<string, List<StatusAbility>>();
        public Dictionary<Type, List<StatusAbility>> TypeStatuses { get; set; } = new Dictionary<Type, List<StatusAbility>>();
        public Vector3 Position { get; set; }
        public float Direction { get; set; }
        public ActionControlType ActionControlType { get; set; }


        public override void Awake()
        {
            AddComponent<AttributeComponent>();
            AddComponent<ActionPointManageComponent>();
            AddComponent<ConditionManageComponent>();
            //AddComponent<MotionComponent>();
            CurrentHealth.SetMaxValue((int)GetComponent<AttributeComponent>().HealthPoint.Value);
            CurrentHealth.Reset();
            SpellActionAbility = AttachActionAbility<SpellActionAbility>();
            MotionActionAbility = AttachActionAbility<MotionActionAbility>();
            DamageActionAbility = AttachActionAbility<DamageActionAbility>();
            CureActionAbility = AttachActionAbility<CureActionAbility>();
            AttackActionAbility = AttachActionAbility<AttackActionAbility>();
            AssignEffectActionAbility = AttachActionAbility<AssignEffectActionAbility>();
            TurnActionAbility = AttachActionAbility<TurnActionAbility>();
            JumpToActionAbility = AttachActionAbility<JumpToActionAbility>();
            AttackAbility = CreateChild<AttackAbility>();
        }

        /// <summary>
        /// 创建行动
        /// </summary>
        public T CreateAction<T>() where T : ActionExecution
        {
            var action = Parent.GetComponent<CombatActionManageComponent>().CreateAction<T>(this);
            return action;
        }

        #region 行动点事件
        public void ListenActionPoint(ActionPointType actionPointType, Action<ActionExecution> action)
        {
            GetComponent<ActionPointManageComponent>().AddListener(actionPointType, action);
        }

        public void UnListenActionPoint(ActionPointType actionPointType, Action<ActionExecution> action)
        {
            GetComponent<ActionPointManageComponent>().RemoveListener(actionPointType, action);
        }

        public void TriggerActionPoint(ActionPointType actionPointType, ActionExecution action)
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

        public void ReceiveDamage(ActionExecution combatAction)
        {
            var damageAction = combatAction as DamageAction;
            CurrentHealth.Minus(damageAction.DamageValue);
        }

        public void ReceiveCure(ActionExecution combatAction)
        {
            var cureAction = combatAction as CureAction;
            CurrentHealth.Add(cureAction.CureValue);
        }

        public bool CheckDead()
        {
            return CurrentHealth.Value <= 0;
        }

        /// <summary>
        /// 挂载能力，技能、被动、buff都通过这个接口挂载
        /// </summary>
        /// <param name="configObject"></param>
        private T AttachAbility<T>(object configObject) where T : AbilityEntity
        {
            var ability = Entity.CreateWithParent<T>(this, configObject);
            return ability;
        }

        public T AttachActionAbility<T>() where T : ActionAbility
        {
            var action = AttachAbility<T>(null);
            TypeActions.Add(typeof(T), action);
            return action;
        }

        public T AttachSkill<T>(object configObject) where T : SkillAbility
        {
            var skill = AttachAbility<T>(configObject);
            NameSkills.Add(skill.SkillConfig.Name, skill);
            return skill;
        }

        public T AttachStatus<T>(object configObject) where T : StatusAbility
        {
            var status = AttachAbility<T>(configObject);
            if (!TypeIdStatuses.ContainsKey(status.StatusConfigObject.ID))
            {
                TypeIdStatuses.Add(status.StatusConfigObject.ID, new List<StatusAbility>());
            }
            TypeIdStatuses[status.StatusConfigObject.ID].Add(status);
            return status;
        }

        public void OnStatusRemove(StatusAbility statusAbility)
        {
            TypeIdStatuses[statusAbility.StatusConfigObject.ID].Remove(statusAbility);
            if (TypeIdStatuses[statusAbility.StatusConfigObject.ID].Count == 0)
            {
                TypeIdStatuses.Remove(statusAbility.StatusConfigObject.ID);
            }
            this.Publish(new RemoveStatusEvent() { CombatEntity = this, Status = statusAbility, StatusId = statusAbility.Id });
        }

        public void BindSkillInput(SkillAbility abilityEntity, KeyCode keyCode)
        {
            InputSkills.Add(keyCode, abilityEntity);
            abilityEntity.TryActivateAbility();
        }

        public bool HasStatus<T>(T statusType) where T : StatusAbility
        {
            return TypeStatuses.ContainsKey(statusType.GetType());
        }
        
        public bool HasStatus(string statusTypeId)
        {
            return TypeIdStatuses.ContainsKey(statusTypeId);
        }

        public StatusAbility GetStatus(string statusTypeId)
        {
            return TypeIdStatuses[statusTypeId][0];
        }

        #region 回合制战斗
        public int SeatNumber { get; set; }
        public int JumpToTime { get; set; }
        public bool IsHero { get; set; }
        public bool IsMonster => IsHero == false;

        public CombatEntity GetEnemy(int seat)
        {
            if (IsHero)
            {
                return GetParent<CombatContext>().GetMonster(seat);
            }
            else
            {
                return GetParent<CombatContext>().GetHero(seat);
            }
        }

        public CombatEntity GetTeammate(int seat)
        {
            if (IsHero)
            {
                return GetParent<CombatContext>().GetHero(seat);
            }
            else
            {
                return GetParent<CombatContext>().GetMonster(seat);
            }
        }
        #endregion
    }

    public class RemoveStatusEvent
    {
        public CombatEntity CombatEntity { get; set; }
        public StatusAbility Status { get; set; }
        public long StatusId { get; set; }
    }
}