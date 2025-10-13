using ECS;
using EGamePlay.Combat;
using ET;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public class EventSystem : AComponentSystem<EcsNode, EventComponent>,
IAwake<EcsNode, EventComponent>
    {
        public void Awake(EcsNode entity, EventComponent component)
        {
            Reload(entity);
        }

        public static void Reload(EcsNode ecsNode)
        {
            var component = ecsNode.GetComponent<EventComponent>();
            var CommandHandlers = new Dictionary<Type, List<ICommandHandler>>();

            var types = ecsNode.AllTypes;
            foreach (var item in types)
            {
                if (item.BaseType == null) continue;
                if (item.BaseType.BaseType == null) continue;
                if (typeof(ICommandHandler).IsAssignableFrom(item.BaseType) == false) continue;

                var handler = Activator.CreateInstance(item) as ICommandHandler;
                var cmdType = handler.Type;
                CommandHandlers.TryGetValue(cmdType, out var handlers);
                if (handlers == null)
                {
                    handlers = new List<ICommandHandler>();
                    CommandHandlers[cmdType] = handlers;
                }
                handlers.Add(handler);
            }

            component.CommandHandlers = CommandHandlers;
        }

        public static void Update(EcsNode entity, EventComponent component)
        {
            while (component.DispatchCommands.Count > 0)
            {
                var cmd = component.DispatchCommands.Dequeue();
                if (component.CommandHandlers.TryGetValue(cmd.GetType(), out var handlers))
                {
                    foreach (var handler in handlers)
                    {
                        handler.HandleCmd(entity, cmd);
                    }
                }
            }
        }

        public static void Dispatch<T>(T cmd) where T : struct, ICommand
        {
            cmd.Entity.EcsNode.GetComponent<EventComponent>().DispatchCommands.Enqueue(cmd);
        }

        public static void Execute<T>(T cmd) where T : struct, IExecuteCommand
        {
            cmd.Entity.EcsNode.GetComponent<EventComponent>().ExecuteCommands.Enqueue(cmd);
        }

        public static void Dispatch<T>(EcsEntity entity, Action<T> action)
        {
            if (entity == null || entity.IsDisposed)
            {
                return;
            }
            if (entity.EcsNode.EntityType2Systems.TryGetValue(entity.GetType(), out var systems))
            {
                foreach (var item in systems)
                {
                    if (item is T eventInstance)
                    {
                        action.Invoke(eventInstance);
                    }
                }
            }
        }

        public static async ETTask Run<T, A>(T eventRun, A a) where T : AEventRun<T, A>, new() where A : EcsEntity
        {
            Dispatch(new BeforeRunEventCmd() { Entity = a, EventRun = eventRun, EventEntity = a });
            await eventRun.Handle(a);
            Dispatch(new AfterRunEventCmd() { Entity = a, EventRun = eventRun, EventEntity = a });
        }

        public static async ETTask Run<T, A1, A2>(T eventRun, A1 a1, A2 a2) where T : AEventRun<T, A1, A2>, new() where A1 : EcsEntity
        {
            Dispatch(new BeforeRunEventCmd() { Entity = a1, EventRun = eventRun, EventEntity = a1, EventArgs2 = a2 });
            await eventRun.Handle(a1, a2);
            Dispatch(new AfterRunEventCmd() { Entity = a1, EventRun = eventRun, EventEntity = a1, EventArgs2 = a2 });
        }

        public static async ETTask Run<T, A1, A2, A3>(T eventRun, A1 a1, A2 a2, A3 a3) where T : AEventRun<T, A1, A2, A3>, new() where A1 : EcsEntity
        {
            Dispatch(new BeforeRunEventCmd() { Entity = a1, EventRun = eventRun, EventEntity = a1, EventArgs2 = a2, EventArgs3 = a3 });
            await eventRun.Handle(a1, a2, a3);
            Dispatch(new AfterRunEventCmd() { Entity = a1, EventRun = eventRun, EventEntity = a1, EventArgs2 = a2, EventArgs3 = a3 });
        }
    }
}
