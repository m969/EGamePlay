using EGamePlay;
using UnityEngine;


public class CombatCreateFlow : WorkFlow
{
    public GameObject CombatRootClone { get; private set; }


    public override void Awake()
    {
        var combatRoot = GameObject.Find("CombatRoot");
        CombatRootClone = GameObject.Instantiate(combatRoot);
        CombatRootClone.SetActive(false);
    }

    public override void Startup()
    {
        base.Startup();
        Log.Debug("CombatCreateFlow Startup");

        var combatRoot = GameObject.Find("CombatRoot");
        if (combatRoot == null)
        {
            combatRoot = GameObject.Instantiate(CombatRootClone);
            combatRoot.name = "CombatRoot";
            combatRoot.SetActive(true);
        }

        var heroRoot = GameObject.Find("CombatRoot/HeroRoot").transform;
        for (int i = 0; i < heroRoot.childCount; i++)
        {
            var hero = heroRoot.GetChild(i);
            var turnHero = hero.gameObject.AddComponent<TurnCombatObject>();
            turnHero.Setup(i);
            turnHero.CombatEntity.JumpToTime = FlowSource.GetParent<CombatFlow>().JumpToTime;
        }
        var monsterRoot = GameObject.Find("CombatRoot/MonsterRoot").transform;
        for (int i = 0; i < monsterRoot.childCount; i++)
        {
            var hero = monsterRoot.GetChild(i);
            var turnMonster = hero.gameObject.AddComponent<TurnCombatObject>();
            turnMonster.Setup(i);
            turnMonster.CombatEntity.JumpToTime = FlowSource.GetParent<CombatFlow>().JumpToTime;
        }

        Finish();
    }
}
