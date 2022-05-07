using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 能力单元体
    /// </summary>
    public class AbilityItem : Entity, IPosition
    {
        public AbilityEntity AbilityEntity => AbilityExecution.AbilityEntity;
        public AbilityExecution AbilityExecution { get; set; }
        public ExecutionEffectComponent ItemExecutionEffectComponent { get; private set; }
        public EffectApplyType EffectApplyType { get; private set; }
        public Vector3 Position { get; set; }
        public float Direction { get; set; }
        public CombatEntity TargetEntity { get; set; }


        public override void Awake(object initData)
        {
            AbilityExecution = initData as AbilityExecution;
            //ItemExecutionEffectComponent = AddComponent<ExecutionEffectComponent>();
            //var abilityEffects = AbilityEntity.AbilityEffects;
            //foreach (var abilityEffect in abilityEffects)
            //{
            //    if (abilityEffect.GetComponent<EffectExecutionSpawnItemComponent>() != null)
            //    {
            //        continue;
            //    }
            //    if (abilityEffect.GetComponent<EffectExecutionAnimationComponent>() != null)
            //    {
            //        continue;
            //    }

            //    var executionEffect = AddChild<ExecutionEffect>(abilityEffect);
            //    ItemExecutionEffectComponent.AddEffect(executionEffect);

            //    if (abilityEffect.EffectConfig is DamageEffect)
            //    {
            //        ItemExecutionEffectComponent.DamageExecutionEffect = executionEffect;
            //    }
            //    if (abilityEffect.EffectConfig is CureEffect)
            //    {
            //        ItemExecutionEffectComponent.CureExecutionEffect = executionEffect;
            //    }
            //}
        }

        //结束单元体
        public void DestroyItem()
        {
            Destroy(this);
        }

        public void OnCollision(CombatEntity otherCombatEntity)
        {
            if (TargetEntity != null)
            {
                if (otherCombatEntity != TargetEntity)
                {
                    return;
                }
                //ItemExecutionEffectComponent.ApplyAllEffectsTo(TargetEntity);
            }

            if (EffectApplyType == EffectApplyType.AllEffects)
            {
                AbilityEntity.AbilityEffectComponent.ApplyAllEffectsTo(otherCombatEntity);
            }
            else
            {
                AbilityEntity.AbilityEffectComponent.ApplyEffectByIndex(otherCombatEntity, (int)EffectApplyType - 1);
            }

            //var abilityEffects = AbilityEntity.AbilityEffects;
            //foreach (var abilityEffect in abilityEffects)
            //{
            //    abilityEffect.ApplyEffectTo(otherCombatEntity);
            //}

            if (TargetEntity != null)
            {
                DestroyItem();
            }
        }
    }
}