using EGamePlay.Combat;
using System.Collections.Generic;
using ET;

namespace EGamePlay.Combat
{
    public class StatusAbility : AbilityEntity
    {
        //投放者、施术者
        public override CombatEntity OwnerEntity { get; set; }
        public StatusConfigObject StatusConfigObject { get; set; }
        public FloatModifier NumericModifier { get; set; }
        public bool IsChildStatus { get; set; }
        public ChildStatus ChildStatusData { get; set; }
        private List<StatusAbility> ChildrenStatuses { get; set; } = new List<StatusAbility>();


        public override void Awake(object initData)
        {
            base.Awake(initData);
            var statusConfig = StatusConfigObject = initData as StatusConfigObject;
            Name = StatusConfigObject.ID;

            //子状态效果
            if (StatusConfigObject.EnableChildrenStatuses)
            {

            }
            //行为禁制
            if (StatusConfigObject.EnabledStateModify)
            {

            }
            //属性修饰
            if (StatusConfigObject.EnabledAttributeModify)
            {
                AddComponent<StatusAttributeModifyComponent>();
            }
            //逻辑触发
            if (StatusConfigObject.EnabledLogicTrigger)
            {
                AddComponent<AbilityEffectComponent>(StatusConfigObject.Effects);
            }
        }

        public void ProccessInputKVParams(Dictionary<string, string> Params)
        {
            for (int i = 0; i < StatusConfigObject.Effects.Count; i++)
            {
                var abilityEffect = AbilityEffectComponent.GetEffect(i);
                var logicEffect = abilityEffect.EffectConfig;

                if (logicEffect.EffectTriggerType == EffectTriggerType.Interval)
                {
                    if (!string.IsNullOrEmpty(logicEffect.Interval))
                    {
                        var intervalComponent = abilityEffect.GetComponent<EffectIntervalTriggerComponent>();
                        intervalComponent.IntervalValue = logicEffect.Interval;
                        foreach (var aInputKVItem in Params)
                        {
                            intervalComponent.IntervalValue = intervalComponent.IntervalValue.Replace(aInputKVItem.Key, aInputKVItem.Value);
                        }
                    }
                }
                if (logicEffect.EffectTriggerType == EffectTriggerType.Condition)
                {
                    if (!string.IsNullOrEmpty(logicEffect.ConditionParam))
                    {
                        var conditionComponent = abilityEffect.GetComponent<EffectConditionTriggerComponent>();
                        conditionComponent.ConditionParamValue = logicEffect.ConditionParam;
                        foreach (var aInputKVItem in Params)
                        {
                            conditionComponent.ConditionParamValue = conditionComponent.ConditionParamValue.Replace(aInputKVItem.Key, aInputKVItem.Value);
                        }
                    }
                }

                if (logicEffect is DamageEffect damage)
                {
                    var damageComponent = abilityEffect.GetComponent<EffectDamageComponent>();
                    damageComponent.DamageValueProperty = damage.DamageValueFormula;
                    foreach (var aInputKVItem in Params)
                    {
                        if (!string.IsNullOrEmpty(damageComponent.DamageValueProperty))
                        {
                            damageComponent.DamageValueProperty = damageComponent.DamageValueProperty.Replace(aInputKVItem.Key, aInputKVItem.Value);
                        }
                    }
                }
                if (logicEffect is CureEffect cure)
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

        //激活
        public override void ActivateAbility()
        {
            base.ActivateAbility();

            //子状态效果
            if (StatusConfigObject.EnableChildrenStatuses)
            {
                foreach (var childStatusData in StatusConfigObject.ChildrenStatuses)
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
            //行为禁制
            if (StatusConfigObject.EnabledStateModify)
            {
                ParentEntity.ActionControlType = ParentEntity.ActionControlType | StatusConfigObject.ActionControlType;
                //Log.Debug($"{OwnerEntity.ActionControlType}");
                if (ParentEntity.ActionControlType.HasFlag(ActionControlType.MoveForbid))
                {
                    ParentEntity.GetComponent<MotionComponent>().Enable = false;
                }
            }
            //属性修饰
            if (StatusConfigObject.EnabledAttributeModify)
            {
                if (StatusConfigObject.AttributeType != AttributeType.None && StatusConfigObject.NumericValue != "")
                {
                    var numericValue = StatusConfigObject.NumericValue;
                    if (IsChildStatus)
                    {
                        foreach (var paramItem in ChildStatusData.Params)
                        {
                            numericValue = numericValue.Replace(paramItem.Key, paramItem.Value);
                        }
                    }
                    numericValue = numericValue.Replace("%", "");
                    var expression = ExpressionHelper.ExpressionParser.EvaluateExpression(numericValue);
                    var value = (float)expression.Value;
                    NumericModifier = new FloatModifier() { Value = value };

                    var attributeType = StatusConfigObject.AttributeType.ToString();
                    if (StatusConfigObject.ModifyType == ModifyType.Add)
                    {
                        ParentEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).AddFinalAddModifier(NumericModifier);
                    }
                    if (StatusConfigObject.ModifyType == ModifyType.PercentAdd)
                    {
                        ParentEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).AddFinalPctAddModifier(NumericModifier);
                    }
                }
            }
            //逻辑触发
            if (StatusConfigObject.EnabledLogicTrigger)
            {
                AbilityEffectComponent.Enable = true;
            }
        }

        //结束
        public override void EndAbility()
        {
            //子状态效果
            if (StatusConfigObject.EnableChildrenStatuses)
            {
                foreach (var item in ChildrenStatuses)
                {
                    item.EndAbility();
                }
                ChildrenStatuses.Clear();
            }
            //行为禁制
            if (StatusConfigObject.EnabledStateModify)
            {
                ParentEntity.ActionControlType = ParentEntity.ActionControlType & (~StatusConfigObject.ActionControlType);
                //Log.Debug($"{OwnerEntity.ActionControlType}");
                if (ParentEntity.ActionControlType.HasFlag(ActionControlType.MoveForbid) == false)
                {
                    ParentEntity.GetComponent<MotionComponent>().Enable = true;
                }
            }
            //属性修饰
            if (StatusConfigObject.EnabledAttributeModify)
            {
                if (StatusConfigObject.AttributeType != AttributeType.None && StatusConfigObject.NumericValue != "")
                {
                    var attributeType = StatusConfigObject.AttributeType.ToString();
                    if (StatusConfigObject.ModifyType == ModifyType.Add)
                    {
                        ParentEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).RemoveFinalAddModifier(NumericModifier);
                    }
                    if (StatusConfigObject.ModifyType == ModifyType.PercentAdd)
                    {
                        ParentEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).RemoveFinalPctAddModifier(NumericModifier);
                    }
                }
            }
            //逻辑触发
            if (StatusConfigObject.EnabledLogicTrigger)
            {

            }

            NumericModifier = null;
            ParentEntity.OnStatusRemove(this);
            base.EndAbility();
        }
    }
}