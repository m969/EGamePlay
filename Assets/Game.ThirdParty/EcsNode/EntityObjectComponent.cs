using System;
using System.Collections.Generic;
using System.Linq;

namespace ECS
{
    /// <summary>
    /// </summary>
    public class EntityObjectComponent:EcsComponent
    {
        public object EntityObject { get; set; }
    }
}