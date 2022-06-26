using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using EGamePlay.Combat;

namespace EGamePlay.Combat
{
    public class EffectAssignAbility : Entity, IActionAbility
    {
        public CombatEntity OwnerEntity { get { return GetParent<CombatEntity>(); } set { } }
        public bool Enable { get; set; }


        public bool TryMakeAction(out EffectAssignAction action)
        {
            if (Enable == false)
            {
                action = null;
            }
            else
            {
                action = OwnerEntity.AddChild<EffectAssignAction>();
                action.ActionAbility = this;
                action.Creator = OwnerEntity;
            }
            return Enable;
        }
    }

    /// <summary>
    /// 赋给效果行动
    /// </summary>
    public class EffectAssignAction : Entity, IActionExecution
    {
        /// 创建这个效果赋给行动的源能力
        public Entity SourceAbility { get; set; }
        /// 目标行动
        public IActionExecution TargetAction { get; set; }
        public AbilityEffect AbilityEffect { get; set; }
        public AbilityItem AbilityItem { get; set; }
        public Effect EffectConfig => AbilityEffect.EffectConfig;
        /// 行动能力
        public Entity ActionAbility { get; set; }
        /// 效果赋给行动源
        public EffectAssignAction SourceAssignAction { get; set; }
        /// 行动实体
        public CombatEntity Creator { get; set; }
        /// 目标对象
        public CombatEntity Target { get; set; }


        /// 前置处理
        private void PreProcess()
        {

        }

        public void ApplyEffectAssign()
        {
            //Log.Debug($"ApplyEffectAssign {EffectConfig}");
            PreProcess();

            AbilityEffect.StartAssignEffect(this);

            PostProcess();

            FinishAction();
        }

        public void FillDatasToAction(IActionExecution action)
        {
            action.SourceAssignAction = this;
            action.Target = Target;
        }

        /// 后置处理
        private void PostProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.AssignEffect, this);
            Target.TriggerActionPoint(ActionPointType.ReceiveEffect, this);
        }

        public void FinishAction()
        {
            Entity.Destroy(this);
        }
    }
}