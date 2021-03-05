using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EGamePlay.Combat;

public class TurnHero : MonoBehaviour
{
    public CombatEntity CombatEntity { get; set; }


    private void Start()
    {
        CombatEntity = TurnBaseCombatContext.Instance.AddHeroEntity();
    }

    public void OnTurnAction()
    {

    }
}
