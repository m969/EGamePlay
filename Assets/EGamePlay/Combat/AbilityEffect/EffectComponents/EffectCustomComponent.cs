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


        public override void Awake()
        {
            Entity.OnEvent(nameof(AbilityEffect.StartAssignEffect), OnAssignEffect);
        }

        public override void OnEnable()
        {
            if (GetEntity<AbilityEffect>().EffectConfig is CustomEffect customEffect)
            {
                if (customEffect.CustomEffectType == "强体")
                {
                    var probabilityTriggerComponent = GetEntity<AbilityEffect>().OwnerEntity.AttackBlockAbility.AddComponent<AbilityProbabilityTriggerComponent>();
                    var param = customEffect.Params.First().Value;
                    probabilityTriggerComponent.Probability = (int)(float.Parse(param.Replace("%", "")) * 100);
                }
            }
        }

        private void OnAssignEffect(Entity entity)
        {

        }
    }
}