using System.Collections.Generic;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 能力触发组件
    /// </summary>
    public class AbilityTriggerComponent : Component
	{
        public override bool DefaultEnable { get; set; } = false;
        public List<AbilityTrigger> AbilityTriggers { get; private set; } = new List<AbilityTrigger>();


        public override void Awake(object initData)
        {
            var triggerConfigs = initData as List<TriggerConfig>;
            foreach (var item in triggerConfigs)
            {
                var abilityTrigger = Entity.AddChild<AbilityTrigger>(item);
                AbilityTriggers.Add(abilityTrigger);
            }
        }

        public override void OnEnable()
        {
            foreach (var item in AbilityTriggers)
            {
                item.EnableTrigger();
            }
        }

        public override void OnDisable()
        {
            foreach (var item in AbilityTriggers)
            {
                item.DisableTrigger();
            }
        }
    }
}
