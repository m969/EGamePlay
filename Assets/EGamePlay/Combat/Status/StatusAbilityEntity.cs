using EGamePlay.Combat.Ability;

namespace EGamePlay.Combat.Status
{
    public partial class StatusAbilityEntity : AbilityEntity
    {
        public CombatEntity Caster { get; set; }
        public StatusConfigObject StatusConfigObject { get; set; }
        public FloatModifier NumericModifier { get; set; }


        public override void Awake(object initData)
        {
            base.Awake(initData);
            StatusConfigObject = initData as StatusConfigObject;
        }

        public override void ActivateAbility()
        {
            base.ActivateAbility();
            if (StatusConfigObject.EnabledStateModify)
            {

            }
            if (StatusConfigObject.EnabledNumericModify)
            {
                switch (StatusConfigObject.NumericType)
                {
                    case NumericType.None:
                        break;
                    case NumericType.PhysicAttack:
                        var value = int.Parse(StatusConfigObject.NumericValue);
                        NumericModifier = new FloatModifier() { Value = value };
                        GetParent<CombatEntity>().AttributeComponent.AttackPower.AddAddModifier(NumericModifier);
                        break;
                    case NumericType.Defense:
                        break;
                    case NumericType.SpellPower:
                        break;
                    case NumericType.MagicDefense:
                        break;
                    case NumericType.CriticalProb:
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

        public override void EndActivate()
        {
            if (StatusConfigObject.EnabledNumericModify)
            {
                switch (StatusConfigObject.NumericType)
                {
                    case NumericType.None:
                        break;
                    case NumericType.PhysicAttack:
                        GetParent<CombatEntity>().AttributeComponent.AttackPower.RemoveAddModifier(NumericModifier);
                        break;
                    case NumericType.Defense:
                        break;
                    case NumericType.SpellPower:
                        break;
                    case NumericType.MagicDefense:
                        break;
                    case NumericType.CriticalProb:
                        break;
                    default:
                        break;
                }
            }
            NumericModifier = null;
            GetParent<CombatEntity>().OnStatusRemove(this);
            base.EndActivate();
        }
    }
}