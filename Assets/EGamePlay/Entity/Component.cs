using System;

namespace EGamePlay
{
    public class Component : IDisposable
    {
        public Entity Entity { get; set; }
        public bool IsDispose { get; set; }


        public virtual void Setup()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void Dispose()
        {
            IsDispose = true;
        }
    }
}