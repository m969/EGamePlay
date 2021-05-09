using EGamePlay.Combat.Ability;
using System;
using GameUtils;
using ET;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public class AttackAbility : AbilityEntity<AttackExecution>
    {
        public override AttackExecution CreateExecution()
        {
            var execution = Entity.CreateWithParent<AttackExecution>(OwnerEntity, this);
            execution.AddComponent<UpdateComponent>();
            return execution;
        }
    }
}