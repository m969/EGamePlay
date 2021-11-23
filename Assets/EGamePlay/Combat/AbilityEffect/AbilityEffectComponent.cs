using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class AbilityEffectComponent : Component
    {
        public override bool DefaultEnable { get; set; } = false;
        public List<AbilityEffect> AbilityEffects { get; private set; } = new List<AbilityEffect>();
        public AbilityEffect DamageAbilityEffect { get; set; }
        public AbilityEffect CureAbilityEffect { get; set; }


        public override void Setup(object initData)
        {
            var effects = initData as List<Effect>;
            foreach (var item in effects)
            {
                //Log.Debug($"AbilityEffectComponent Setup {item}");
                var abilityEffect = Entity.AddChild<AbilityEffect>(item);
                AddEffect(abilityEffect);

                if (abilityEffect.EffectConfig is DamageEffect)
                {
                    DamageAbilityEffect = abilityEffect;
                }
                if (abilityEffect.EffectConfig is CureEffect)
                {
                    CureAbilityEffect = abilityEffect;
                }
            }
        }

        public override void OnEnable()
        {
            foreach (var item in AbilityEffects)
            {
                item.EnableEffect();
            }
        }

        public override void OnDisable()
        {
            foreach (var item in AbilityEffects)
            {
                item.DisableEffect();
            }
        }

        public void AddEffect(AbilityEffect abilityEffect)
        {
            AbilityEffects.Add(abilityEffect);
        }

        //public void SetOneEffect(AbilityEffect abilityEffect)
        //{
        //    AbilityEffects.Clear();
        //    AbilityEffects.Add(abilityEffect);
        //}

        //public void FillEffects(List<AbilityEffect> abilityEffects)
        //{
        //    AbilityEffects.Clear();
        //    AbilityEffects.AddRange(abilityEffects);
        //}

        public AbilityEffect GetEffect(int index = 0)
        {
            return AbilityEffects[index];
        }

        //public void ApplyAllEffectsTo(CombatEntity targetEntity)
        //{
        //    if (AbilityEffects.Count > 0)
        //    {
        //        foreach (var abilityEffect in AbilityEffects)
        //        {
        //            abilityEffect.ApplyEffectTo(targetEntity);
        //        }
        //    }
        //}

        //public void ApplyEffectByIndex(CombatEntity targetEntity, int index)
        //{
        //    AbilityEffects[index].ApplyEffectTo(targetEntity);
        //}
    }
}