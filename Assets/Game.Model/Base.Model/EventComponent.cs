using ECS;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ECS
{
    public class EventComponent : EcsComponent
    {
        public List<IEventRun> RunningEvents { get; set; } = new();
        public Queue<ICommand> DispatchCommands { get; set; } = new();
        public Queue<IExecuteCommand> ExecuteCommands { get; set; } = new();
        public Dictionary<Type, List<ICommandHandler>> CommandHandlers { get; set; } = new();
    }
}