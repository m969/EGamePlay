using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System;
using ECSGame;
using ET;
using System.Linq;

namespace EGamePlay
{
    public class CombatEntitySystem : AEntitySystem<CombatEntity>,
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
            entity.Enable = true;
        }

        public void Update(CombatEntity entity)
        {

        }

        public void FixedUpdate(CombatEntity entity)
        {
            //if (entity.GetComponent<MotionComponent>() is { } component)
            //{
            //    if (component.Enable)
            //    {
            //        MotionSystem.FixedUpdate(entity, component);
            //    }
            //}
        }

        public static CombatEntity Create(Actor actor)
        {
            var entity = actor.AddChild<CombatEntity>();
            entity.Actor = actor;
            entity.AddComponent<TransformComponent>();
            entity.AddComponent<AttributeComponent>();
            entity.AddComponent<HealthPointComponent>();
            entity.AddComponent<BehaviourPointComponent>();
            entity.AddComponent<AbilityComponent>();
            entity.AddComponent<BuffComponent>();
            entity.AddComponent<SkillComponent>();
            entity.AddComponent<SpellComponent>();
            //entity.AddComponent<MotionComponent>();

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

        public static void HeroInit(CombatEntity entity)
        {
            AttributeSystem.InitializeCharacter(entity);
            HealthSystem.Reset(entity);

            HealthSystem.Minus(entity, 30000);

            var allConfigs = StaticConfig.Config.GetAll<AbilityConfig>().Values.ToArray();
            for (int i = 0; i < allConfigs.Length; i++)
            {
                var config = allConfigs[i];
                if (config.Type != "Skill")
                {
                    continue;
                }
                var skilld = config.Id;
                if (skilld == 3001)
                {
                    continue;
                }
                var configObj = GameUtils.AssetUtils.LoadObject<AbilityConfigObject>($"{AbilityManagerObject.SkillResFolder}/Skill_{skilld}");
                var ability = SkillSystem.Attach(entity, configObj);
                if (skilld == 1001) BindSkillInput(entity, ability, KeyCode.Q);
                if (skilld == 1002) BindSkillInput(entity, ability, KeyCode.W);
                if (skilld == 1003) BindSkillInput(entity, ability, KeyCode.Y);
                if (skilld == 1004) BindSkillInput(entity, ability, KeyCode.E);
                if (skilld == 1005) BindSkillInput(entity, ability, KeyCode.R);
                if (skilld == 1008) BindSkillInput(entity, ability, KeyCode.A);
            }

            SpellSystem.LoadExecutionObjects(entity);
        }

        public static void MonsterInit(CombatEntity entity)
        {
            AttributeSystem.InitializeCharacter(entity);
            HealthSystem.Reset(entity);
        }

        public static T AttachAction<T>(CombatEntity entity) where T : EcsEntity, IActionAbility, new()
        {
            var action = entity.AddChild<T>();
            action.Enable = true;
            return action;
        }

        public static Ability AttachStatus(CombatEntity entity, object configObject)
        {
            return BuffSystem.Attach(entity, configObject);
        }

        public static void BindSkillInput(CombatEntity entity, Ability abilityEntity, KeyCode keyCode)
        {
            entity.GetComponent<SkillComponent>().InputSkills.Add(keyCode, abilityEntity);
            AbilitySystem.TryActivateAbility(abilityEntity);
        }
    }
}