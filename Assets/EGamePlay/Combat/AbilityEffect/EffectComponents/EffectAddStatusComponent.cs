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


        public override void Setup()
        {
            AddStatusEffect = GetEntity<AbilityEffect>().EffectConfig as AddStatusEffect;
            Duration = AddStatusEffect.Duration;

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
            var statusConfig = AddStatusEffect.AddStatus;
            if (statusConfig.EnabledAttributeModify)
            {
                if (!string.IsNullOrEmpty(statusConfig.NumericValue))
                {
                    NumericValueProperty = statusConfig.NumericValue;
                    foreach (var aInputKVItem in AddStatusEffect.Params)
                    {
                        NumericValueProperty = NumericValueProperty.Replace(aInputKVItem.Key, aInputKVItem.Value);
                    }
                }
            }
#endif
        }

        public int GetNumericValue()
        {
            return 1;
        }
    }
}