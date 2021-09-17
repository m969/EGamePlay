using System;
using System.Collections.Generic;


namespace EGamePlay
{
    public class EventExecution<T> : Entity where T : class
    {
        public virtual void OnStart()
        {

        }

        public override void Update()
        {
            Run();
        }

        public virtual void Run()
        {

        }

        public virtual void OnFinish()
        {

        }

        public virtual void OnEnd()
        {

        }
    }
}
