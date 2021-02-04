using System;

namespace EGamePlay
{
    [EnableUpdate]
    public class UpdateComponent : Component
    {
        public override void Update()
        {
            Entity.Update();
        }
    }
}