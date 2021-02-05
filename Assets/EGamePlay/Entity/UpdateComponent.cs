using System;

namespace EGamePlay
{
    [EnableUpdate]
    public class UpdateComponent : Component
    {
        public override bool Enable { get; set; } = true;


        public override void Update()
        {
            Entity.Update();
        }
    }
}