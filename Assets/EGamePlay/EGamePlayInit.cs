using EGamePlay;
using EGamePlay.Combat;
using ET;
using System.Threading;
using UnityEngine;


public class EGamePlayInit : MonoBehaviour
{
    private void Awake()
    {
        EntityFactory.Global = new GlobalEntity();
        EntityFactory.CreateWithParent<TimerComponent>(EntityFactory.Global);
        EntityFactory.Create<CombatContext>();
    }

    private void Start()
    {
    }

    private void Update()
    {
        EntityFactory.Global.Update();
        TimerComponent.Instance.Update();
    }

    private void OnApplicationQuit()
    {
        Entity.Destroy(EntityFactory.Global);
    }
}
