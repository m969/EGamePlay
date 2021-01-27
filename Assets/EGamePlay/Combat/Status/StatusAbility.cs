﻿using EGamePlay.Combat.Ability;
using System.Collections.Generic;

namespace EGamePlay.Combat.Status
{
    public class StatusAbility : AbilityEntity
    {
        //投放者、施术者
        public CombatEntity Caster { get; set; }
        public StatusConfigObject StatusConfigObject { get; set; }
        public FloatModifier NumericModifier { get; set; }
        //public bool ShowInStatusSlots { get; set; }
        public bool IsChildStatus { get; set; }
        public ChildStatus ChildStatusData { get; set; }
        private List<StatusAbility> ChildrenStatuses { get; set; } = new List<StatusAbility>();


        public override void Awake(object initData)
        {
            base.Awake(initData);
            StatusConfigObject = initData as StatusConfigObject;
            Name = StatusConfigObject.ID;
            //ShowInStatusSlots = StatusConfigObject.ShowInStatusSlots;
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
                    status.Caster = OwnerEntity;
                    status.IsChildStatus = true;
                    status.ChildStatusData = item;
                    status.TryActivateAbility();
                    ChildrenStatuses.Add(status);
                }
            }
            //行为禁制
            if (StatusConfigObject.EnabledStateModify)
            {
                OwnerEntity.ActionControlType |= StatusConfigObject.ActionControlType;
//                OwnerEntity.ActionControlType |= CheckActionControlType(ActionControlType.MoveForbid);
//                OwnerEntity.ActionControlType |= CheckActionControlType(ActionControlType.AttackForbid);
//                OwnerEntity.ActionControlType |= CheckActionControlType(ActionControlType.SkillForbid);
//                ActionControlType CheckActionControlType(ActionControlType ActionControlType)
//                {
//                    return StatusConfigObject.ActionControlType.HasFlag(ActionControlType) ? ActionControlType : ~ActionControlType;
//                }
            }
            //属性修饰
            if (StatusConfigObject.EnabledAttributeModify)
            {
                if (StatusConfigObject.AttributeType != AttributeType.None && StatusConfigObject.NumericValue != "")
                {
                    var numericValue = StatusConfigObject.NumericValue;
                    if (IsChildStatus)
                    {
                        foreach (var item in ChildStatusData.Params)
                        {
                            numericValue = numericValue.Replace(item.Key, item.Value);
                        }
                    }
                    numericValue = numericValue.Replace("%", "");
                    var expression = ExpressionHelper.ExpressionParser.EvaluateExpression(numericValue);
                    var value = (float)expression.Value;
                    NumericModifier = new FloatModifier() { Value = value };

                    //switch (StatusConfigObject.AttributeType)
                    //{
                    //    case AttributeType.None:
                    //        break;
                    //    case AttributeType.MoveSpeed:
                    //        break;
                    //    case AttributeType.AttackPower:
                    //        break;
                    //    case AttributeType.AttackDefense:
                    //        break;
                    //    case AttributeType.SpellPower:
                    //        break;
                    //    case AttributeType.MagicDefense:
                    //        break;
                    //    case AttributeType.CriticalProbability:
                    //        break;
                    //    default:
                    //        break;
                    //}

                    var attributeType = StatusConfigObject.AttributeType.ToString();
                    if (StatusConfigObject.ModifyType == ModifyType.Add)
                    {
                        GetParent<CombatEntity>().GetComponent<AttributeComponent>().GetNumeric(attributeType).AddFinalAddModifier(NumericModifier);
                    }
                    if (StatusConfigObject.ModifyType == ModifyType.PercentAdd)
                    {
                        GetParent<CombatEntity>().GetComponent<AttributeComponent>().GetNumeric(attributeType).AddFinalPctAddModifier(NumericModifier);
                    }
                }
            }
            //逻辑触发
            if (StatusConfigObject.EnabledLogicTrigger)
            {
                foreach (var item in StatusConfigObject.Effects)
                {
                    var logicEntity = Entity.CreateWithParent<LogicEntity>(this, item);
                    if (item.EffectTriggerType == EffectTriggerType.Instant)
                    {
                        logicEntity.ApplyEffect();
                        Destroy(logicEntity);
                    }
                    if (item.EffectTriggerType == EffectTriggerType.Interval)
                    {
                        logicEntity.AddComponent<LogicIntervalTriggerComponent>();
                    }
                    if (item.EffectTriggerType == EffectTriggerType.Condition)
                    {
                        logicEntity.AddComponent<LogicConditionTriggerComponent>();
                    }
                    if (item.EffectTriggerType == EffectTriggerType.Action)
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
                OwnerEntity.ActionControlType |= ~StatusConfigObject.ActionControlType;
            }
            //属性修饰
            if (StatusConfigObject.EnabledAttributeModify)
            {
                if (StatusConfigObject.AttributeType != AttributeType.None && StatusConfigObject.NumericValue != "")
                {
                    //switch (StatusConfigObject.AttributeType)
                    //{
                    //    case AttributeType.None:
                    //        break;
                    //    case AttributeType.MoveSpeed:
                    //        break;
                    //    case AttributeType.AttackPower:
                    //        break;
                    //    case AttributeType.AttackDefense:
                    //        break;
                    //    case AttributeType.SpellPower:
                    //        break;
                    //    case AttributeType.MagicDefense:
                    //        break;
                    //    case AttributeType.CriticalProbability:
                    //        break;
                    //    default:
                    //        break;
                    //}

                    var attributeType = StatusConfigObject.AttributeType.ToString();
                    if (StatusConfigObject.ModifyType == ModifyType.Add)
                    {
                        GetParent<CombatEntity>().GetComponent<AttributeComponent>().GetNumeric(attributeType).RemoveFinalAddModifier(NumericModifier);
                    }
                    if (StatusConfigObject.ModifyType == ModifyType.PercentAdd)
                    {
                        GetParent<CombatEntity>().GetComponent<AttributeComponent>().GetNumeric(attributeType).RemoveFinalPctAddModifier(NumericModifier);
                    }
                }
            }
            //逻辑触发
            if (StatusConfigObject.EnabledLogicTrigger)
            {

            }

            NumericModifier = null;
            GetParent<CombatEntity>().OnStatusRemove(this);
            base.EndAbility();
        }

        //应用能力效果
        public override void ApplyAbilityEffectsTo(CombatEntity targetEntity)
        {
            base.ApplyAbilityEffectsTo(targetEntity);
        }
    }
}