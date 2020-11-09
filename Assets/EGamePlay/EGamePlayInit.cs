using EGamePlay;
using EGamePlay.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EGamePlayInit : MonoBehaviour
{
    public static Entity GlobalEntity { get; set; }


    private void Start()
    {
    }

    private void Update()
    {
        foreach (var item in Entity.Entities)
        {
            foreach (var entity in item.Value)
            {
                foreach (var item2 in entity.Components)
                {
                    item2.Value.Update();
                }
            }
        }
    }
}
