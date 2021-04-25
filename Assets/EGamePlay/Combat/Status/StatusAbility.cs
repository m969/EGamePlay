using EGamePlay.Combat.Ability;
using System.Collections.Generic;
using ET;

namespace EGamePlay.Combat.Status
{
    public class StatusAbility : AbilityEntity
    {
        //投放者、施术者
        public CombatEntity Caster { get; set; }
        public StatusConfigObject StatusConfigObject { get; set; }
        public FloatModifier NumericModifier { get; set; }
        public bool IsChildStatus { get; set; }
        public ChildStatus ChildStatusData { get; set; }
        private List<StatusAbility> ChildrenStatuses { get; set; } = new List<StatusAbility>();


        public override void Awake(object initData)
        {
            base.Awake(initData);
            StatusConfigObject = initData as StatusConfigObject;
            Name = StatusConfigObject.ID;
        }

        //激活
        public override void ActivateAbility()
        {
            base.ActivateAbility();

            //子状态效果
            if (StatusConfigObject.EnableChildrenStatuses)
            {
                foreach (var item in StatusConfigObject.ChildrenStatuses)
                {
                    var status = OwnerEntity.AttachStatus<StatusAbility>(item.StatusConfigObject);
                    status.Caster = Caster;
                    status.IsChildStatus = true;
                    status.ChildStatusData = item;
                    status.TryActivateAbility();
                    ChildrenStatuses.Add(status);
                }
            }
            //行为禁制
            if (StatusConfigObject.EnabledStateModify)
            {
                OwnerEntity.ActionControlType = OwnerEntity.ActionControlType | StatusConfigObject.ActionControlType;
                //Log.Debug($"{OwnerEntity.ActionControlType}");
                if (OwnerEntity.ActionControlType.HasFlag(ActionControlType.MoveForbid))
                {
                    OwnerEntity.GetComponent<MotionComponent>().Enable = false;
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
                        OwnerEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).AddFinalAddModifier(NumericModifier);
                    }
                    if (StatusConfigObject.ModifyType == ModifyType.PercentAdd)
                    {
                        OwnerEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).AddFinalPctAddModifier(NumericModifier);
                    }
                }
            }
            //逻辑触发
            if (StatusConfigObject.EnabledLogicTrigger)
            {
                foreach (var effectItem in StatusConfigObject.Effects)
                {
                    if (IsChildStatus)
                    {
                        if (effectItem is DamageEffect damageEffect)
                        {
                            damageEffect.DamageValueProperty = damageEffect.DamageValueFormula;
                            foreach (var paramItem in ChildStatusData.Params)
                            {
                                damageEffect.DamageValueProperty = damageEffect.DamageValueProperty.Replace(paramItem.Key, paramItem.Value);
                            }
                        }
                        else if (effectItem is CureEffect cureEffect)
                        {
                            cureEffect.CureValueProperty = cureEffect.CureValueFormula;
                            foreach (var paramItem in ChildStatusData.Params)
                            {
                                cureEffect.CureValueProperty = cureEffect.CureValueProperty.Replace(paramItem.Key, paramItem.Value);
                            }
                        }
                    }
                    var logicEntity = Entity.CreateWithParent<LogicEntity>(this, effectItem);
                    if (effectItem.EffectTriggerType == EffectTriggerType.Instant)
                    {
                        logicEntity.ApplyEffect();
                        Destroy(logicEntity);
                    }
                    else if (effectItem.EffectTriggerType == EffectTriggerType.Interval)
                    {
                        if (IsChildStatus)
                        {
                            effectItem.IntervalValue = effectItem.Interval;
                            foreach (var paramItem in ChildStatusData.Params)
                            {
                                effectItem.IntervalValue = effectItem.IntervalValue.Replace(paramItem.Key, paramItem.Value);
                            }
                        }
                        logicEntity.AddComponent<LogicIntervalTriggerComponent>();
                    }
                    else if (effectItem.EffectTriggerType == EffectTriggerType.Condition)
                    {
                        if (IsChildStatus)
                        {
                            effectItem.ConditionParamValue = effectItem.ConditionParam;
                            foreach (var paramItem in ChildStatusData.Params)
                            {
                                effectItem.ConditionParamValue = effectItem.ConditionParamValue.Replace(paramItem.Key, paramItem.Value);
                            }
                        }
                        logicEntity.AddComponent<LogicConditionTriggerComponent>();
                    }
                    else if (effectItem.EffectTriggerType == EffectTriggerType.Action)
                    {
                        logicEntity.AddComponent<LogicActionTriggerComponent>();
                    }
                }
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
                OwnerEntity.ActionControlType = OwnerEntity.ActionControlType & (~StatusConfigObject.ActionControlType);
                //Log.Debug($"{OwnerEntity.ActionControlType}");
                if (OwnerEntity.ActionControlType.HasFlag(ActionControlType.MoveForbid) == false)
                {
                    OwnerEntity.GetComponent<MotionComponent>().Enable = true;
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
                        OwnerEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).RemoveFinalAddModifier(NumericModifier);
                    }
                    if (StatusConfigObject.ModifyType == ModifyType.PercentAdd)
                    {
                        OwnerEntity.GetComponent<AttributeComponent>().GetNumeric(attributeType).RemoveFinalPctAddModifier(NumericModifier);
                    }
                }
            }
            //逻辑触发
            if (StatusConfigObject.EnabledLogicTrigger)
            {

            }

            NumericModifier = null;
            OwnerEntity.OnStatusRemove(this);
            base.EndAbility();
        }

        //应用能力效果
        public override void ApplyAbilityEffectsTo(CombatEntity targetEntity)
        {
            List<Effect> Effects = null;
            if (StatusConfigObject.EnabledLogicTrigger)
            {
                Effects = StatusConfigObject.Effects;
            }
            if (Effects == null)
            {
                return;
            }
            foreach (var effectItem in Effects)
            {
                ApplyEffectTo(targetEntity, effectItem);
            }
        }
    }
}