using EGamePlay.Combat.Ability;

namespace EGamePlay.Combat.Status
{
    public class StatusAbility : AbilityEntity
    {
        //投放者、施术者
        public CombatEntity Caster { get; set; }
        public StatusConfigObject StatusConfigObject { get; set; }
        public FloatModifier NumericModifier { get; set; }


        public override void Awake(object initData)
        {
            base.Awake(initData);
            StatusConfigObject = initData as StatusConfigObject;
        }

        //激活
        public override void ActivateAbility()
        {
            base.ActivateAbility();
            if (StatusConfigObject.EnabledStateModify)
            {

            }
            if (StatusConfigObject.EnabledAttributeModify)
            {
                switch (StatusConfigObject.AttributeType)
                {
                    case AttributeType.None:
                        break;
                    case AttributeType.AttackPower:
                        var value = int.Parse(StatusConfigObject.NumericValue);
                        NumericModifier = new FloatModifier() { Value = value };
                        GetParent<CombatEntity>().AttributeComponent.AttackPower.AddAddModifier(NumericModifier);
                        break;
                    case AttributeType.AttackDefense:
                        break;
                    case AttributeType.SpellPower:
                        break;
                    case AttributeType.MagicDefense:
                        break;
                    case AttributeType.CriticalProb:
                        break;
                    default:
                        break;
                }
            }
            if (StatusConfigObject.EnabledLogicTrigger)
            {
                foreach (var item in StatusConfigObject.Effects)
                {
                    var logicEntity = EntityFactory.CreateWithParent<LogicEntity>(this, item);
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
            if (StatusConfigObject.EnabledAttributeModify)
            {
                switch (StatusConfigObject.AttributeType)
                {
                    case AttributeType.None:
                        break;
                    case AttributeType.AttackPower:
                        GetParent<CombatEntity>().AttributeComponent.AttackPower.RemoveAddModifier(NumericModifier);
                        break;
                    case AttributeType.AttackDefense:
                        break;
                    case AttributeType.SpellPower:
                        break;
                    case AttributeType.MagicDefense:
                        break;
                    case AttributeType.CriticalProb:
                        break;
                    default:
                        break;
                }
            }
            NumericModifier = null;
            GetParent<CombatEntity>().OnStatusRemove(this);
            base.EndAbility();
        }

        //应用能力效果
        public override void ApplyAbilityEffect(CombatEntity targetEntity)
        {
            base.ApplyAbilityEffect(targetEntity);
        }
    }
}