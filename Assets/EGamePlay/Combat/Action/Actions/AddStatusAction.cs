using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using EGamePlay.Combat;

namespace EGamePlay.Combat
{
    public class AddStatusActionAbility : ActionAbility<AddStatusAction>
    {

    }

    /// <summary>
    /// 施加状态行动
    /// </summary>
    public class AddStatusAction : ActionExecution
    {
        public AbilityEntity SourceAbility { get; set; }
        public AddStatusEffect AddStatusEffect => AbilityEffect.EffectConfig as AddStatusEffect;
        public StatusAbility Status { get; set; }


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
            var canStack = statusConfig.CanStack;
            var enabledLogicTrigger = statusConfig.EnabledLogicTrigger;
#endif
            if (canStack == false)
            {
                if (Target.HasStatus(statusConfig.ID))
                {
                    var status = Target.GetStatus(statusConfig.ID);
                    var statusLifeTimer = status.GetComponent<StatusLifeTimeComponent>().LifeTimer;
                    statusLifeTimer.MaxTime = AddStatusEffect.Duration / 1000f;
                    statusLifeTimer.Reset();
                    return;
                }
            }

            Status = Target.AttachStatus<StatusAbility>(statusConfig);
            Status.OwnerEntity = Creator;
            Status.Level = SourceAbility.Level;
            Status.Duration = (int)AddStatusEffect.Duration;
            //Log.Debug($"ApplyEffectAssign AddStatusEffect {Status}");

            if (enabledLogicTrigger)
            {
                Status.ProccessInputKVParams(AddStatusEffect.Params);
            }

            Status.AddComponent<StatusLifeTimeComponent>();
            Status.TryActivateAbility();

            PostProcess();

            ApplyAction();
        }

        //后置处理
        private void PostProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.PostGiveStatus, this);
            Target.TriggerActionPoint(ActionPointType.PostReceiveStatus, this);
        }
    }
}