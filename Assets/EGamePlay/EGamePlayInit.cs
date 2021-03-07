using EGamePlay;
using EGamePlay.Combat;
using ET;
using System.Threading;
using Sirenix.OdinInspector;
using UnityEngine;


public enum ExampleType { Rpg, TurnBase, SLG, Card }

public class EGamePlayInit : SerializedMonoBehaviour
{
    public static EGamePlayInit Instance { get; private set; }
    public ReferenceCollector ConfigsCollector;
    public ExampleType ExampleType;
    public int JumpToTime;


    private void Awake()
    {
        Instance = this;
        SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
        MasterEntity.Create();
        Entity.Create<TimerComponent>();
        MasterEntity.Instance.AddComponent<ConfigManageComponent>(ConfigsCollector);
        if (ExampleType == ExampleType.Rpg)
        {
            Entity.Create<CombatContext>();
        }
        if (ExampleType == ExampleType.TurnBase)
        {
            Entity.Create<CombatContext>();
            var heroRoot = GameObject.Find("CombatRoot/HeroRoot").transform;
            for (int i = 0; i < heroRoot.childCount; i++)
            {
                var hero = heroRoot.GetChild(i);
                var turnHero = hero.gameObject.AddComponent<TurnHero>();
                turnHero.Setup(i);
                turnHero.CombatEntity.JumpToTime = JumpToTime;
            }
            var monsterRoot = GameObject.Find("CombatRoot/MonsterRoot").transform;
            for (int i = 0; i < monsterRoot.childCount; i++)
            {
                var hero = monsterRoot.GetChild(i);
                var turnMonster = hero.gameObject.AddComponent<TurnMonster>();
                turnMonster.Setup(i);
                turnMonster.CombatEntity.JumpToTime = JumpToTime;
            }
        }
    }

    private void Start()
    {
    }

    private void Update()
    {
        ThreadSynchronizationContext.Instance.Update();
        MasterEntity.Instance.Update();
        TimerComponent.Instance.Update();
    }

    private void OnApplicationQuit()
    {
        Entity.Destroy(MasterEntity.Instance);
        MasterEntity.Destroy();
    }
}
