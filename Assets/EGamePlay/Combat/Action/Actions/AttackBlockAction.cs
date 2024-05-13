using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;

namespace EGamePlay.Combat
{
    public class AttackBlockActionAbility : Entity, IActionAbility
    {
        public CombatEntity OwnerEntity { get { return GetParent<CombatEntity>(); } set { } }
        public bool Enable { get; set; }


        public override void Awake()
        {
            OwnerEntity.ListenActionPoint(ActionPointType.PreReceiveAttackEffect, TryBlock);
        }

        public bool TryMakeAction(out AttackBlockAction action)
        {
            if (Enable == false)
            {
                action = null;
            }
            else
            {
                action = OwnerEntity.AddChild<AttackBlockAction>();
                action.ActionAbility = this;
                action.Creator = OwnerEntity;
            }
            return Enable;
        }

        private bool IsAbilityEffectTrigger()
        {
            if (TryGet(out AbilityProbabilityTriggerComponent probabilityTriggerComponent))
            {
                var r = GameUtils.RandomHelper.RandomNumber(0, 10000);
                //Log.Debug($"IsAbilityEffectTrigger {r} < {probabilityTriggerComponent.Probability}");
                return r < probabilityTriggerComponent.Probability;
            }
            return false;
        }

        public void TryBlock(Entity action)
        {
            //Log.Debug($"TryBlock");
            if (IsAbilityEffectTrigger())
            {
                if (TryMakeAction(out var attackBlockAction))
                {
                    attackBlockAction.ActionAbility = this;
                    attackBlockAction.AttackExecution = action.As<AttackAction>().AttackExecution;
                    attackBlockAction.ApplyBlock();
                }
            }
        }
    }

    /// <summary>
    /// 格挡行动
    /// </summary>
    public class AttackBlockAction : Entity, IActionExecute
    {
        /// 行动能力
        public Entity ActionAbility { get; set; }
        /// 效果赋给行动源
        public EffectAssignAction SourceAssignAction { get; set; }
        /// 行动实体
        public CombatEntity Creator { get; set; }
        /// 目标对象
        public CombatEntity Target { get; set; }
        public AttackExecution AttackExecution { get; set; }


        public void FinishAction()
        {
            Entity.Destroy(this);
        }

        //前置处理
        private void PreProcess()
        {

        }

        public void ApplyBlock()
        {
            PreProcess();

            AttackExecution.SetBlocked();

            PostProcess();

            FinishAction();
        }

        //后置处理
        private void PostProcess()
        {
            //Creator.TriggerActionPoint(ActionPointType.PostGiveCure, this);
            //Target.TriggerActionPoint(ActionPointType.PostReceiveCure, this);
        }
    }
}