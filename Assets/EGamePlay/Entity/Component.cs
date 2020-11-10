using System;

namespace EGamePlay
{
    public class Component : IDisposable
    {
        public Entity Master { get; private set; }
        public bool IsDispose { get; set; }


        public void SetMaster(Entity master)
        {
            Master = master;
        }

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