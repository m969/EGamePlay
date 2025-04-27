using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using EGamePlay;
using EGamePlay.Combat;

namespace ECSGame
{
    public class Handler_EndClipCmd : ACommandHandler<EndClipCmd>
    {
        protected override async ET.ETTask Handle(EcsNode ecsNode, EndClipCmd cmd)
        {
            ExecuteClipViewSystem.EndClip(cmd.Entity as ExecuteClip);
        }
    }
}
