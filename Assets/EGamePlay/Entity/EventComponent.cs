using System;
using System.Collections.Generic;


namespace EGamePlay
{
    public sealed class EventSubscribeCollection<T> where T : class
    {
        public readonly List<EventSubscribe<T>> Subscribes = new List<EventSubscribe<T>>();
        public readonly Dictionary<Action<T>, EventSubscribe<T>> Action2Subscribes = new Dictionary<Action<T>, EventSubscribe<T>>();
        //public readonly List<Action<T>> RemoveActions = new List<Action<T>>();


        public EventSubscribe<T> Add(Action<T> action)
        {
            var eventSubscribe = new EventSubscribe<T>();
            eventSubscribe.EventAction = action;
            Subscribes.Add(eventSubscribe);
            Action2Subscribes.Add(action, eventSubscribe);
            return eventSubscribe;
        }

        //public void AddRemove(Action<T> action)
        //{
        //    RemoveActions.Add(action);
        //}

        public void Remove(Action<T> action)
        {
            Subscribes.Remove(Action2Subscribes[action]);
            Action2Subscribes.Remove(action);
        }
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
        public static bool DebugLog { get; set; } = false;


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
                if (eventSubscribeCollection.Subscribes.Count == 0)
                {
                    return TEvent;
                }
                var arr = eventSubscribeCollection.Subscribes.ToArray();
                foreach (EventSubscribe<T> eventSubscribe in arr)
                {
                    if (eventSubscribe.Coroutine == false)
                    {
                        eventSubscribe.EventAction.Invoke(TEvent);
                    }
                    else
                    {
                        CoroutineEventSubscribeQueue.Add(eventSubscribe, TEvent);
                    }
                }

                //for (int i = eventSubscribeCollection.Subscribes.Count - 1; i >= 0; i--)
                //{
                //    var eventSubscribe = eventSubscribeCollection.Subscribes[i];
                //    if (eventSubscribe.Coroutine == false)
                //    {
                //        eventSubscribe.EventAction.Invoke(TEvent);
                //    }
                //    else
                //    {
                //        CoroutineEventSubscribeQueue.Add(eventSubscribe, TEvent);
                //    }
                //}

                //if (eventSubscribeCollection.RemoveActions.Count > 0)
                //{
                //    foreach (var item in eventSubscribeCollection.RemoveActions)
                //    {
                //        eventSubscribeCollection.Remove(item);
                //    }
                //    eventSubscribeCollection.RemoveActions.Clear();
                //}
            }
            return TEvent;
        }

        public new EventSubscribe<T> Subscribe<T>(Action<T> action) where T : class
        {
            EventSubscribeCollection<T> eventSubscribeCollection;
            if (EventSubscribeCollections.TryGetValue(typeof(T), out var collection))
            {
                eventSubscribeCollection = collection as EventSubscribeCollection<T>;
            }
            else
            {
                eventSubscribeCollection = new EventSubscribeCollection<T>();
                EventSubscribeCollections.Add(typeof(T), eventSubscribeCollection);
            }
            return eventSubscribeCollection.Add(action);
        }

        public new void UnSubscribe<T>(Action<T> action) where T : class
        {
            if (EventSubscribeCollections.TryGetValue(typeof(T), out var collection))
            {
                var eventSubscribeCollection = collection as EventSubscribeCollection<T>;
                eventSubscribeCollection.Remove(action);
            }
        }
    }
}