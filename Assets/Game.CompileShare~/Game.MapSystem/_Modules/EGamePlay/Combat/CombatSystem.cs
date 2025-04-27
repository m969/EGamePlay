using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System;

namespace EGamePlay
{
    public class CombatSystem : AEntitySystem<CombatEntity>,
        IAwake<CombatEntity>,
        IInit<CombatEntity>,
        IUpdate<CombatEntity>,
        IFixedUpdate<CombatEntity>
    {
        public void Awake(CombatEntity entity)
        {

        }
        
        public void Init(CombatEntity entity)
        {
            AttributeSystem.InitializeCharacter(entity);
            HealthPointSystem.Reset(entity);
            entity.Enable = true;
        }

        public void Update(CombatEntity entity)
        {

        }

        public void FixedUpdate(CombatEntity entity)
        {
            if (entity.GetComponent<MotionComponent>() is { } component)
            {
                MotionSystem.FixedUpdate(entity, component);
            }
        }

        public static CombatEntity Create(CombatContext context)
        {
            var entity = context.AddChild<CombatEntity>();
            entity.AddComponent<AttributeComponent>();
            entity.AddComponent<HealthPointComponent>();
            entity.AddComponent<BehaviourPointComponent>();
            entity.AddComponent<AbilityComponent>();
            entity.AddComponent<BuffComponent>();
            entity.AddComponent<SkillComponent>();
            entity.AddComponent<SpellComponent>();
            entity.AddComponent<MotionComponent>();

            entity.EffectAssignAbility = AttachAction<EffectAssignAbility>(entity);
            entity.SpellAbility = AttachAction<SpellAbility>(entity);
            //entity.MotionAbility = AttachAction<MotionAbility>(entity);
            entity.DamageAbility = AttachAction<DamageAbility>(entity);
            entity.CureAbility = AttachAction<CureAbility>(entity);
            entity.AddStatusAbility = AttachAction<AddBuffAbility>(entity);
            entity.AttackSpellAbility = AttachAction<AttackAbility>(entity);
            entity.CollisionAbility = AttachAction<CollisionAbility>(entity);
            return entity;
        }

        #region 行动点事件
        public static void ListenActionPoint(CombatEntity entity, ActionPointType actionPointType, Action<EcsEntity> action)
        {
            BehaviourPointSystem.AddListener(entity, actionPointType, action);
        }

        public static void UnListenActionPoint(CombatEntity entity, ActionPointType actionPointType, Action<EcsEntity> action)
        {
            BehaviourPointSystem.RemoveListener(entity, actionPointType, action);
        }

        public static void TriggerActionPoint(CombatEntity entity, ActionPointType actionPointType, EcsEntity action)
        {
            BehaviourPointSystem.TriggerActionPoint(entity, actionPointType, action);
        }
        #endregion

        public static T AttachAction<T>(CombatEntity entity) where T : EcsEntity, IActionAbility, new()
        {
            var action = entity.AddChild<T>();
            action.AddComponent<ActionComponent>();
            action.Enable = true;
            return action;
        }

        public static Ability AttachStatus(CombatEntity entity, object configObject)
        {
            return BuffSystem.AttachStatus(entity, configObject);
        }

        public static void BindSkillInput(CombatEntity entity, Ability abilityEntity, KeyCode keyCode)
        {
            entity.GetComponent<SkillComponent>().InputSkills.Add(keyCode, abilityEntity);
            AbilitySystem.TryActivateAbility(abilityEntity);
        }

        //#region 回合制战斗
        //public int SeatNumber { get; set; }
        //public int JumpToTime { get; set; }
        //public bool IsHero { get; set; }
        //public bool IsMonster => IsHero == false;

        //public CombatEntity GetEnemy(int seat)
        //{
        //    if (IsHero)
        //    {
        //        return GetParent<CombatContext>().GetMonster(seat);
        //    }
        //    else
        //    {
        //        return GetParent<CombatContext>().GetHero(seat);
        //    }
        //}

        //public CombatEntity GetTeammate(int seat)
        //{
        //    if (IsHero)
        //    {
        //        return GetParent<CombatContext>().GetHero(seat);
        //    }
        //    else
        //    {
        //        return GetParent<CombatContext>().GetMonster(seat);
        //    }
        //}
        //#endregion
    }
}