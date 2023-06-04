using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;
using GameUtils;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 能力单元体
    /// </summary>
    public class AbilityItem : Entity, IPosition
    {
        public Entity AbilityEntity => AbilityExecution.AbilityEntity;
        public IAbilityExecution AbilityExecution { get; set; }
        //public ExecutionEffectComponent ItemExecutionEffectComponent { get; private set; }
        public EffectApplyType EffectApplyType { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public CombatEntity TargetEntity { get; set; }


        public override void Awake(object initData)
        {
            AbilityExecution = initData as IAbilityExecution;
            if (AbilityEntity == null)
            {
                return;
            }
            var abilityEffects = AbilityEntity.GetComponent<AbilityEffectComponent>().AbilityEffects;
            foreach (var abilityEffect in abilityEffects)
            {
                if (abilityEffect.EffectConfig.Decorators != null)
                {
                    foreach (var effectDecorator in abilityEffect.EffectConfig.Decorators)
                    {
                        if (effectDecorator is DamageReduceWithTargetCountDecorator reduceWithTargetCountDecorator)
                        {
                            AddComponent<AbilityItemTargetCounterComponent>();
                        }
                    }
                }
            }
        }

        /// 结束单元体
        public void DestroyItem()
        {
            Log.Debug("AbilityItem DestroyItem");
            Destroy(this);
        }

        public override void OnDestroy()
        {
            //Log.Debug("AbilityItem OnDestroy");
        }

        public void OnCollision(CombatEntity otherCombatEntity)
        {
            if (TargetEntity != null)
            {
                if (otherCombatEntity != TargetEntity)
                {
                    return;
                }
            }

            var collisionExecuteData = GetComponent<AbilityItemCollisionExecuteComponent>().CollisionExecuteData;

            if (AbilityEntity != null)
            {
                //Log.Debug($"AbilityItem OnCollision {collisionExecuteData.ActionData.ActionEventType}");
                if (collisionExecuteData.ActionData.ActionEventType == FireEventType.AssignEffect)
                {
                    if (EffectApplyType == EffectApplyType.AllEffects)
                    {
                        //AbilityEntity.GetComponent<AbilityEffectComponent>().TryAssignAllEffectsToTargetWithAbilityItem(otherCombatEntity, this);
                        var effectAssigns = AbilityEntity.GetComponent<AbilityEffectComponent>().CreateAssignActions(otherCombatEntity);
                        foreach (var item in effectAssigns)
                        {
                            item.AbilityItem = this;
                            item.AssignEffect();
                        }
                    }
                    else
                    {
                        //AbilityEntity.GetComponent<AbilityEffectComponent>().TryAssignEffectByIndex(otherCombatEntity, (int)EffectApplyType - 1);
                        var effectAssign = AbilityEntity.GetComponent<AbilityEffectComponent>().CreateAssignActionByIndex(otherCombatEntity, (int)EffectApplyType - 1);
                        effectAssign.AbilityItem = this;
                        effectAssign.AssignEffect();
                    }
                }
            }

            if (AbilityExecution != null)
            {
                if (collisionExecuteData.ActionData.ActionEventType == FireEventType.TriggerNewExecution)
                {
                    OnTriggerNewExecution(collisionExecuteData.ActionData);
                }
            }

            if (TryGet(out AbilityItemTargetCounterComponent targetCounterComponent))
            {
                targetCounterComponent.TargetCounter++;
            }

            if (TargetEntity != null)
            {
                DestroyItem();
            }
        }

        public void OnTriggerNewExecution(ActionEventData ActionEventData)
        {
            Log.Debug($"AbilityItem OnTriggerNewExecution");
            var executionObject = AssetUtils.Load<ExecutionObject>(ActionEventData.NewExecution);
            if (executionObject == null)
            {
                Log.Error($"Can not find {ActionEventData.NewExecution}");
                return;
            }
            var sourceExecution = AbilityExecution as SkillExecution;
            var execution = sourceExecution.OwnerEntity.AddChild<SkillExecution>(sourceExecution.SkillAbility);
            execution.ExecutionObject = executionObject;
            //execution.InputTarget = sourceExecution.InputTarget;
            execution.InputPoint = Position;
            execution.LoadExecutionEffects();
            execution.BeginExecute();
            if (executionObject != null)
            {
                execution.AddComponent<UpdateComponent>();
            }
        }
    }
}