using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;
using EGamePlay.Combat;
using ET;

namespace EGamePlay.Combat
{
    public class SpellActionAbility : Entity, IActionAbility
    {
        public CombatEntity OwnerEntity { get { return GetParent<CombatEntity>(); } set { } }
        public CombatEntity ParentEntity { get => GetParent<CombatEntity>(); }
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

        //public void TryActivateAbility() => ActivateAbility();

        //public void ActivateAbility() => Enable = true;

        //public void DeactivateAbility() { }

        //public void EndAbility() { }

        //public Entity CreateExecution()
        //{
        //    var execution = OwnerEntity.MakeAction<SpellAction>();
        //    execution.ActionAbility = this;
        //    return execution;
        //}

        //public bool TryMakeAction(out SpellAction abilityExecution)
        //{
        //    if (Enable == false)
        //    {
        //        abilityExecution = null;
        //    }
        //    else
        //    {
        //        abilityExecution = CreateExecution() as SpellAction;
        //    }
        //    return Enable;
        //}
    }

    /// <summary>
    /// 施法行动
    /// </summary>
    public class SpellAction : Entity, IActionExecution
    {
        public SkillAbility SkillAbility { get; set; }
        public SkillExecution SkillExecution { get; set; }
        public List<CombatEntity> SkillTargets { get; set; } = new List<CombatEntity>();
        public Vector3 InputPoint { get; set; }
        public float InputDirection { get; set; }

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
            Creator.TriggerActionPoint(ActionPointType.PreSpell, this);
        }

        public void SpellSkill()
        {
            PreProcess();
            if (SkillTargets.Count > 0)
            {
                SkillExecution.SkillTargets.AddRange(SkillTargets);
            }
            SkillExecution.Name = SkillAbility.Name;
            SkillExecution.BeginExecute();
            AddComponent<UpdateComponent>();
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
            Creator.TriggerActionPoint(ActionPointType.PostSpell, this);
        }
    }
}