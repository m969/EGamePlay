using EGamePlay;
using ET;
using System.Threading;
using UnityEngine;


public class EGamePlayInit : MonoBehaviour
{
    private void Awake()
    {
        EntityFactory.GlobalEntity = new GlobalEntity();
    }

    private void Start()
    {
        EntityFactory.CreateWithParent<ET.TimerComponent>(EntityFactory.GlobalEntity);
    }

    private void Update()
    {
        EntityFactory.GlobalEntity.Update();
        ET.TimerComponent.Instance.Update();
    }

    private void OnApplicationQuit()
    {
        Entity.Destroy(EntityFactory.GlobalEntity);
    }
}
