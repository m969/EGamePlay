using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 能力生命组件
    /// </summary>
    public class AbilityLifeTimeComponent : Component
    {
        public override bool DefaultEnable { get; set; } = true;
        public GameTimer LifeTimer { get; set; }
        public float Duration { get; set; }


        public override void Awake(object initData)
        {
            Duration = (float)initData;
            var lifeTime = Duration;
            LifeTimer = new GameTimer(lifeTime);
        }

        public override void Update()
        {
            if (LifeTimer.IsRunning)
            {
                LifeTimer.UpdateAsFinish(Time.deltaTime, OnLifeTimeFinish);
            }
        }

        private void OnLifeTimeFinish()
        {
            var ability = GetEntity<Ability>();
            var config = GetEntity<Ability>().Config;
            if (config.Type == "Skill")
            {
                ability.ParentEntity.GetComponent<SkillComponent>().RemoveSkill(ability);
            }
            if (config.Type == "Buff")
            {
                ability.ParentEntity.GetComponent<StatusComponent>().RemoveBuff(ability);
            }
            //GetEntity<Ability>().EndAbility();
        }
    }
}