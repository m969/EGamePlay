using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtils;
using ECS;
using EGamePlay.Combat;
using System;
using ET;
using System.ComponentModel;
using ECSGame;
using static UnityEngine.GraphicsBuffer;

namespace EGamePlay
{
    public class AddBuffActionSystem : AEntitySystem<AddBuffAction>,
        IAwake<AddBuffAction>
    {
        public void Awake(AddBuffAction entity)
        {

        }

        public static void FinishAction(AddBuffAction entity)
        {
            EcsObject.Destroy(entity);
        }

        //前置处理
        private static bool ActionCheckProcess(AddBuffAction entity)
        {
            return true;
        }

        public static void Execute(AddBuffAction entity)
        {
            ActionSystem.ExecuteAction(entity, _ =>
            {
                return ActionCheckProcess(entity);
            }, _ =>
            {
                var buffObject = entity.AddStatusEffect.AddStatus;
                if (buffObject == null)
                {
                    var statusId = entity.AddStatusEffect.AddStatusId;
                    buffObject = AssetUtils.LoadObject<AbilityConfigObject>($"{AbilityManagerObject.BuffResFolder}/Buff_{statusId}");
                }
                var buffConfig = AbilityConfigCategory.Instance.Get(buffObject.Id);
                var canStack = buffConfig.CanStack == "是";
                if (canStack == false)
                {
                    if (BuffSystem.HasBuff(entity.Target as CombatEntity, buffConfig.KeyName))
                    {
                        var status = BuffSystem.GetBuff(entity.Target as CombatEntity, buffConfig.KeyName);
                        var lifeComp = status.GetComponent<AbilityDurationComponent>();
                        if (lifeComp != null)
                        {
                            var statusLifeTimer = lifeComp.LifeTimer;
                            statusLifeTimer.MaxTime = entity.AddStatusEffect.Duration;
                            statusLifeTimer.Reset();
                        }
                        FinishAction(entity);
                        return true;
                    }
                }

                entity.BuffAbility = BuffSystem.Attach(entity.Target as CombatEntity, buffObject);
                entity.BuffAbility.OwnerEntity = entity.Creator;
                entity.BuffAbility.GetComponent<AbilityLevelComponent>().Level = entity.SourceAbility.GetComponent<AbilityLevelComponent>().Level;
                ProcessInputKVParams(entity, entity.BuffAbility, entity.AddStatusEffect.Params);

                if (entity.AddStatusEffect.Duration > 0)
                {
                    entity.BuffAbility.AddComponent<AbilityDurationComponent>(x => x.Duration = entity.AddStatusEffect.Duration);
                }
                AbilitySystem.TryActivateAbility(entity.BuffAbility);
                return true;
            });

            FinishAction(entity);
        }

        /// 这里处理技能传入的参数数值替换
        public static void ProcessInputKVParams(AddBuffAction entity, Ability ability, Dictionary<string, string> Params)
        {
            foreach (var abilityTrigger in ability.AbilityTriggers)
            {
                var effect = abilityTrigger.TriggerConfig;

                if (!string.IsNullOrEmpty(effect.ConditionParam))
                {
                    abilityTrigger.ConditionParamValue = ProcessReplaceKV(entity, effect.ConditionParam, Params);
                }
            }

            foreach (var abilityEffect in ability.AbilityEffects)
            {
                var effect = abilityEffect.EffectConfig;

                if (effect is AttributeModifyEffect attributeModify && abilityEffect.GetComponent<EffectAttributeModifyComponent>() is { } attributeModifyComponent)
                {
                    attributeModifyComponent.ModifyValueFormula = ProcessReplaceKV(entity, attributeModify.NumericValue, Params);
                }
                if (effect is DamageEffect damage && abilityEffect.GetComponent<EffectDamageComponent>() is { } damageComponent)
                {
                    damageComponent.DamageValueFormula = ProcessReplaceKV(entity, damage.DamageValueFormula, Params);
                }
                if (effect is CureEffect cure && abilityEffect.GetComponent<EffectCureComponent>() is { } cureComponent)
                {
                    cureComponent.CureValueProperty = ProcessReplaceKV(entity, cure.CureValueFormula, Params);
                }
            }
        }

        private static string ProcessReplaceKV(AddBuffAction entity, string originValue, Dictionary<string, string> Params)
        {
            foreach (var aInputKVItem in Params)
            {
                if (!string.IsNullOrEmpty(originValue))
                {
                    originValue = originValue.Replace(aInputKVItem.Key, aInputKVItem.Value);
                }
            }
            return originValue;
        }
    }
}