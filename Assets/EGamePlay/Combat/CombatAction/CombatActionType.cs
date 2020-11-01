using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat
{
    public enum CombatActionType
    {
        CauseDamage,
        ReceiveDamage,
        GiveCure,
        ReceiveCure,
        AssignEffect,
        ReceiveEffect,

        Max,
    }
}