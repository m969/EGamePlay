using Sirenix.OdinInspector;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 行动点观察者，用于订阅战斗实体的行动点触发事件
    /// </summary>
    public class ActionPointObserveComponent : Component
    {
        public override void Awake()
        {
            var abilityEffect = GetEntity<AbilityTrigger>();
            var combatEntity = abilityEffect.OwnerAbility.ParentEntity;
            combatEntity.GetComponent<ActionPointComponent>().AddObserver(abilityEffect.TriggerConfig.ActionPointType, this);
        }

        public override void OnDestroy()
        {
            var abilityEffect = GetEntity<AbilityTrigger>();
            var combatEntity = abilityEffect.OwnerAbility.ParentEntity;
            combatEntity.GetComponent<ActionPointComponent>().RemoveObserver(abilityEffect.TriggerConfig.ActionPointType, this);
        }

        public void OnTrigger(Entity source)
        {
            var abilityEffect = GetEntity<AbilityTrigger>();
            abilityEffect.OnTrigger(new TriggerContext() { TriggerSource = source });
        }
    }
}