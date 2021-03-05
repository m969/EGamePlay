using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay.Combat;

public class TurnMonster : MonoBehaviour
{
    public CombatEntity CombatEntity { get; set; }


    private void Start()
    {
        CombatEntity = TurnBaseCombatContext.Instance.AddMonsterEntity();

    }
}
