using System;
using System.Collections.Generic;


namespace EGamePlay
{
    public static class EventAggregatorHelper
    {
        public static void Publish<T>(this object self, T TEvent) where T : class
        {
            EventAggregator.Instance.Publish<T>(TEvent);
        }

        public static void Subscribe<T>(this object self, Action<T> action) where T : class
        {
            EventAggregator.Instance.Subscribe<T>(action);
        }
    }

    public sealed class EventAggregator : Entity
    {
        private static EventAggregator eventAggregator;
        public static EventAggregator Instance
        {
            get
            {
                if (eventAggregator == null) eventAggregator = EntityFactory.Create<EventAggregator>();
                return eventAggregator;
            }
        }


        public void Publish<T>(T TEvent) where T : class
        {

        }

        public void Subscribe<T>(Action<T> action)
        {

        }
    }
}