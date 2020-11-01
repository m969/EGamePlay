using EGamePlay.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EGamePlayInit : MonoBehaviour
{
    private GlobalCombatManager GlobalCombatManager;


    private void Start()
    {
        GlobalCombatManager = new GlobalCombatManager();
    }
}
