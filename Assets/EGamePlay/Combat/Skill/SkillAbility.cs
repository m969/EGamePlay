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
        public CombatEntity OwnerEntity { get { return GetParent<CombatEntity>(); } set { } }
        public CombatEntity ParentEntity { get => GetParent<CombatEntity>(); }
        public bool Enable { get; set; }
        public SkillConfigObject SkillEffectsConfig { get; set; }
        public SkillConfig SkillConfig { get; set; }
        public bool Spelling { get; set; }
        public GameTimer CooldownTimer { get; } = new GameTimer(1f);
        private List<StatusAbility> ChildrenStatuses { get; set; } = new List<StatusAbility>();
        public ExecutionObject ExecutionObject { get; set; }


        public override void Awake(object initData)
        {
            base.Awake(initData);
            SkillEffectsConfig = initData as SkillConfigObject;

            SkillConfig = ConfigHelper.Get<SkillConfig>(SkillEffectsConfig.Id);

            if (SkillConfig.Type == "主动")
            {
                SkillEffectsConfig.SkillSpellType = SkillSpellType.Initiative;
            }
            else if (SkillConfig.Type == "被动")
            {
                SkillEffectsConfig.SkillSpellType = SkillSpellType.Passive;
            }
            else
            {
                Log.Error($"技能类型错误 {SkillConfig.Type}");
            }

            if (SkillConfig.TargetGroup == "己方")
            {
                SkillEffectsConfig.AffectTargetType = SkillAffectTargetType.SelfTeam;
            }
            else if (SkillConfig.TargetGroup == "敌方")
            {
                SkillEffectsConfig.AffectTargetType = SkillAffectTargetType.EnemyTeam;
            }
            else if (SkillConfig.TargetGroup == "自身")
            {
                SkillEffectsConfig.AffectTargetType = SkillAffectTargetType.Self;
            }
            else
            {
                Log.Error($"技能目标阵营错误 {SkillConfig.TargetGroup}");
            }

            if (SkillConfig.TargetSelect == "碰撞检测")
            {
                SkillEffectsConfig.TargetSelectType = SkillTargetSelectType.CollisionSelect;
            }
            else if (SkillConfig.TargetSelect == "条件指定")
            {
                SkillEffectsConfig.TargetSelectType = SkillTargetSelectType.ConditionSelect;
            }
            else if (SkillConfig.TargetSelect == "手动指定")
            {
                SkillEffectsConfig.TargetSelectType = SkillTargetSelectType.PlayerSelect;
            }
            else
            {
                Log.Error($"目标选取类型错误 {SkillConfig.TargetSelect}");
            }

            Name = this.SkillConfig.Name;
            AddComponent<AbilityEffectComponent>(SkillEffectsConfig.Effects);
            LoadExecution();
            //if (SkillEffectsConfig.SkillSpellType == SkillSpellType.Passive)
            {
                TryActivateAbility();
            }
        }

        public void LoadExecution()
        {
            ExecutionObject = AssetUtils.LoadObject<ExecutionObject>($"SkillConfigs/ExecutionConfigs/Execution_{SkillEffectsConfig.Id}");
            if (ExecutionObject == null)
            {
                return;
            }
        }

        public void TryActivateAbility()
        {
            this.ActivateAbility();
        }

        public void DeactivateAbility()
        {
            Enable = false;
            GetComponent<AbilityEffectComponent>().Enable = false;
        }

        public void ActivateAbility()
        {
            //base.ActivateAbility();
            FireEvent(nameof(ActivateAbility));
            //子状态效果
            if (SkillEffectsConfig.EnableChildrenStatuses)
            {
                foreach (var item in SkillEffectsConfig.ChildrenStatuses)
                {
                    var status = OwnerEntity.AttachStatus(item.StatusConfigObject);
                    status.OwnerEntity = OwnerEntity;
                    status.IsChildStatus = true;
                    status.ChildStatusData = item;
                    status.ProcessInputKVParams(item.Params);
                    status.TryActivateAbility();
                    ChildrenStatuses.Add(status);
                }
            }

            Enable = true;
            GetComponent<AbilityEffectComponent>().Enable = true;
        }

        public void EndAbility()
        {
            //子状态效果
            if (SkillEffectsConfig.EnableChildrenStatuses)
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
            execution.ExecutionObject = ExecutionObject;
            execution.LoadExecutionEffects();
            this.FireEvent(nameof(CreateExecution), execution);
            return execution;
        }
    }
}
#endif