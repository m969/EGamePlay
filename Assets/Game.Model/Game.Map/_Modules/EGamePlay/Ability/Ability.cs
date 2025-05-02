using System;
using GameUtils;
using ET;
using System.Collections.Generic;
using UnityEngine;
using ECS;
#if EGAMEPLAY_ET
using SkillConfig = cfg.Skill.SkillCfg;
using AO;
#endif

namespace EGamePlay.Combat
{
    public partial class Ability : EcsEntity
    {
        public CombatEntity OwnerEntity { get { return GetParent<CombatEntity>(); } set { } }
        public EcsEntity ParentEntity { get => Parent; }
        public AbilityConfig Config { get; set; }
        public AbilityConfigObject ConfigObject { get; set; }
        public bool Spelling { get; set; }
        public GameTimer CooldownTimer { get; } = new GameTimer(1f);
        public ExecutionObject ExecutionObject { get; set; }
        public bool IsBuff => Config.Type == "Buff";
        public bool IsSkill => !IsBuff;
        public string Name { get; set; }

        public List<AbilityEffect> AbilityEffects { get; private set; } = new List<AbilityEffect>();
        //public AbilityEffect DamageAbilityEffect { get; set; }
        //public AbilityEffect CureAbilityEffect { get; set; }
        public List<AbilityTrigger> AbilityTriggers { get; private set; } = new List<AbilityTrigger>();
    }
}
