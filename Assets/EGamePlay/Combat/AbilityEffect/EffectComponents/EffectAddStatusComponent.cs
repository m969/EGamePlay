using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 
    /// </summary>
    public class EffectAddStatusComponent : Component
    {
        public AddStatusEffect AddStatusEffect { get; set; }
        public uint Duration { get; set; }
        public string NumericValueProperty { get; set; }


        public override void Awake()
        {
            AddStatusEffect = GetEntity<AbilityEffect>().EffectConfig as AddStatusEffect;
            Duration = AddStatusEffect.Duration;
            Entity.OnEvent(nameof(AbilityEffect.StartAssignEffect), OnAssignEffect);

#if EGAMEPLAY_EXCEL
            var statusConfig = AddStatusEffect.AddStatusConfig;
            if (statusConfig.EnabledAttributeModify())
            {
                if (!string.IsNullOrEmpty(statusConfig.AttributeParams))
                {
                    NumericValueProperty = statusConfig.AttributeParams;
                    foreach (var aInputKVItem in AddStatusEffect.Params)
                    {
                        NumericValueProperty = NumericValueProperty.Replace(aInputKVItem.Key, aInputKVItem.Value);
                    }
                }
            }
#else

#endif
        }

        public int GetNumericValue()
        {
            return 1;
        }

        private void OnAssignEffect(Entity entity)
        {
            //Log.Debug($"EffectCureComponent OnAssignEffect");
            var effectAssignAction = entity.As<EffectAssignAction>();
            if (GetEntity<AbilityEffect>().OwnerEntity.AddStatusAbility.TryMakeAction(out var action))
            {
                effectAssignAction.FillDatasToAction(action);
                action.SourceAbility = effectAssignAction.SourceAbility;
                action.ApplyAddStatus();
            }
        }
    }
}