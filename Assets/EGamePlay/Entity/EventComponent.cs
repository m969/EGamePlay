using System;
using System.Collections.Generic;


namespace EGamePlay
{
    //public sealed class EventSubscribeCollection<T> where T : class
    //{
    //    public readonly List<EventSubscribe<T>> Subscribes = new List<EventSubscribe<T>>();
    //    public readonly Dictionary<Action<T>, EventSubscribe<T>> Action2Subscribes = new Dictionary<Action<T>, EventSubscribe<T>>();


    //    public EventSubscribe<T> Add(Action<T> action)
    //    {
    //        var eventSubscribe = new EventSubscribe<T>();
    //        eventSubscribe.EventAction = action;
    //        Subscribes.Add(eventSubscribe);
    //        Action2Subscribes.Add(action, eventSubscribe);
    //        return eventSubscribe;
    //    }

    //    public void Remove(Action<T> action)
    //    {
    //        Subscribes.Remove(Action2Subscribes[action]);
    //        Action2Subscribes.Remove(action);
    //    }
    //}

    //public sealed class EventSubscribe<T>
    //{
    //    public Action<T> EventAction;
    //}

    public sealed class EventComponent : Component
    {
        public override bool Enable { get; set; } = true;
        private Dictionary<Type, List<object>> Event2ActionLists = new Dictionary<Type, List<object>>();
        public static bool DebugLog { get; set; } = false;


        //private Dictionary<Type, object> EventSubscribeCollections = new Dictionary<Type, object>();

        //public new T Publish<T>(T TEvent) where T : class
        //{
        //    if (EventSubscribeCollections.TryGetValue(typeof(T), out var collection))
        //    {
        //        var eventSubscribeCollection = collection as EventSubscribeCollection<T>;
        //        if (eventSubscribeCollection.Subscribes.Count == 0)
        //        {
        //            return TEvent;
        //        }
        //        var eventSubscribes = eventSubscribeCollection.Subscribes.ToArray();
        //        foreach (EventSubscribe<T> eventSubscribe in eventSubscribes)
        //        {
        //            eventSubscribe.EventAction.Invoke(TEvent);
        //        }
        //    }
        //    return TEvent;
        //}

        //public new EventSubscribe<T> Subscribe<T>(Action<T> action) where T : class
        //{
        //    EventSubscribeCollection<T> eventSubscribeCollection;
        //    if (EventSubscribeCollections.TryGetValue(typeof(T), out var collection))
        //    {
        //        eventSubscribeCollection = collection as EventSubscribeCollection<T>;
        //    }
        //    else
        //    {
        //        eventSubscribeCollection = new EventSubscribeCollection<T>();
        //        EventSubscribeCollections.Add(typeof(T), eventSubscribeCollection);
        //    }
        //    return eventSubscribeCollection.Add(action);
        //}

        //public new void UnSubscribe<T>(Action<T> action) where T : class
        //{
        //    if (EventSubscribeCollections.TryGetValue(typeof(T), out var collection))
        //    {
        //        var eventSubscribeCollection = collection as EventSubscribeCollection<T>;
        //        eventSubscribeCollection.Remove(action);
        //    }
        //}

        public new T Publish<T>(T TEvent) where T : class
        {
            if (Event2ActionLists.TryGetValue(typeof(T), out var actionList))
            {
                foreach (Action<T> action in actionList)
                {
                    action.Invoke(TEvent);
                }
            }
            return TEvent;
        }

        public new void Subscribe<T>(Action<T> action) where T : class
        {
            if (Event2ActionLists.ContainsKey(typeof(T)) == false)
            {
                Event2ActionLists.Add(typeof(T), new List<object>());
            }
            Event2ActionLists[typeof(T)].Add(action);
        }

        public new void UnSubscribe<T>(Action<T> action) where T : class
        {
            if (Event2ActionLists.TryGetValue(typeof(T), out var actionList))
            {
                actionList.Remove(action);
            }
        }
    }
}