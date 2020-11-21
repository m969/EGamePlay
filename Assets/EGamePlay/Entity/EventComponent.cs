using System;
using System.Collections.Generic;


namespace EGamePlay
{
    public sealed class EventSubscribeCollection<T> where T : class
    {
        public List<EventSubscribe<T>> Subscribes = new List<EventSubscribe<T>>();
    }

    public sealed class EventSubscribe<T>
    {
        public Action<T> EventAction;
        public bool Coroutine;

        public void AsCoroutine()
        {
            Coroutine = true;
        }
    }

    public sealed class EventComponent : Component
    {
        private Dictionary<Type, object> EventSubscribeCollections = new Dictionary<Type, object>();
        private Dictionary<object, object> CoroutineEventSubscribeQueue = new Dictionary<object, object>();
        private Dictionary<object, object> SwapCoroutineEventSubscribeQueue = new Dictionary<object, object>();


        public override void Update()
        {
            if (CoroutineEventSubscribeQueue.Count > 0)
            {
                foreach (var item in CoroutineEventSubscribeQueue)
                {
                    var TEvent = item.Value;
                    var eventSubscribe = item.Key;
                    var field = eventSubscribe.GetType().GetField("EventAction");
                    var value = field.GetValue(eventSubscribe);
                    value.GetType().GetMethod("Invoke").Invoke(value, new object[] { TEvent });
                }
                CoroutineEventSubscribeQueue.Clear();
            }
        }

        public new T Publish<T>(T TEvent) where T : class
        {
            if (EventSubscribeCollections.TryGetValue(typeof(T), out var collection))
            {
                var eventSubscribeCollection = collection as EventSubscribeCollection<T>;
                for (int i = 0; i < eventSubscribeCollection.Subscribes.Count; i++)
                {
                    var eventSubscribe = eventSubscribeCollection.Subscribes[i];
                    if (eventSubscribe.Coroutine == false)
                    {
                        eventSubscribe.EventAction.Invoke(TEvent);
                    }
                    else
                    {
                        CoroutineEventSubscribeQueue.Add(eventSubscribe, TEvent);
                    }
                }
            }
            return TEvent;
        }

        public new EventSubscribe<T> Subscribe<T>(Action<T> action) where T : class
        {
            var eventSubscribe = new EventSubscribe<T>();
            eventSubscribe.EventAction = action;
            if (EventSubscribeCollections.TryGetValue(typeof(T), out var collection))
            {
                var eventSubscribeCollection = collection as EventSubscribeCollection<T>;
                eventSubscribeCollection.Subscribes.Add(eventSubscribe);
            }
            else
            {
                var eventSubscribeCollection = new EventSubscribeCollection<T>();
                EventSubscribeCollections.Add(typeof(T), eventSubscribeCollection);
                eventSubscribeCollection.Subscribes.Add(eventSubscribe);
            }
            return eventSubscribe;
        }

        public new void UnSubscribe<T>(Action<T> action) where T : class
        {
            //if (EventSubscribeCollections.TryGetValue(typeof(T), out var collection))
            //{
            //    var eventSubscribeCollection = collection as EventSubscribeCollection<T>;
            //    eventSubscribeCollection.Subscribes.Remove(action);
            //}
        }
    }
}