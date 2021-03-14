using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using EGamePlay.Combat.Ability;
using ET;
using Log = EGamePlay.Log;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace EGamePlay.Combat
{
    /// <summary>
    /// 普攻执行体
    /// </summary>
    public class AttackExecution : AbilityExecution
    {
        public AttackAction AttackAction { get; set; }
        
        
        public override void Update()
        {

        }

        public override void BeginExecute()
        {
            var action = OwnerEntity.CreateAction<DamageAction>();
            action.Target = AttackAction.Target;
            action.DamageSource = DamageSource.Attack;
            action.ApplyDamage();

            this.EndExecute();
        }

        public override void EndExecute()
        {
            base.EndExecute();
            AttackAction = null;
        }
    }
}