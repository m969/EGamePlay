using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using EGamePlay.Combat.Status;
using EGamePlay.Combat.Ability;
using EGamePlay.Combat;
using EGamePlay;

public class Status_Scare : StatusAbility
{
    public override void ActivateAbility()
    {
        base.ActivateAbility();
        Coroutine();
    }

    //协程
    private async void Coroutine()
    {
        while (true)
        {
            if (IsDisposed)
            {
                break;
            }
            await ET.TimerComponent.Instance.WaitAsync(100);
        }
    }
}
