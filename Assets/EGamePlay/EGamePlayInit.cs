using EGamePlay;
using EGamePlay.Combat;
using ET;
using System.Threading;
using UnityEngine;


public class EGamePlayInit : MonoBehaviour
{
    private void Awake()
    {
        EntityFactory.Master = new MasterEntity();
        EntityFactory.Create<TimerComponent>();
        EntityFactory.Create<CombatContext>();
    }

    private void Start()
    {
    }

    private void Update()
    {
        EntityFactory.Master.Update();
        TimerComponent.Instance.Update();
    }

    private void OnApplicationQuit()
    {
        Entity.Destroy(EntityFactory.Master);
    }
}
