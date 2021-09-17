using System;
using GameUtils;
using ET;
using System.Collections.Generic;
using UnityEngine;

#if !EGAMEPLAY_EXCEL
namespace EGamePlay.Combat
{
    public class SkillAbility : AbilityEntity
    {
        public SkillConfigObject SkillConfig { get; set; }
        public bool Spelling { get; set; }
        public GameTimer CooldownTimer { get; } = new GameTimer(1f);
        private List<StatusAbility> ChildrenStatuses { get; set; } = new List<StatusAbility>();


        public override void Awake(object initData)
        {
            base.Awake(initData);
            SkillConfig = initData as SkillConfigObject;
            if (SkillConfig.SkillSpellType == SkillSpellType.Passive)
            {
                TryActivateAbility();
            }
        }

        public override void ActivateAbility()
        {
            base.ActivateAbility();
            //子状态效果
            if (SkillConfig.EnableChildrenStatuses)
            {
                foreach (var item in SkillConfig.ChildrenStatuses)
                {
                    var status = OwnerEntity.AttachStatus<StatusAbility>(item.StatusConfigObject);
                    status.Caster = OwnerEntity;
                    status.IsChildStatus = true;
                    status.ChildStatusData = item;
                    status.TryActivateAbility();
                    ChildrenStatuses.Add(status);
                }
            }
        }

        public override void EndAbility()
        {
            base.EndAbility();
            //子状态效果
            if (SkillConfig.EnableChildrenStatuses)
            {
                foreach (var item in ChildrenStatuses)
                {
                    item.EndAbility();
                }
                ChildrenStatuses.Clear();
            }
        }

        public override AbilityExecution CreateExecution()
        {
            var execution = Entity.CreateWithParent<SkillExecution>(OwnerEntity, this);
            execution.AddComponent<UpdateComponent>();
            return execution;
        }

        public override void ApplyAbilityEffectsTo(CombatEntity targetEntity)
        {
            List<Effect> Effects = null;
            Effects = SkillConfig.Effects;
            if (Effects == null)
                return;
            foreach (var effectItem in Effects)
            {
                ApplyEffectTo(targetEntity, effectItem);
            }
        }
    }
}
#endif