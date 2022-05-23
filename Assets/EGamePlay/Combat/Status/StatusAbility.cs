using EGamePlay.Combat;
using System.Collections.Generic;
using ET;

namespace EGamePlay.Combat
{
    public partial class StatusAbility : AbilityEntity
    {
#if !EGAMEPLAY_EXCEL
        /// 投放者、施术者
        public override CombatEntity OwnerEntity { get; set; }
        public StatusConfigObject StatusConfig { get; set; }
        public ActionControlType ActionControlType { get; set; }
        public Dictionary<string, FloatModifier> AddModifiers { get; set; } = new Dictionary<string, FloatModifier>();
        public Dictionary<string, FloatModifier> PctAddModifiers { get; set; } = new Dictionary<string, FloatModifier>();
        public bool IsChildStatus { get; set; }
        public int Duration { get; set; }
        public ChildStatus ChildStatusData { get; set; }
        private List<StatusAbility> ChildrenStatuses { get; set; } = new List<StatusAbility>();


        public override void Awake(object initData)
        {
            base.Awake(initData);
            var statusConfig = StatusConfig = initData as StatusConfigObject;
            Name = StatusConfig.ID;

            /// 逻辑触发
            if (StatusConfig.Effects.Count > 0)
            {
                AddComponent<AbilityEffectComponent>(StatusConfig.Effects);
            }
        }

        /// 激活
        public override void ActivateAbility()
        {
            base.ActivateAbility();

            /// 子状态效果
            if (StatusConfig.EnableChildrenStatuses)
            {
                foreach (var childStatusData in StatusConfig.ChildrenStatuses)
                {
                    var status = ParentEntity.AttachStatus<StatusAbility>(childStatusData.StatusConfigObject);
                    status.OwnerEntity = OwnerEntity;
                    status.IsChildStatus = true;
                    status.ChildStatusData = childStatusData;
                    status.ProccessInputKVParams(childStatusData.Params);
                    status.TryActivateAbility();
                    ChildrenStatuses.Add(status);
                }
            }

            AbilityEffectComponent.Enable = true;
        }

        /// 结束
        public override void EndAbility()
        {
            /// 子状态效果
            if (StatusConfig.EnableChildrenStatuses)
            {
                foreach (var item in ChildrenStatuses)
                {
                    item.EndAbility();
                }
                ChildrenStatuses.Clear();
            }

            foreach (var effect in StatusConfig.Effects)
            {
                if (!effect.Enabled)
                {
                    continue;
                }
            }

            ParentEntity.OnStatusRemove(this);
            base.EndAbility();
        }

        public int GetDuration()
        {
            return Duration;
        }

#endif
        /// 这里处理技能传入的参数数值替换
        public void ProccessInputKVParams(Dictionary<string, string> Params)
        {
            foreach (var abilityEffect in AbilityEffectComponent.AbilityEffects)
            {
                var effect = abilityEffect.EffectConfig;

                if (abilityEffect.TryGet(out EffectIntervalTriggerComponent intervalTriggerComponent))
                {
                    intervalTriggerComponent.IntervalValue = effect.Interval;
                    foreach (var aInputKVItem in Params)
                    {
                        intervalTriggerComponent.IntervalValue = intervalTriggerComponent.IntervalValue.Replace(aInputKVItem.Key, aInputKVItem.Value);
                    }
                }
                if (abilityEffect.TryGet(out EffectConditionTriggerComponent conditionTriggerComponent))
                {
                    conditionTriggerComponent.ConditionParamValue = effect.ConditionParam;
                    foreach (var aInputKVItem in Params)
                    {
                        conditionTriggerComponent.ConditionParamValue = conditionTriggerComponent.ConditionParamValue.Replace(aInputKVItem.Key, aInputKVItem.Value);
                    }
                }

                if (effect is AttributeModifyEffect attributeModify)
                {
                    var attributeModifyComponent = abilityEffect.GetComponent<EffectAttributeModifyComponent>();
                    attributeModifyComponent.ModifyValueFormula = attributeModify.NumericValue;
                    //Log.Debug($"ProccessInputKVParams {attributeModify} {attributeModifyComponent.ModifyValueFormula}");
                    foreach (var aInputKVItem in Params)
                    {
                        if (!string.IsNullOrEmpty(attributeModifyComponent.ModifyValueFormula))
                        {
                            attributeModifyComponent.ModifyValueFormula = attributeModifyComponent.ModifyValueFormula.Replace(aInputKVItem.Key, aInputKVItem.Value);
                        }
                    }
                }
                if (effect is DamageEffect damage)
                {
                    var damageComponent = abilityEffect.GetComponent<EffectDamageComponent>();
                    damageComponent.DamageValueFormula = damage.DamageValueFormula;
                    foreach (var aInputKVItem in Params)
                    {
                        if (!string.IsNullOrEmpty(damageComponent.DamageValueFormula))
                        {
                            damageComponent.DamageValueFormula = damageComponent.DamageValueFormula.Replace(aInputKVItem.Key, aInputKVItem.Value);
                        }
                    }
                }
                if (effect is CureEffect cure)
                {
                    var cureComponent = abilityEffect.GetComponent<EffectCureComponent>();
                    cureComponent.CureValueProperty = cure.CureValueFormula;
                    foreach (var aInputKVItem in Params)
                    {
                        if (!string.IsNullOrEmpty(cureComponent.CureValueProperty))
                        {
                            cureComponent.CureValueProperty = cureComponent.CureValueProperty.Replace(aInputKVItem.Key, aInputKVItem.Value);
                        }
                    }
                }
            }
        }
    }
}


