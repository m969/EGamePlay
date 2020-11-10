using EGamePlay;
using UnityEngine;


public class EGamePlayInit : MonoBehaviour
{
    private void Awake()
    {
        EntityFactory.GlobalEntity = new GlobalEntity();
    }

    private void Update()
    {
        EntityFactory.GlobalEntity.Update();
    }
}
