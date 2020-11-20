using System;
using System.Collections.Generic;


namespace EGamePlay
{
    public sealed class EventSubscribeCollection<T> where T : class
    {
        public List<Action<T>> Subscribes = new List<Action<T>>();
    }

    public sealed class EventComponent : Component
    {
        private Dictionary<Type, object> EventSubscribeCollections = new Dictionary<Type, object>();


        public new void Publish<T>(T TEvent) where T : class
        {
            if (EventSubscribeCollections.TryGetValue(typeof(T), out var collection))
            {
                var eventSubscribeCollection = collection as EventSubscribeCollection<T>;
                for (int i = 0; i < eventSubscribeCollection.Subscribes.Count; i++)
                {
                    eventSubscribeCollection.Subscribes[i].Invoke(TEvent);
                }
            }
        }

        public new void Subscribe<T>(Action<T> action) where T : class
        {
            if (EventSubscribeCollections.TryGetValue(typeof(T), out var collection))
            {
                var eventSubscribeCollection = collection as EventSubscribeCollection<T>;
                eventSubscribeCollection.Subscribes.Add(action);
            }
            else
            {
                var eventSubscribeCollection = new EventSubscribeCollection<T>();
                EventSubscribeCollections.Add(typeof(T), eventSubscribeCollection);
                eventSubscribeCollection.Subscribes.Add(action);
            }
        }

        public new void UnSubscribe<T>(Action<T> action) where T : class
        {
            if (EventSubscribeCollections.TryGetValue(typeof(T), out var collection))
            {
                var eventSubscribeCollection = collection as EventSubscribeCollection<T>;
                eventSubscribeCollection.Subscribes.Remove(action);
            }
        }
    }
}