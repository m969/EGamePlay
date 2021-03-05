using EGamePlay;
using EGamePlay.Combat;
using ET;
using System.Threading;
using Sirenix.OdinInspector;


public enum ExampleType { Rpg, TurnBase, SLG, Card }

public class EGamePlayInit : SerializedMonoBehaviour
{
    public static EGamePlayInit Instance { get; private set; }
    public ExampleType ExampleType;
    public ReferenceCollector ConfigsCollector;


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
            Entity.Create<TurnBaseCombatContext>();
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
