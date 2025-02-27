using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using ET;

#if EGAMEPLAY_ET
using Unity.Mathematics;
using Vector3 = Unity.Mathematics.float3;
using Quaternion = Unity.Mathematics.quaternion;
using AO;
using AO.EventType;
using ET.EventType;
#else
using float3 = UnityEngine.Vector3;
#endif

namespace EGamePlay.Combat
{
    public class SpellActionAbility : Entity, IActionAbility
    {
        public CombatEntity OwnerEntity { get { return GetParent<CombatEntity>(); } set { } }
        public bool Enable { get; set; }


        public bool TryMakeAction(out SpellAction action)
        {
            if (Enable == false)
            {
                action = null;
            }
            else
            {
                action = OwnerEntity.AddChild<SpellAction>();
                action.ActionAbility = this;
                action.Creator = OwnerEntity;
            }
            return Enable;
        }
    }

    /// <summary>
    /// 施法行动
    /// </summary>
    public class SpellAction : Entity, IActionExecute
    {
        public Ability SkillAbility { get; set; }
        public AbilityExecution SkillExecution { get; set; }
        public List<CombatEntity> SkillTargets { get; set; } = new List<CombatEntity>();
        public CombatEntity InputTarget { get; set; }
        public Vector3 InputPoint { get; set; }
        public Vector3 InputDirection { get; set; }
        public float InputRadian { get; set; }
        public ETTask Task { get; set; }

        /// 行动能力
        public Entity ActionAbility { get; set; }
        /// 效果赋给行动源
        public EffectAssignAction SourceAssignAction { get; set; }
        /// 行动实体
        public CombatEntity Creator { get; set; }
        /// 目标对象
        public Entity Target { get; set; }


        public void FinishAction()
        {
            Entity.Destroy(this);
        }

        //前置处理
        private void PreProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.PreExecuteSpell, this);
        }

        public void SpellSkill(bool actionOccupy = true)
        {
            PreProcess();

            var execution = SkillAbility.OwnerEntity.AddChild<AbilityExecution>(SkillAbility);
            SkillExecution = execution;
#if EGAMEPLAY_ET && DEBUG
            SkillAbility.LoadExecution();
#endif
            execution.ExecutionObject = SkillAbility.ExecutionObject;
            execution.LoadExecutionEffects();
            execution.Position = SkillAbility.OwnerEntity.Position + SkillAbility.ExecutionObject.Offset;
            execution.Rotation = InputDirection.GetRotation();
#if EGAMEPLAY_ET
            execution.CreateItemUnit();
#endif

            execution.FireEvent("CreateExecution");
            execution.Name = SkillAbility.Name;
            if (SkillTargets.Count > 0)
            {
                SkillExecution.SkillTargets.AddRange(SkillTargets);
            }
            if (SkillAbility.Config.Id != 2001)
            {
                SkillExecution.ActionOccupy = actionOccupy;
            }
            execution.InputTarget = InputTarget;
            execution.InputPoint = InputPoint;
            execution.InputDirection = InputDirection;
            execution.InputRadian = InputRadian;
            execution.BeginExecute();
            AddComponent<UpdateComponent>();
            if (SkillAbility.Config.Id == 2001)
            {
                execution.GetParent<CombatEntity>().SpellingExecution = null;
            }
        }

        public override void Update()
        {
            if (SkillExecution != null)
            {
                if (SkillExecution.IsDisposed)
                {
                    PostProcess();
                    FinishAction();
                }
            }
        }

        //后置处理
        private void PostProcess()
        {
            Creator.TriggerActionPoint(ActionPointType.PostExecuteSpell, this);
        }
    }
}