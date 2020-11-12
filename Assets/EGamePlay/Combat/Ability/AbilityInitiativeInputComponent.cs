using EGamePlay.Combat.Status;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EGamePlay.Combat.Ability
{
    /// <summary>
    /// 技能的主动输入处理组件
    /// </summary>
    public class AbilityInitiativeInputComponent : Component
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
    }
}