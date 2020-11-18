using EGamePlay.Combat.Ability;

namespace EGamePlay.Combat.Status
{
    public partial class StatusAbilityEntity : AbilityEntity
    {
        public CombatEntity Caster { get; set; }
        public StatusConfigObject StatusConfigObject { get; set; }


        public override void Awake(object paramObject)
        {
            base.Awake(paramObject);

            StatusConfigObject = paramObject as StatusConfigObject;
        }

        public override void ActivateAbility()
        {
            base.ActivateAbility();
            foreach (var item in StatusConfigObject.Effects)
            {
                var effectEntity = EntityFactory.CreateWithParent<EffectEntity>(Caster, item);
                if (item is DamageEffect damageEffect)
                {

                }
                if (item is AddStatusEffect addStatusEffect)
                {

                }

                if (item.EffectTriggerType == EffectTriggerType.Interval)
                {
                    effectEntity.AddComponent<EffectIntervalTriggerComponent>();
                }
            }
        }
    }
}