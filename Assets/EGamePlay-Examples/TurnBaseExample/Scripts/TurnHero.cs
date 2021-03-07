using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay.Combat;

public class TurnHero : TurnCombatObject
{
    public void Setup(int seat)
    {
        CombatEntity = CombatContext.Instance.AddHeroEntity(seat);
        base.Setup();
    }
}
