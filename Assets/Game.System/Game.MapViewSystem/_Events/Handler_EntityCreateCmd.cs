using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;
using EGamePlay.Combat;
using EGamePlay;

namespace ECSGame
{
    public class Handler_EntityCreateCmd : ACommandHandler<EntityCreateCmd>
    {
        protected override async ET.ETTask Handle(EcsNode ecsNode, EntityCreateCmd cmd)
        {
            var entity = cmd.Entity;
        }
    }
}