/// 行为禁制
//if (StatusConfig.EnabledStateModify)
//{
//    ParentEntity.ActionControlType = ParentEntity.ActionControlType | StatusConfig.ActionControlType;
//    if (ParentEntity.ActionControlType.HasFlag(ActionControlType.MoveForbid))
//    {
//        ParentEntity.GetComponent<MotionComponent>().Enable = false;
//    }
//}
/// 属性修饰
//if (StatusConfig.EnabledAttributeModify)
//{
//    if (StatusConfig.AttributeType != AttributeType.None && StatusConfig.NumericValue != "")
//    {
//        var numericValue = StatusConfig.NumericValue;
//        if (IsChildStatus)
//        {
//            foreach (var paramItem in ChildStatusData.Params)
//            {
//                numericValue = numericValue.Replace(paramItem.Key, paramItem.Value);
//            }
//        }
//        numericValue = numericValue.Replace("%", "");
//        var expression = ExpressionHelper.ExpressionParser.EvaluateExpression(numericValue);
//        var value = (float)expression.Value;
//        NumericModifier = new FloatModifier() { Value = value };

//        var attributeType = StatusConfig.AttributeType.ToString();
//        if (StatusConfig.ModifyType == ModifyType.Add)
//        {
//            ParentEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).AddFinalAddModifier(NumericModifier);
//        }
//        if (StatusConfig.ModifyType == ModifyType.PercentAdd)
//        {
//            ParentEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).AddFinalPctAddModifier(NumericModifier);
//        }
//    }
//}

///// 行为禁制
//if (StatusConfig.EnabledStateModify)
//{
//    ParentEntity.ActionControlType = ParentEntity.ActionControlType & (~StatusConfig.ActionControlType);
//    //Log.Debug($"{OwnerEntity.ActionControlType}");
//    if (ParentEntity.ActionControlType.HasFlag(ActionControlType.MoveForbid) == false)
//    {
//        ParentEntity.GetComponent<MotionComponent>().Enable = true;
//    }
//}
///// 属性修饰
//if (StatusConfig.EnabledAttributeModify)
//{
//    if (StatusConfig.AttributeType != AttributeType.None && StatusConfig.NumericValue != "")
//    {
//        var attributeType = StatusConfig.AttributeType.ToString();
//        if (StatusConfig.ModifyType == ModifyType.Add)
//        {
//            ParentEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).RemoveFinalAddModifier(NumericModifier);
//        }
//        if (StatusConfig.ModifyType == ModifyType.PercentAdd)
//        {
//            ParentEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).RemoveFinalPctAddModifier(NumericModifier);
//        }
//    }
//}