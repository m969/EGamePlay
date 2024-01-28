using Sirenix.OdinInspector;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 行动点观察者，用于订阅战斗实体的行动点触发事件
    /// </summary>
    public class ActionPointObserver : Entity, ICombatObserver
    {
        public override void Awake()
        {
            var abilityEffect = GetParent<AbilityEffect>();
            var combatEntity = abilityEffect.OwnerAbility.As<IAbilityEntity>().ParentEntity;
            combatEntity.GetComponent<ActionPointComponent>().AddObserver(abilityEffect.EffectConfig.ActionPointType, this);
        }

        public override void OnDestroy()
        {
            var abilityEffect = GetParent<AbilityEffect>();
            var combatEntity = abilityEffect.OwnerAbility.As<IAbilityEntity>().ParentEntity;
            combatEntity.GetComponent<ActionPointComponent>().RemoveObserver(abilityEffect.EffectConfig.ActionPointType, this);
        }

        public void OnTrigger(Entity source)
        {
            //Log.Debug("ActionPointObserver OnTrigger");
            var abilityEffect = GetParent<AbilityEffect>();
            abilityEffect.OnObserverTrigger(new TriggerContext(this, source));
        }
    }
}