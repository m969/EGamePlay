using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using EGamePlay;
using EGamePlay.Combat;

namespace ECSGame
{
    public class Handler_AfterReceiveApplyCmd : ACommandHandler<AfterReceiveApplyCmd>
    {
        protected override async ET.ETTask Handle(EcsNode ecsNode, AfterReceiveApplyCmd cmd)
        {
            if (cmd.Action is DamageAction damageAction)
            {
                HealthViewSystem.OnReceiveDamage(cmd.Entity, damageAction);
            }
        }
    }
}
