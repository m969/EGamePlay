using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat.Status
{
    /// <summary>
    /// 状态的间隔触发组件
    /// </summary>
    public class StatusIntervalTriggerComponent : Component
    {
        public Effect[] Effects { get; set; }
        public GameTimer IntervalTimer { get; set; }


        public override void Setup()
        {
            //var status = Master as StatusEntity;
            //Effects = status.StatusConfigObject.Effects.Where(x => x.EffectTriggerType == EffectTriggerType.Interval).ToArray();
            //var interval = Effects[0].Interval / 1000f;
            //IntervalTimer = new GameTimer(interval);
        }

        public override void Update()
        {
            //IntervalTimer.UpdateAsRepeat(Time.deltaTime, TriggerEffect);
        }

        private void TriggerEffect()
        {
            var status = Entity as StatusAbilityEntity;
            var combatEntity = status.Parent as CombatEntity;
            if (Effects[0] is DamageEffect damageEffect)
            {
                var damageAction = CombatActionManager.CreateAction<DamageAction>(status.Caster);
                damageAction.DamageEffect = damageEffect;
                damageAction.Target = combatEntity; ;
                damageAction.DamageSource = DamageSource.Buff;
                damageAction.ApplyDamage();
            }
        }
    }
}