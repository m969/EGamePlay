using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay
{
    public class Component
    {
        public Entity Parent { get; private set; }

        public void SetParent(Entity parent)
        {
            Parent = parent;
        }
    }
}