using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat.Status
{
    /// <summary>
    /// 效果的间隔触发组件
    /// </summary>
    public class EffectIntervalTriggerComponent : Component
    {
        public GameTimer IntervalTimer { get; set; }


        public override void Setup()
        {
            var effectEntity = Entity as EffectEntity;
            var interval = effectEntity.Effect.Interval / 1000f;
            IntervalTimer = new GameTimer(interval);
        }

        public override void Update()
        {
            var effectEntity = Entity as EffectEntity;
            IntervalTimer.UpdateAsRepeat(Time.deltaTime, effectEntity.ApplyEffect);
        }

        //private void TriggerEffect()
        //{
        //    var effectEntity = Master as EffectEntity;
        //    //var statusEntity = effectEntity.Parent as StatusEntity;
        //    var combatEntity = effectEntity.Parent as CombatEntity;
        //    if (effectEntity.Effect is DamageEffect damageEffect)
        //    {
        //        var damageAction = CombatActionManager.CreateAction<DamageAction>(effectEntity.Caster);
        //        damageAction.DamageEffect = damageEffect;
        //        damageAction.Target = combatEntity; ;
        //        damageAction.DamageSource = DamageSource.Buff;
        //        damageAction.ApplyDamage();
        //    }
        //}
    }
}