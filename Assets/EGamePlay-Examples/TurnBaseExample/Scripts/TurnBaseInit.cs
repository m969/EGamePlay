using EGamePlay;
using EGamePlay.Combat;
using ET;
using System.Threading;
using Sirenix.OdinInspector;
using UnityEngine;


public class TurnBaseInit : SerializedMonoBehaviour
{
    public static TurnBaseInit Instance { get; private set; }
    public int JumpToTime;


    private void Start()
    {
        Instance = this;

        var heroRoot = GameObject.Find("CombatRoot/HeroRoot").transform;
        for (int i = 0; i < heroRoot.childCount; i++)
        {
            var hero = heroRoot.GetChild(i);
            var turnHero = hero.gameObject.AddComponent<TurnCombatObject>();
            turnHero.Setup(i);
            turnHero.CombatEntity.JumpToTime = JumpToTime;
        }
        var monsterRoot = GameObject.Find("CombatRoot/MonsterRoot").transform;
        for (int i = 0; i < monsterRoot.childCount; i++)
        {
            var hero = monsterRoot.GetChild(i);
            var turnMonster = hero.gameObject.AddComponent<TurnCombatObject>();
            turnMonster.Setup(i);
            turnMonster.CombatEntity.JumpToTime = JumpToTime;
        }
    }
}
