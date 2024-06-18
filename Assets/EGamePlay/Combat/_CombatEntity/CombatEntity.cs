using EGamePlay.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if EGAMEPLAY_ET
using Unity.Mathematics;
using Vector3 = Unity.Mathematics.float3;
using Quaternion = Unity.Mathematics.float3;
#endif

namespace EGamePlay.Combat
{
    public class EntityDeadEvent { public Entity DeadEntity; }

    /// <summary>
    /// 战斗实体
    /// </summary>
    public sealed class CombatEntity : Entity, IPosition
    {
        public GameObject HeroObject { get; set; }
        public Transform ModelTrans { get; set; }
        public HealthPointComponent CurrentHealth { get; private set; }

        //效果赋给行动能力
        public EffectAssignAbility EffectAssignAbility { get; private set; }
        //施法行动能力
        public SpellActionAbility SpellAbility { get; private set; }
        //移动行动能力
        public MotionActionAbility MotionAbility { get; private set; }
        //伤害行动能力
        public DamageActionAbility DamageAbility { get; private set; }
        //治疗行动能力
        public CureActionAbility CureAbility { get; private set; }
        //施加状态行动能力
        public AddStatusActionAbility AddStatusAbility { get; private set; }
        //施法普攻行动能力
        public AttackActionAbility AttackSpellAbility { get; private set; }
        //回合行动能力
        public RoundActionAbility RoundAbility { get; private set; }
        //起跳行动能力
        public JumpToActionAbility JumpToAbility { get; private set; }
        public CollisionActionAbility CollisionAbility { get; private set; }

        //普攻能力
        public AttackAbility AttackAbility { get; set; }
        //普攻格挡能力
        public AttackBlockActionAbility AttackBlockAbility { get; set; }

        //执行中的执行体
        public SkillExecution SpellingExecution { get; set; }
        //public Dictionary<string, SkillAbility> NameSkills { get; set; } = new Dictionary<string, SkillAbility>();
        //public Dictionary<int, SkillAbility> IdSkills { get; set; } = new Dictionary<int, SkillAbility>();
        //public Dictionary<KeyCode, SkillAbility> InputSkills { get; set; } = new Dictionary<KeyCode, SkillAbility>();
        //public Dictionary<string, List<StatusAbility>> TypeIdStatuses { get; set; } = new Dictionary<string, List<StatusAbility>>();
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        /// 行为禁制
        public ActionControlType ActionControlType { get; set; }
        /// 禁制豁免
        public ActionControlType ActionControlImmuneType { get; set; }


        public override void Awake()
        {
            AddComponent<AttributeComponent>().InitializeCharacter();
            AddComponent<ActionPointComponent>();
            //AddComponent<ConditionEventComponent>();
            AddComponent<StatusComponent>();
            AddComponent<SkillComponent>();
            AddComponent<SpellComponent>();
            AddComponent<MotionComponent>();
            AddComponent<AbilityComponent>();
            CurrentHealth = AddComponent<HealthPointComponent>();
            CurrentHealth.HealthPointNumeric = GetComponent<AttributeComponent>().HealthPoint;
            CurrentHealth.HealthPointMaxNumeric = GetComponent<AttributeComponent>().HealthPointMax;
            CurrentHealth.Reset();

            AttackAbility = GetComponent<AbilityComponent>().AttachAbility<AttackAbility>(null);
            AttackBlockAbility = AttachAction<AttackBlockActionAbility>();

            EffectAssignAbility = AttachAction<EffectAssignAbility>();
            SpellAbility = AttachAction<SpellActionAbility>();
            MotionAbility = AttachAction<MotionActionAbility>();
            DamageAbility = AttachAction<DamageActionAbility>();
            CureAbility = AttachAction<CureActionAbility>();
            AddStatusAbility = AttachAction<AddStatusActionAbility>();
            AttackSpellAbility = AttachAction<AttackActionAbility>();
            RoundAbility = AttachAction<RoundActionAbility>();
            JumpToAbility = AttachAction<JumpToActionAbility>();
            CollisionAbility = AttachAction<CollisionActionAbility>();
        }

        #region 行动点事件
        public void ListenActionPoint(ActionPointType actionPointType, Action<Entity> action)
        {
            GetComponent<ActionPointComponent>().AddListener(actionPointType, action);
        }

        public void UnListenActionPoint(ActionPointType actionPointType, Action<Entity> action)
        {
            GetComponent<ActionPointComponent>().RemoveListener(actionPointType, action);
        }

        public void TriggerActionPoint(ActionPointType actionPointType, Entity action)
        {
            GetComponent<ActionPointComponent>().TriggerActionPoint(actionPointType, action);
        }
        #endregion

        /// <summary>
        /// 挂载能力，技能、被动、buff等都通过这个接口挂载
        /// </summary>
        /// <param name="configObject"></param>
        public T AttachAbility<T>(object configObject) where T : Entity, IAbilityEntity
        {
            var ability = this.AddChild<T>(configObject);
            ability.AddComponent<AbilityLevelComponent>();
            return ability;
        }

        public T AttachAction<T>() where T : Entity, IActionAbility
        {
            var action = AddChild<T>();
            action.AddComponent<ActionComponent>();
            action.Enable = true;
            //var action = AttachAbility<T>(null);
            //action.TryActivateAbility();
            return action;
        }

        public SkillAbility AttachSkill(object configObject)
        {
            var abilityComp = GetComponent<AbilityComponent>();
            var skill = abilityComp.AttachAbility<SkillAbility>(configObject);
            abilityComp.NameSkills.Add(skill.SkillConfig.Name, skill);
            abilityComp.IdSkills.Add(skill.SkillConfig.Id, skill);
            return skill;
        }

        public StatusAbility AttachStatus(object configObject)
        {
            return GetComponent<StatusComponent>().AttachStatus(configObject);
        }

        //public void OnStatusRemove(StatusAbility statusAbility)
        //{
        //    GetComponent<StatusComponent>().OnStatusRemove(statusAbility);
        //}

        public void BindSkillInput(SkillAbility abilityEntity, KeyCode keyCode)
        {
            GetComponent<AbilityComponent>().InputSkills.Add(keyCode, abilityEntity);
            abilityEntity.TryActivateAbility();
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
        public Entity Entity { get; set; }
        public StatusAbility Status { get; set; }
        public long StatusId { get; set; }
    }
}