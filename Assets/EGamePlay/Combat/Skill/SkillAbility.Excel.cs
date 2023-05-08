using System;
using GameUtils;
using ET;
using System.Collections.Generic;
using UnityEngine;

#if EGAMEPLAY_EXCEL
namespace EGamePlay.Combat
{
    public partial class SkillAbility : Entity, IAbilityEntity
    {
        public CombatEntity OwnerEntity { get { return GetParent<CombatEntity>(); } set { } }
        public CombatEntity ParentEntity { get => GetParent<CombatEntity>(); }
        public bool Enable { get; set; }
        public SkillConfig SkillConfig { get; set; }
        public bool Spelling { get; set; }
        public GameTimer CooldownTimer { get; } = new GameTimer(1f);
        private List<StatusAbility> ChildrenStatuses { get; set; } = new List<StatusAbility>();


        public override void Awake(object initData)
        {
            base.Awake(initData);
            SkillConfig = initData as SkillConfig;
            Name = SkillConfig.Name;
            var Effects = new List<Effect>();
            var effect = ParseSkillDamage(SkillConfig);
            if (effect != null) Effects.Add(effect);
            effect = ParseEffect(SkillConfig, SkillConfig.Effect1);
            if (effect != null) Effects.Add(effect);
            effect = ParseEffect(SkillConfig, SkillConfig.Effect2);
            if (effect != null) Effects.Add(effect);
            effect = ParseEffect(SkillConfig, SkillConfig.Effect3);
            if (effect != null) Effects.Add(effect);
            AddComponent<AbilityEffectComponent>(Effects);
#if !SERVER
            Awake_Client();
#endif
            if (SkillConfig.Type == "被动")
            {
                TryActivateAbility();
            }
        }

        public void TryActivateAbility()
        {
            this.ActivateAbility();
        }

        public void DeactivateAbility()
        {
            Enable = false;
        }

        public void ActivateAbility()
        {
            FireEvent(nameof(ActivateAbility));
        }

        public void EndAbility()
        {
            Entity.Destroy(this);
        }

        public Entity CreateExecution()
        {
            var execution = OwnerEntity.AddChild<SkillExecution>(this);
            execution.ExecutionObject = ExecutionObject;
            execution.LoadExecutionEffects();
            this.FireEvent(nameof(CreateExecution), execution);
            if (ExecutionObject != null)
            {
                execution.AddComponent<UpdateComponent>();
            }
            return execution;
        }

        public Effect ParseSkillDamage(SkillConfig skillConfig)
        {
            Effect effect = null;
            if (string.IsNullOrEmpty(skillConfig.DamageTarget) == false)
            {
                var damageEffect = new DamageEffect();
                effect = damageEffect;
                damageEffect.DamageValueFormula = skillConfig.ValueFormula;
                damageEffect.TriggerProbability = skillConfig.Probability;
                if (skillConfig.DamageTarget == "自身") damageEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.Self;
                if (skillConfig.DamageTarget == "技能目标") damageEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.SkillTarget;
                if (skillConfig.DamageType == "魔法伤害") damageEffect.DamageType = DamageType.Magic;
                if (skillConfig.DamageType == "物理伤害") damageEffect.DamageType = DamageType.Physic;
                if (skillConfig.DamageType == "真实伤害") damageEffect.DamageType = DamageType.Real;
            }
            return effect;
        }

        public Effect ParseEffect(SkillConfig skillConfig, string effectConfig)
        {
            Effect effect = null;
            if (!string.IsNullOrEmpty(effectConfig) && effectConfig.Contains("="))
            {
                effectConfig = effectConfig.Replace("=Id", $"={skillConfig.Id}");
                var arr = effectConfig.Split('=');
                var effectType = arr[0];
                var effectId = arr[1];
                var skillEffectConfig = ConfigHelper.Get<SkillEffectsConfig>(int.Parse(effectId));
                var KVList = new List<string>(3);
                KVList.Add(skillEffectConfig.KV1);
                KVList.Add(skillEffectConfig.KV2);
                KVList.Add(skillEffectConfig.KV3);
                if (effectType == "Damage")
                {
                    var Type = "";
                    var DamageValueFormula = "";
                    foreach (var item in KVList)
                    {
                        if (string.IsNullOrEmpty(item)) continue;
                        if (item.Contains("伤害类型=")) Type = item.Replace("伤害类型=", "");
                        if (item.Contains("伤害取值=")) DamageValueFormula = item.Replace("伤害取值=", "");
                    }
                    var damageEffect = new DamageEffect();
                    effect = damageEffect;
                    damageEffect.DamageValueFormula = DamageValueFormula;
                    damageEffect.TriggerProbability = skillEffectConfig.Probability;
                    if (skillEffectConfig.Target == "自身") damageEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.Self;
                    if (skillEffectConfig.Target == "技能目标") damageEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.SkillTarget;
                    if (Type == "魔法伤害") damageEffect.DamageType = DamageType.Magic;
                    if (Type == "物理伤害") damageEffect.DamageType = DamageType.Physic;
                    if (Type == "真实伤害") damageEffect.DamageType = DamageType.Real;
                }
                else if (effectType == "Cure")
                {
                    var Type = "";
                    var CureValueFormula = "";
                    foreach (var item in KVList)
                    {
                        if (string.IsNullOrEmpty(item)) continue;
                        if (item.Contains("治疗类型=")) Type = item.Replace("治疗类型=", "");
                        if (item.Contains("治疗取值=")) CureValueFormula = item.Replace("治疗取值=", "");
                    }
                    var cureEffect = new CureEffect();
                    effect = cureEffect;
                    cureEffect.CureValueFormula = CureValueFormula;
                    cureEffect.TriggerProbability = skillEffectConfig.Probability;
                    if (skillEffectConfig.Target == "自身") cureEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.Self;
                    if (skillEffectConfig.Target == "技能目标") cureEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.SkillTarget;
                }
                else if (effectType == "AddStatus")
                {
                    var StatusID = "";
                    var Duration = "";
                    foreach (var item in KVList)
                    {
                        if (string.IsNullOrEmpty(item)) continue;
                        if (item.Contains("状态类型=")) StatusID = item.Replace("状态类型=", "");
                        if (item.Contains("持续时间=")) Duration = item.Replace("持续时间=", "");
                    }
                    var addStatusEffect = new AddStatusEffect();
                    effect = addStatusEffect;
                    addStatusEffect.AddStatusConfig = StatusConfigCategory.Instance.GetByName(StatusID);
                    addStatusEffect.Duration = (uint)(float.Parse(Duration) * 1000);
                    ParseParam(skillEffectConfig.Param1);
                    ParseParam(skillEffectConfig.Param2);
                    void ParseParam(string paramStr)
                    {
                        if (!string.IsNullOrEmpty(paramStr))
                        {
                            arr = paramStr.Split('=');
                            addStatusEffect.Params.Add(arr[0], arr[1]);
                        }
                    }
                }
                else
                {
                    effect = new CustomEffect() { CustomEffectType = effectConfig };
                }
            }
            return effect;
        }
    }
}
#endif