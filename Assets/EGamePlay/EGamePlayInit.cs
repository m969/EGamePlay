using EGamePlay;
using EGamePlay.Combat;
using ET;
using System.Threading;
using UnityEngine;


public class EGamePlayInit : MonoBehaviour
{
    private void Awake()
    {
        MasterEntity.Create();
        Entity.Create<TimerComponent>();
        Entity.Create<CombatContext>();
    }

    private void Start()
    {
    }

    private void Update()
    {
        MasterEntity.Instance.Update();
        TimerComponent.Instance.Update();
    }

    private void OnApplicationQuit()
    {
        Entity.Destroy(MasterEntity.Instance);
        MasterEntity.Instance = null;
    }
}
