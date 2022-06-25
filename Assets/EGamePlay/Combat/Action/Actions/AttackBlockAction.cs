using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;

namespace EGamePlay.Combat
{
    public class AttackBlockActionAbility : Entity, IActionAbility
    {
        public CombatEntity OwnerEntity { get { return GetParent<CombatEntity>(); } set { } }
        public CombatEntity ParentEntity { get => GetParent<CombatEntity>(); }
        public bool Enable { get; set; }


        public override void Awake()
        {
            ParentEntity.AttackAbility.OnEvent(nameof(AttackExecution.TriggerAttackPreProcess), OnTriggerAttackPreProcess);
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

        //public void TryActivateAbility() => ActivateAbility();

        //public void ActivateAbility() => Enable = true;

        //public void DeactivateAbility() { }

        //public void EndAbility() { }

        //public Entity CreateExecution()
        //{
        //    var execution = OwnerEntity.MakeAction<AttackBlockAction>();
        //    execution.ActionAbility = this;
        //    return execution;
        //}

        //public bool TryMakeAction(out AttackBlockAction abilityExecution)
        //{
        //    if (Enable == false)
        //    {
        //        abilityExecution = null;
        //    }
        //    else
        //    {
        //        abilityExecution = CreateExecution() as AttackBlockAction;
        //    }
        //    return Enable;
        //}

        private void OnTriggerAttackPreProcess(Entity attackExecution)
        {
            TryBlock(attackExecution.As<AttackExecution>());
        }

        private bool IsAbilityEffectTrigger()
        {
            if (TryGet(out AbilityProbabilityTriggerComponent probabilityTriggerComponent))
            {
                var r = ET.RandomHelper.RandomNumber(0, 10000);
                return r < probabilityTriggerComponent.Probability;
            }
            return false;
        }

        private void TryBlock(AttackExecution attackExecution)
        {
            if (IsAbilityEffectTrigger())
            {
                attackExecution.SetBlocked();
            }
        }
    }

    /// <summary>
    /// 格挡行动
    /// </summary>
    public class AttackBlockAction : Entity, IActionExecution
    {
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

        public void ApplyBlock()
        {
            PreProcess();

            //var attackAction = SourceEffectAssignAction.TargetAction.As<AttackAction>();
            //attackAction.SetBlocked();

            PostProcess();
        }

        //后置处理
        private void PostProcess()
        {
            //Creator.TriggerActionPoint(ActionPointType.PostGiveCure, this);
            //Target.TriggerActionPoint(ActionPointType.PostReceiveCure, this);
        }
    }
}