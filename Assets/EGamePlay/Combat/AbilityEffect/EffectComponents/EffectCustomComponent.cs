using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class EffectCustomComponent : Component
    {
        public override bool DefaultEnable => false;


        public override void Setup()
        {

        }

        public override void OnEnable()
        {
            if (GetEntity<AbilityEffect>().EffectConfig is CustomEffect customEffect)
            {
                if (customEffect.CustomEffectType == "格挡普攻")
                {
                    var probabilityTriggerComponent = GetEntity<AbilityEffect>().OwnerEntity.AttackBlockAbility.AddComponent<AbilityProbabilityTriggerComponent>();
                    probabilityTriggerComponent.Probability = (int)(float.Parse(customEffect.TriggerProbability.Replace("%", "")) * 100);
                }
            }
        }
    }
}