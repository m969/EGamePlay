using EGamePlay;
using EGamePlay.Combat;
using ET;
using System.Threading;
using Sirenix.OdinInspector;


#if UNITY
public class EGamePlayInit : SerializedMonoBehaviour
{
    public static EGamePlayInit Instance { get; private set; }
    public ReferenceCollector ConfigsCollector;
    public bool EntityLog;

#if !EGAMEPLAY_ET
    private void Awake()
    {
        Instance = this;
        SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
        Entity.EnableLog = EntityLog;
        var ecsNode = ECSNode.Create();
        ecsNode.AddChild<TimerManager>();
        ecsNode.AddChild<CombatContext>();
        ecsNode.AddComponent<ConfigManageComponent>(ConfigsCollector);
    }

    private void Update()
    {
        ThreadSynchronizationContext.Instance.Update();
        ECSNode.Instance.Update();
        TimerManager.Instance.Update();
    }

    private void OnApplicationQuit()
    {
        ECSNode.Destroy();
    }
#endif
}
#endif