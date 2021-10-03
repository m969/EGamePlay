using EGamePlay.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 战斗实体
    /// </summary>
    public sealed class CombatEntity : Entity, IPosition
    {
        public GameObject ModelObject { get; set; }
        public HealthPoint CurrentHealth { get; private set; } = new HealthPoint();

        public SpellActionAbility SpellActionAbility { get; private set; }
        public MotionActionAbility MotionActionAbility { get; private set; }
        public DamageActionAbility DamageActionAbility { get; private set; }
        public CureActionAbility CureActionAbility { get; private set; }
        public AttackActionAbility AttackActionAbility { get; private set; }
        public EffectAssignAbility EffectAssignAbility { get; private set; }
        public RoundActionAbility RoundActionAbility { get; private set; }
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
            AddComponent<ActionComponent>();
            AddComponent<ActionPointComponent>();
            AddComponent<ConditionComponent>();
            AddComponent<StatusComponent>();
            AddComponent<SkillComponent>();
            AddComponent<SpellComponent>();
            CurrentHealth.SetMaxValue((int)GetComponent<AttributeComponent>().HealthPoint.Value);
            CurrentHealth.Reset();
            SpellActionAbility = AttachAction<SpellActionAbility>();
            MotionActionAbility = AttachAction<MotionActionAbility>();
            DamageActionAbility = AttachAction<DamageActionAbility>();
            CureActionAbility = AttachAction<CureActionAbility>();
            AttackActionAbility = AttachAction<AttackActionAbility>();
            EffectAssignAbility = AttachAction<EffectAssignAbility>();
            RoundActionAbility = AttachAction<RoundActionAbility>();
            JumpToActionAbility = AttachAction<JumpToActionAbility>();
            AttackAbility = AttachAbility<AttackAbility>(null);
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
            GetComponent<ActionPointComponent>().AddListener(actionPointType, action);
        }

        public void UnListenActionPoint(ActionPointType actionPointType, Action<ActionExecution> action)
        {
            GetComponent<ActionPointComponent>().RemoveListener(actionPointType, action);
        }

        public void TriggerActionPoint(ActionPointType actionPointType, ActionExecution action)
        {
            GetComponent<ActionPointComponent>().TriggerActionPoint(actionPointType, action);
        }
        #endregion

        #region 条件事件
        public void ListenerCondition(ConditionType conditionType, Action action, object paramObj = null)
        {
            GetComponent<ConditionComponent>().AddListener(conditionType, action, paramObj);
        }

        public void UnListenCondition(ConditionType conditionType, Action action)
        {
            GetComponent<ConditionComponent>().RemoveListener(conditionType, action);
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
        /// 挂载能力，技能、被动、buff等都通过这个接口挂载
        /// </summary>
        /// <param name="configObject"></param>
        private T AttachAbility<T>(object configObject) where T : AbilityEntity
        {
            var ability = this.AddChild<T>(configObject);
            return ability;
        }

        public T AttachAction<T>() where T : EffectActionAbility
        {
            var action = AttachAbility<T>(null);
            return action;
        }

        public T GetAction<T>() where T : EffectActionAbility
        {
            return GetChild<T>();
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