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
        //public string Interval { get; set; }
        //public string ConditionParam { get; set; }
        //public string DamageValue { get; set; }
        //public string CureValue { get; set; }


        public override void Setup()
        {
            AddStatusEffect = GetEntity<AbilityEffect>().EffectConfig as AddStatusEffect;

            var statusConfig = AddStatusEffect.AddStatus;
            Duration = AddStatusEffect.Duration;

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
            if (AddStatusEffect.Params != null && statusConfig.Effects != null)
            {
                if (statusConfig.EnabledLogicTrigger)
                {
                    //foreach (var logicEffect in statusConfig.Effects)
                    //{
                    //    logicEffect.IntervalValue = logicEffect.Interval;
                    //    logicEffect.ConditionParamValue = logicEffect.ConditionParam;
                    //    foreach (var aInputParamItem in AddStatusEffect.Params)
                    //    {
                    //        if (!string.IsNullOrEmpty(logicEffect.IntervalValue))
                    //        {
                    //            logicEffect.IntervalValue = logicEffect.IntervalValue.Replace(aInputParamItem.Key, aInputParamItem.Value);
                    //        }
                    //        if (!string.IsNullOrEmpty(logicEffect.ConditionParamValue))
                    //        {
                    //            logicEffect.ConditionParamValue = logicEffect.ConditionParamValue.Replace(aInputParamItem.Key, aInputParamItem.Value);
                    //        }
                    //    }
                    //    if (logicEffect is DamageEffect damage)
                    //    {
                    //        damage.DamageValueProperty = damage.DamageValueFormula;
                    //        foreach (var item4 in AddStatusEffect.Params)
                    //        {
                    //            if (!string.IsNullOrEmpty(damage.DamageValueProperty))
                    //            {
                    //                damage.DamageValueProperty = damage.DamageValueProperty.Replace(item4.Key, item4.Value);
                    //            }
                    //        }
                    //    }
                    //    else if (logicEffect is CureEffect cure)
                    //    {
                    //        cure.CureValueProperty = cure.CureValueFormula;
                    //        foreach (var item5 in AddStatusEffect.Params)
                    //        {
                    //            if (!string.IsNullOrEmpty(cure.CureValueProperty))
                    //            {
                    //                cure.CureValueProperty = cure.CureValueProperty.Replace(item5.Key, item5.Value);
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
        }

        public int GetNumericValue()
        {
            return 1;
        }
    }
}