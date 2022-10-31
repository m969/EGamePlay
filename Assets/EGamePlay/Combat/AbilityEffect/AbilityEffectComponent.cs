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


        public override void Awake(object initData)
        {
            if (initData == null)
            {
                return;
            }
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

        public AbilityEffect GetEffect(int index = 0)
        {
            return AbilityEffects[index];
        }

        /// <summary>   尝试将所有效果赋给目标   </summary>
        public void TryAssignAllEffectsToTargetWithExecution(CombatEntity targetEntity, IAbilityExecution execution)
        {
            Log.Debug($"TryAssignAllEffectsToTargetWithExecution {targetEntity} {AbilityEffects.Count}");
            if (AbilityEffects.Count > 0)
            {
                foreach (var abilityEffect in AbilityEffects)
                {
                    abilityEffect.TryAssignEffectTo(targetEntity);
                }
            }
        }

        /// <summary>   尝试将所有效果赋给目标   </summary>
        public void TryAssignAllEffectsToTargetWithAbilityItem(CombatEntity targetEntity, AbilityItem abilityItem)
        {
            if (AbilityEffects.Count > 0)
            {
                foreach (var abilityEffect in AbilityEffects)
                {
                    abilityEffect.TryAssignEffectToTargetWithAbilityItem(targetEntity, abilityItem);
                }
            }
        }

        public void TryAssignEffectByIndex(CombatEntity targetEntity, int index)
        {
            AbilityEffects[index].TryAssignEffectTo(targetEntity);
        }
    }
}