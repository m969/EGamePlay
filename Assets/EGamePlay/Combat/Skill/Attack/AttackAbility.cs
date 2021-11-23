using EGamePlay.Combat;
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
            var execution = OwnerEntity.AddChild<AttackExecution>(this);
            execution.AddComponent<UpdateComponent>();
            return execution;
        }
    }
}