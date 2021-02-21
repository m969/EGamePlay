using EGamePlay;
using EGamePlay.Combat;
using ET;
using System.Threading;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;


public class EGamePlayInit : SerializedMonoBehaviour
{
    public static EGamePlayInit Instance { get; private set; }
    public ReferenceCollector ConfigsCollector;


    private void Awake()
    {
        Instance = this;
        SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
        MasterEntity.Create();
        Entity.Create<TimerComponent>();
        Entity.Create<CombatContext>();
        MasterEntity.Instance.AddComponent<ConfigManageComponent>(ConfigsCollector);
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
