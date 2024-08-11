using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay;

namespace EGamePlay.Combat
{
    public class CollisionActionAbility : Entity, IActionAbility
    {
        public CombatEntity OwnerEntity { get { return GetParent<CombatEntity>(); } set { } }
        public bool Enable { get; set; }


        public bool TryMakeAction(out CollisionAction action)
        {
            if (Enable == false)
            {
                action = null;
            }
            else
            {
                action = OwnerEntity.AddChild<CollisionAction>();
                action.ActionAbility = this;
                action.Creator = OwnerEntity;
            }
            return Enable;
        }
    }

    /// <summary>
    /// 碰撞行动
    /// </summary>
    public class CollisionAction : Entity, IActionExecute
    {
        /// 行动能力
        public Entity ActionAbility { get; set; }
        /// 效果赋给行动源
        public EffectAssignAction SourceAssignAction { get; set; }
        /// 行动实体
        public CombatEntity Creator { get; set; }
        /// 目标对象
        public Entity Target { get; set; }
        public AbilityItem CauseItem { get; set; }
        //public AbilityItem TargetItem { get; set; }


        public void FinishAction()
        {
            Entity.Destroy(this);
        }

        //前置处理
        private void PreProcess()
        {

        }

        public void ApplyCollision()
        {
            PreProcess();



            if (Target != null)
            {
                if (Target is CombatEntity combatEntity)
                {
                    CauseItem.OnTriggerEvent(combatEntity);
                }
                if (Target is AbilityItem abilityItem)
                {
                    var collisionComp = Target.GetComponent<AbilityItemCollisionExecuteComponent>();
                    var causeCollisionComp = CauseItem.GetComponent<AbilityItemCollisionExecuteComponent>();
                    var actionEvent = collisionComp.CollisionExecuteData.ActionData.ActionEventType;
                    if (Target.GetComponent<AbilityItemShieldComponent>() != null)
                    {
#if !EGAMEPLAY_ET
                        if (CauseItem.OwnerEntity.IsHero != abilityItem.OwnerEntity.IsHero)
#endif
                        {
                            CauseItem.OnTriggerEvent(Target);

                            if (causeCollisionComp.CollisionExecuteData.ActionData.FireType == FireType.CollisionTrigger)
                            {
#if EGAMEPLAY_ET
                                var itemUnit = CauseItem.GetComponent<CombatUnitComponent>().Unit as ItemUnit;
                                itemUnit.DestroyType = UnitDestroyType.DestroyWithExplosion;
#endif
                                CauseItem.GetComponent<HealthPointComponent>().SetDie();
                            }
                        }
                    }
                }
            }

            PostProcess();

            FinishAction();
        }

        //后置处理
        private void PostProcess()
        {

        }
    }
}