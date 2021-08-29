using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;

namespace EGamePlay.Combat.Ability
{
    /// <summary>
    /// 能力单元体
    /// </summary>
    public class AbilityItem : Entity, IPosition
    {
        public AbilityEntity AbilityEntity { get; set; }
        public Vector3 Position { get; set; }
        public float Direction { get; set; }
        public CombatEntity TargetEntity { get; set; }


        public override void Awake(object initData)
        {
            Name = (string)initData;
        }

        //尝试激活能力单元体
        public virtual void TryActivateAbilityItem()
        {
            //Log.Debug($"{GetType().Name}->TryActivateAbility");
            ActivateAbilityItem();
        }

        //激活能力单元体
        public virtual void ActivateAbilityItem()
        {

        }

        //禁用能力单元体
        public virtual void DeactivateAbilityItem()
        {

        }

        //结束能力单元体
        public virtual void EndAbilityItem()
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
                else
                {
                    AbilityEntity.ApplyAbilityEffectsTo(otherCombatEntity);
                    EndAbilityItem();
                }
            }
            else
            {
                AbilityEntity.ApplyAbilityEffectsTo(otherCombatEntity);
            }
        }
    }
}