using System;
using System.Collections.Generic;


namespace EGamePlay
{
    public static class EEvent
    {
        public static void Run<T>() where T : EventExecution
        {
            var evnt = Entity.Create<T>();
            evnt.Run();
            Entity.Destroy(evnt);
        }

        public static void Run<T>(Entity entity) where T : EventExecution<Entity>
        {
            var evnt = Entity.Create<T>();
            evnt.Run(entity);
            Entity.Destroy(evnt);
        }

        public static ET.ETTask RunAsync<T>(T TEvent) where T : AsyncEventExecution
        {
            var evnt = Entity.Create<T>();
            evnt.ETTaskCompletionSource = new ET.ETTaskCompletionSource();
            return evnt.RunAsync();
        }

        public static void Run<T, A1>(T TEvent) where T : EventExecution
        {

        }

        public static void Start<T>() where T : EventExecution
        {
            var evnt = Entity.Create<T>();
        }
    }
}
