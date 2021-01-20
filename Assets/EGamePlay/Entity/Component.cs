using System;

namespace EGamePlay
{
    public class Component : IDisposable
    {
        public Entity Entity { get; set; }
        public bool IsDisposed { get; set; }
        public bool Enable { get; set; }
        public bool Disable => Enable == false;


        public T GetEntity<T>() where T : Entity
        {
            return Entity as T;
        }

        public virtual void Setup()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void OnDestroy()
        {

        }

        public void Dispose()
        {
            if (Entity.DebugLog) Log.Debug($"{GetType().Name}->Dispose");
            IsDisposed = true;
        }

        public T Publish<T>(T TEvent) where T : class
        {
            Entity.Publish(TEvent);
            return TEvent;
        }

        public void Subscribe<T>(Action<T> action) where T : class
        {
            Entity.Subscribe(action);
        }

        public void UnSubscribe<T>(Action<T> action) where T : class
        {
            Entity.UnSubscribe(action);
        }
    }
}