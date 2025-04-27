using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using EGamePlay;
using EGamePlay.Combat;

namespace ECSGame
{
    public class Handler_TriggerClipCmd : ACommandHandler<TriggerClipCmd>
    {
        protected override async ET.ETTask Handle(EcsNode ecsNode, TriggerClipCmd cmd)
        {
            ExecuteClipViewSystem.TriggerClip(cmd.Entity as ExecuteClip);
        }
    }
}
