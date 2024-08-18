using System;
using GameUtils;
using ET;
using System.Collections.Generic;
using UnityEngine;
#if EGAMEPLAY_ET
using SkillConfig = cfg.Skill.SkillCfg;
using AO;
#endif

//#if !EGAMEPLAY_EXCEL
namespace EGamePlay.Combat
{
    public partial class Ability : Entity
    {
        public CombatEntity OwnerEntity { get { return GetParent<CombatEntity>(); } set { } }
        public Entity ParentEntity { get => Parent; }
        public bool Enable { get; set; }
        public AbilityConfig Config { get; set; }
        public AbilityConfigObject ConfigObject { get; set; }
        public bool Spelling { get; set; }
        public GameTimer CooldownTimer { get; } = new GameTimer(1f);
        public ExecutionObject ExecutionObject { get; set; }
        public bool IsBuff => Config.Type == "Buff";
        public bool IsSkill => !IsBuff;


        public override void Awake(object initData)
        {
            base.Awake(initData);
            ConfigObject = initData as AbilityConfigObject;
            Config = ConfigHelper.Get<AbilityConfig>(ConfigObject.Id);

            if (Config.TargetGroup == "己方")
            {
                ConfigObject.AffectTargetType = SkillAffectTargetType.SelfTeam;
            }
            else if (Config.TargetGroup == "敌方")
            {
                ConfigObject.AffectTargetType = SkillAffectTargetType.EnemyTeam;
            }
            else if (Config.TargetGroup == "自身")
            {
                ConfigObject.AffectTargetType = SkillAffectTargetType.Self;
            }
            else
            {
                Log.Error($"技能目标阵营错误 {Config.TargetGroup}");
            }

            if (IsSkill)
            {
                if (Config.TargetSelect == "碰撞检测")
                {
                    ConfigObject.TargetSelectType = SkillTargetSelectType.CollisionSelect;
                }
                else if (Config.TargetSelect == "条件指定")
                {
                    ConfigObject.TargetSelectType = SkillTargetSelectType.ConditionSelect;
                }
                else if (Config.TargetSelect == "手动指定")
                {
                    ConfigObject.TargetSelectType = SkillTargetSelectType.PlayerSelect;
                }
                else
                {
                    Log.Error($"目标选取类型错误 {Config.TargetSelect}");
                }
            }

            Name = this.Config.Name;
            AddComponent<AbilityEffectComponent>(ConfigObject.Effects);
            AddComponent<AbilityTriggerComponent>(ConfigObject.TriggerActions);
            LoadExecution();
            //TryActivateAbility();
        }

        public void LoadExecution()
        {
            ExecutionObject = AssetUtils.LoadObject<ExecutionObject>($"{AbilityManagerObject.ExecutionResFolder}/Execution_{ConfigObject.Id}");
            if (ExecutionObject == null)
            {
                return;
            }
        }

        public void TryActivateAbility()
        {
            this.ActivateAbility();
        }

        public void ActivateAbility()
        {
            Enable = true;
            GetComponent<AbilityEffectComponent>().Enable = true;
            GetComponent<AbilityTriggerComponent>().Enable = true;
        }

        public void DeactivateAbility()
        {
            Enable = false;
            GetComponent<AbilityEffectComponent>().Enable = false;
            GetComponent<AbilityTriggerComponent>().Enable = false;
        }

        public void EndAbility()
        {
            //ParentEntity.GetComponent<StatusComponent>().OnStatusRemove(this);
            DeactivateAbility();
            Entity.Destroy(this);
        }
    }
}
//#endif