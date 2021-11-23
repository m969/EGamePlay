using System;
using System.Collections.Generic;


namespace EGamePlay
{
    public interface IEventExecution
    {

    }

    public abstract class EventExecution : Entity, IEventExecution
    {
        public abstract void Run();
    }

    public abstract class EventExecution<E> : Entity, IEventExecution
    {
        public abstract void Run(E a1);
    }

    public abstract class AsyncEventExecution : Entity, IEventExecution
    {
        public ET.ETTaskCompletionSource ETTaskCompletionSource { get; set; }


        public abstract ET.ETTask RunAsync();

        public void Finish()
        {
            ETTaskCompletionSource.SetResult();
            Destroy(this);
        }
    }
}
