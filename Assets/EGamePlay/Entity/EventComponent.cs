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
        private Dictionary<Type, List<object>> Event2ActionLists = new Dictionary<Type, List<object>>();
        public static bool DebugLog { get; set; } = false;


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

        public new SubscribeSubject Subscribe<T>(Action<T> action) where T : class
        {
            var type = typeof(T);
            if (Event2ActionLists.ContainsKey(type) == false)
            {
                Event2ActionLists.Add(type, new List<object>());
            }
            Event2ActionLists[type].Add(action);
            return Entity.AddChild<SubscribeSubject>(action);
        }

        public new void UnSubscribe<T>(Action<T> action) where T : class
        {
            if (Event2ActionLists.TryGetValue(typeof(T), out var actionList))
            {
                actionList.Remove(action);
            }
            Entity.Find<SubscribeSubject>(action.GetHashCode().ToString())?.Dispose();
        }
    }
}
