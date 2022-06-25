using System;
using GameUtils;
using ET;
using System.Collections.Generic;
using UnityEngine;

#if !EGAMEPLAY_EXCEL
namespace EGamePlay.Combat
{
    public partial class SkillAbility : Entity, IAbilityEntity
    {
        public SkillConfigObject SkillConfig { get; set; }
        public bool Spelling { get; set; }
        public GameTimer CooldownTimer { get; } = new GameTimer(1f);
        private List<StatusAbility> ChildrenStatuses { get; set; } = new List<StatusAbility>();
        public CombatEntity OwnerEntity { get; set; }
        public CombatEntity ParentEntity { get => GetParent<CombatEntity>(); }
        public bool Enable { get; set; }
        //public Func<SkillExecution> Enable { get; set; }


        public override void Awake(object initData)
        {
            base.Awake(initData);
            SkillConfig = initData as SkillConfigObject;
            Name = SkillConfig.Name;
            AddComponent<AbilityEffectComponent>(SkillConfig.Effects);
#if !SERVER
            ParseAbilityEffects();
#endif
            if (SkillConfig.SkillSpellType == SkillSpellType.Passive)
            {
                TryActivateAbility();
            }
        }

        public void ActivateAbility()
        {
            //base.ActivateAbility();
            FireEvent(nameof(ActivateAbility));
            //子状态效果
            if (SkillConfig.EnableChildrenStatuses)
            {
                foreach (var item in SkillConfig.ChildrenStatuses)
                {
                    var status = OwnerEntity.AttachStatus<StatusAbility>(item.StatusConfigObject);
                    status.OwnerEntity = OwnerEntity;
                    status.IsChildStatus = true;
                    status.ChildStatusData = item;
                    status.ProccessInputKVParams(item.Params);
                    status.TryActivateAbility();
                    ChildrenStatuses.Add(status);
                }
            }
        }

        public void EndAbility()
        {
            //base.EndAbility();
            //子状态效果
            if (SkillConfig.EnableChildrenStatuses)
            {
                foreach (var item in ChildrenStatuses)
                {
                    item.EndAbility();
                }
                ChildrenStatuses.Clear();
            }
            Entity.Destroy(this);
        }

        public Entity CreateExecution()
        {
            var execution = OwnerEntity.AddChild<SkillExecution>(this);
            this.FireEvent(nameof(CreateExecution), execution);
            execution.AddComponent<UpdateComponent>();
            return execution;
        }

        public void TryActivateAbility()
        {
            this.ActivateAbility();
        }

        public void DeactivateAbility()
        {
            Enable = false;
        }
    }
}
#endif