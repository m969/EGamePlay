using System;
using System.Collections.Generic;
using System.Linq;
using ECS;

namespace ECSGame
{
    /// <summary>
    /// </summary>
    public class EntityObjectComponent : EcsComponent
    {
        public object EntityObject { get; set; }
    }
}