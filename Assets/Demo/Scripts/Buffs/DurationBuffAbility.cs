using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat.Ability;
using EGamePlay;

public class DurationBuffAbility : AbilityEntity
{
    public override void ActivateAbility()
    {
        base.ActivateAbility();
        EndActivate();
    }

    public override void EndActivate()
    {
        base.EndActivate();
    }
}
