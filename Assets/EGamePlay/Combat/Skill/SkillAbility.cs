using EGamePlay.Combat.Ability;
using System;
using GameUtils;
using ET;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat.Skill
{
    public class SkillAbility : AbilityEntity
    {
#if EGAMEPLAY_EXCEL
        public SkillConfig SkillConfig { get; set; }
#else
        public SkillConfigObject SkillConfig { get; set; }
#endif
        public bool Spelling { get; set; }
        public GameTimer CooldownTimer { get; } = new GameTimer(1f);


        public override void Awake(object initData)
        {
            base.Awake(initData);
#if EGAMEPLAY_EXCEL
            SkillConfig = initData as SkillConfig;
            //SkillConfigObject = new SkillConfigObject();
            //SkillConfigObject.ID = (uint)SkillConfig.Id;
            //SkillConfigObject.Name = SkillConfig.Name;
            //SkillConfigObject.SkillDescription = SkillConfig.Description;
            //SkillConfigObject.SkillSpellType = SkillConfig.Type == "主动" ? SkillSpellType.Initiative : SkillSpellType.Passive;
            //SkillConfigObject.ColdTime = (uint)(SkillConfig.Cooldown * 1000);
            if (SkillConfig.Type == "被动")
            {
                TryActivateAbility();
            }
#else
            SkillConfig = initData as SkillConfigObject;
            if (SkillConfig.SkillSpellType == SkillSpellType.Passive)
            {
                TryActivateAbility();
            }
#endif
        }

        public override AbilityExecution CreateExecution()
        {
            var execution = Entity.CreateWithParent<SkillExecution>(OwnerEntity, this);
            execution.AddComponent<UpdateComponent>();
            return execution;
        }

        public override void ApplyAbilityEffectsTo(CombatEntity targetEntity)
        {
            List<Effect> Effects = null;
#if EGAMEPLAY_EXCEL
            Effects = new List<Effect>();
            var 
            effect = ParseEffect(SkillConfig.Effect1);
            if (effect != null) Effects.Add(effect);
            effect = ParseEffect(SkillConfig.Effect2);
            if (effect != null) Effects.Add(effect);
            effect = ParseEffect(SkillConfig.Effect3);
            if (effect != null) Effects.Add(effect);
#else
            Effects = SkillConfig.Effects;
#endif
            if (Effects == null)
                return;
            foreach (var effectItem in Effects)
            {
                ApplyEffectTo(targetEntity, effectItem);
            }
        }

#if EGAMEPLAY_EXCEL
        public Effect ParseEffect(string effectConfig)
        {
            Effect effect = null;
            if (!string.IsNullOrEmpty(effectConfig) && effectConfig.Contains("="))
            {
                var arr = effectConfig.Split('=');
                var effectType = arr[0];
                var effectId = arr[1];
                if (effectType == "Damage")
                {
                    var damageEffectConfig = ConfigHelper.Get<SkillDamageEffectConfig>(int.Parse(effectId));
                    var damageEffect = new DamageEffect();
                    effect = damageEffect;
                    damageEffect.DamageValueFormula = damageEffectConfig.ValueFormula;
                    damageEffect.TriggerProbability = damageEffectConfig.Probability;
                    if (damageEffectConfig.Target == "自身") damageEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.Self;
                    if (damageEffectConfig.Target == "技能目标") damageEffect.AddSkillEffectTargetType = AddSkillEffetTargetType.SkillTarget;
                    if (damageEffectConfig.Type == "魔法伤害") damageEffect.DamageType = DamageType.Magic;
                    if (damageEffectConfig.Type == "物理伤害") damageEffect.DamageType = DamageType.Physic;
                    if (damageEffectConfig.Type == "真实伤害") damageEffect.DamageType = DamageType.Real;
                }
                if (effectType == "AddStatus")
                {
                    var addStatusEffectConfig = ConfigHelper.Get<SkillAddStatusEffectConfig>(int.Parse(effectId));
                    var addStatusEffect = new AddStatusEffect();
                    effect = addStatusEffect;
                    addStatusEffect.AddStatus = Resources.Load<StatusConfigObject>($"StatusConfigs/Status_{addStatusEffectConfig.StatusID}");
                    if (addStatusEffect.AddStatus == null)
                    {
                        addStatusEffect.AddStatus = Resources.Load<StatusConfigObject>($"StatusConfigs/BaseStatus/Status_{addStatusEffectConfig.StatusID}");
                    }
                    addStatusEffect.Duration = (uint)(float.Parse(addStatusEffectConfig.Duration) * 1000);
                    ParseParam(addStatusEffectConfig.Param1);
                    ParseParam(addStatusEffectConfig.Param2);
                    void ParseParam(string paramStr)
                    {
                        if (!string.IsNullOrEmpty(paramStr))
                        {
                            arr = paramStr.Split('=');
                            addStatusEffect.Params.Add(arr[0], arr[1]);
                        }
                    }
                }
            }
            else
            {
                effect = new CustomEffect() {  CustomEffectType = effectConfig };
            }
            return effect;
        }
#else

#endif
    }
}