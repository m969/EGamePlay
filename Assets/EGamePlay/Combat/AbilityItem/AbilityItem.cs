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
        public AbilityEntity AbilityEntity { get; set; }
        public AbilityEffectComponent AbilityEffectComponent => GetComponent<AbilityEffectComponent>();
        public Vector3 Position { get; set; }
        public float Direction { get; set; }
        public CombatEntity TargetEntity { get; set; }


        public override void Awake(object initData)
        {
            Name = (string)initData;
            var abilityEffectComponent = AddComponent<AbilityEffectComponent>();
        }

        ////尝试激活能力单元体
        //public void TryActivateAbilityItem()
        //{
        //    //Log.Debug($"{GetType().Name}->TryActivateAbility");
        //    ActivateAbilityItem();
        //}

        ////激活能力单元体
        //public void ActivateAbilityItem()
        //{

        //}

        ////禁用能力单元体
        //public void DeactivateAbilityItem()
        //{

        //}

        //结束能力单元体
        public void EndAbilityItem()
        {
            Destroy(this);
        }

        public void FillAbilityEffects(AbilityEntity abilityEntity)
        {
            AbilityEffectComponent.FillEffects(AbilityEntity.AbilityEffects);
        }

        public void OnCollision(CombatEntity otherCombatEntity)
        {
            if (TargetEntity != null)
            {
                if (otherCombatEntity != TargetEntity)
                {
                    return;
                }
                else
                {
                    AbilityEffectComponent.ApplyAllEffectsTo(otherCombatEntity);
                    EndAbilityItem();
                }
            }
            else
            {
                AbilityEffectComponent.ApplyAllEffectsTo(otherCombatEntity);
            }
        }
    }
}