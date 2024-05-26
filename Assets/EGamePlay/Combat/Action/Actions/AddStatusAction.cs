using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using EGamePlay.Combat;
using ET;
using GameUtils;

namespace EGamePlay.Combat
{
    public class AddStatusActionAbility : Entity, IActionAbility
    {
        public CombatEntity OwnerEntity { get { return GetParent<CombatEntity>(); } set { } }
        public bool Enable { get; set; }


        public bool TryMakeAction(out AddStatusAction action)
        {
            if (Enable == false)
            {
                action = null;
            }
            else
            {
                action = OwnerEntity.AddChild<AddStatusAction>();
                action.ActionAbility = this;
                action.Creator = OwnerEntity;
            }
            return Enable;
        }
    }

    /// <summary>
    /// 施加状态行动
    /// </summary>
    public class AddStatusAction : Entity, IActionExecute
    {
        public Entity SourceAbility { get; set; }
        public AddStatusEffect AddStatusEffect => SourceAssignAction.AbilityEffect.EffectConfig as AddStatusEffect;
        public StatusAbility Status { get; set; }

        /// 行动能力
        public Entity ActionAbility { get; set; }
        /// 效果赋给行动源
        public EffectAssignAction SourceAssignAction { get; set; }
        /// 行动实体
        public CombatEntity Creator { get; set; }
        /// 目标对象
        public CombatEntity Target { get; set; }


        public void FinishAction()
        {
            Entity.Destroy(this);
        }

        //前置处理
        private void PreProcess()
        {

        }

        public void ApplyAddStatus()
        {
            PreProcess();

#if EGAMEPLAY_EXCEL
            var statusConfig = AddStatusEffect.AddStatusConfig;
            var canStack = statusConfig.CanStack == "是";
            var enabledLogicTrigger = statusConfig.EnabledLogicTrigger();
#else
            var statusConfig = AddStatusEffect.AddStatus;
            if (statusConfig == null)
            {
                var statusId = AddStatusEffect.AddStatusId;
                statusConfig = AssetUtils.LoadObject<StatusConfigObject>($"StatusConfigs/Status_{statusId}");
            }
            var config = StatusConfigCategory.Instance.GetWithIDType(statusConfig.ID);
            var canStack = config.CanStack == "是";
            //var enabledLogicTrigger = statusConfig.EnabledLogicTrigger;
#endif
            if (canStack == false)
            {
                if (Target.HasStatus(statusConfig.ID))
                {
                    var status = Target.GetStatus(statusConfig.ID);
                    var statusLifeTimer = status.GetComponent<StatusLifeTimeComponent>().LifeTimer;
                    statusLifeTimer.MaxTime = AddStatusEffect.Duration / 1000f;
                    statusLifeTimer.Reset();
                    FinishAction();
                    return;
                }
            }

            Status = Target.AttachStatus(statusConfig);
            Status.OwnerEntity = Creator;
            Status.GetComponent<AbilityLevelComponent>().Level = SourceAbility.GetComponent<AbilityLevelComponent>().Level;
            Status.Duration = (int)AddStatusEffect.Duration;
            //Log.Debug($"AddStatusAction ApplyAddStatus {statusConfig.Name}");

            Status.ProcessInputKVParams(AddStatusEffect.Params);

            Status.AddComponent<StatusLifeTimeComponent>();
            Status.TryActivateAbility();

            PostProcess();

            FinishAction();
        }

        //后置处理
        private void PostProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.PostGiveStatus, this);
            Target.TriggerActionPoint(ActionPointType.PostReceiveStatus, this);
        }
    }
}