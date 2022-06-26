using EGamePlay.Combat;
using System;
using GameUtils;
using ET;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public class AttackAbility : Entity, IAbilityEntity
    {
        public CombatEntity OwnerEntity { get => GetParent<CombatEntity>(); set { } }
        public CombatEntity ParentEntity { get => GetParent<CombatEntity>(); }
        public bool Enable { get; set; }


        public override void Awake(object initData)
        {
            var effects = new List<Effect>();
            var damageEffect = new DamageEffect();
            damageEffect.Enabled = true;
            damageEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.SkillTarget;
            damageEffect.EffectTriggerType = EffectTriggerType.Condition;
            damageEffect.CanCrit = true;
            damageEffect.DamageType = DamageType.Physic;
            damageEffect.DamageValueFormula = $"自身攻击力";
            effects.Add(damageEffect);
            AddComponent<AbilityEffectComponent>(effects);
        }

        //public AttackExecution CreateExecution()
        //{
        //    var execution = OwnerEntity.AddChild<AttackExecution>(this);
        //    execution.AbilityEntity = this;
        //    return execution;
        //}

        public void DeactivateAbility()
        {
        }

        public void EndAbility()
        {
        }

        public void TryActivateAbility()
        {
            ActivateAbility();
        }
        
        public void ActivateAbility()
        {
            Enable = true;
        }

        public Entity CreateExecution()
        {
            var execution = OwnerEntity.AddChild<AttackExecution>(this);
            execution.AbilityEntity = this;
            return execution;
        }
    }
}