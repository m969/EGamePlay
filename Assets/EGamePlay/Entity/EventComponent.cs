using System;
using System.Collections.Generic;


namespace EGamePlay
{
    public class SubscribeSubject : Entity
    {
        public override void Awake(object initData)
        {
            Name = initData.GetHashCode().ToString();
        }

        public SubscribeSubject DisposeWith(Entity entity)
        {
            entity.SetChild(this);
            return this;
        }
    }

    public sealed class EventComponent : Component
    {
        public override bool DefaultEnable { get; set; } = false;
        private Dictionary<Type, List<object>> TypeEvent2ActionLists = new Dictionary<Type, List<object>>();
        private Dictionary<string, List<object>> FireEvent2ActionLists = new Dictionary<string, List<object>>();
        public static bool DebugLog { get; set; } = false;


        public new T Publish<T>(T TEvent) where T : class
        {
            if (TypeEvent2ActionLists.TryGetValue(typeof(T), out var actionList))
            {
                var tempList = actionList.ToArray();
                foreach (Action<T> action in tempList)
                {
                    action.Invoke(TEvent);
                }
            }
            return TEvent;
        }

        public new SubscribeSubject Subscribe<T>(Action<T> action) where T : class
        {
            var type = typeof(T);
            if (!TypeEvent2ActionLists.TryGetValue(type, out var actionList))
            {
                actionList = new List<object>();
                TypeEvent2ActionLists.Add(type, actionList);
            }
            actionList.Add(action);
            return Entity.AddChild<SubscribeSubject>(action);
        }

        public new void UnSubscribe<T>(Action<T> action) where T : class
        {
            if (TypeEvent2ActionLists.TryGetValue(typeof(T), out var actionList))
            {
                actionList.Remove(action);
            }
            var e = Entity.Find<SubscribeSubject>(action.GetHashCode().ToString());
            if (e != null)
            {
                Entity.Destroy(e);
            }
        }

        public void FireEvent<T>(string eventType, T entity) where T : Entity
        {
            if (FireEvent2ActionLists.TryGetValue(eventType, out var actionList))
            {
                var tempList = actionList.ToArray();
                foreach (Action<T> action in tempList)
                {
                    action.Invoke(entity);
                }
            }
        }

        public void OnEvent<T>(string eventType, Action<T> action) where T : Entity
        {
            if (FireEvent2ActionLists.TryGetValue(eventType, out var actionList))
            {
            }
            else
            {
                actionList = new List<object>();
                FireEvent2ActionLists.Add(eventType, actionList);
            }
            actionList.Add(action);
        }

        public void OffEvent<T>(string eventType, Action<T> action) where T : Entity
        {
            if (FireEvent2ActionLists.TryGetValue(eventType, out var actionList))
            {
                actionList.Remove(action);
            }
        }
    }
}


//public void FireEvent<T>(string eventType, T entity) where T : Entity
//{
//    if (FireEvent2ActionLists.TryGetValue(eventType, out var actionList))
//    {
//        var tempList = actionList.ToArray();
//        foreach (Action<T> action in tempList)
//        {
//            action.Invoke(entity);
//        }
//    }
//}

//public void OnEvent<T>(string eventType, Action<T> action) where T : Entity
//{
//    if (FireEvent2ActionLists.TryGetValue(eventType, out var actionList))
//    {
//    }
//    else
//    {
//        actionList = new List<object>();
//        FireEvent2ActionLists.Add(eventType, actionList);
//    }
//    actionList.Add(action);
//}

//public void OffEvent<T>(string eventType, Action<T> action) where T : Entity
//{
//    if (FireEvent2ActionLists.TryGetValue(eventType, out var actionList))
//    {
//        actionList.Remove(action);
//    }
//}