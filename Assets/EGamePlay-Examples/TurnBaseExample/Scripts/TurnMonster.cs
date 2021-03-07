using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EGamePlay.Combat;
using DG.Tweening;

public class TurnMonster : TurnCombatObject
{
    public void Setup(int seat)
    {
        CombatEntity = CombatContext.Instance.AddMonsterEntity(seat);
        base.Setup();
    }
}
