using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECS;
using ET;

namespace ECSGame
{
    public class Handler_AfterRunEventCmd : ACommandHandler<AfterRunEventCmd>
    {
        protected override async ET.ETTask Handle(EcsNode ecsNode, AfterRunEventCmd cmd)
        {

        }
    }
}
