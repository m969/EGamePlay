using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;

namespace ECSGame
{
    public class Handler_EntityUpdateCmd : ACommandHandler<EntityUpdateCmd>
    {
        protected override async ET.ETTask Handle(EcsNode ecsNode, EntityUpdateCmd cmd)
        {
            var entity = cmd.Entity;
        }
    }
}
