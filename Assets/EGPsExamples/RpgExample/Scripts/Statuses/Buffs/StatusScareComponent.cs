using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat;
using EGamePlay;

public class StatusScareComponent : EGamePlay.Component
{
    public override void Awake()
    {
        Entity.OnEvent(nameof(IAbilityEntity.ActivateAbility), Coroutine);
    }

    //协程
    private async void Coroutine(Entity statusAbility)
    {
        while (true)
        {
            if (IsDisposed)
            {
                break;
            }
            await ET.TimeHelper.WaitAsync(100);
        }
    }
}
