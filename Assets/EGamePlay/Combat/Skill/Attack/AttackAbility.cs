using EGamePlay.Combat.Ability;
using System;
using GameUtils;
using ET;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public class AttackAbility : AbilityEntity
    {
        public override AbilityExecution CreateExecution()
        {
            var execution = Entity.CreateWithParent<AttackExecution>(OwnerEntity, this);
            execution.AddComponent<UpdateComponent>();
            return execution;
        }
    }
}