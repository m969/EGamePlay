using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    public interface IDomainEvent
    {
    }

    public interface IEventRun
    {
        public ETTask Handle(EcsNode domain, IDomainEvent a);
    }

    public abstract class AEventRun<T, A> : IEventRun where T : EcsNode where A : IDomainEvent
    {
        protected abstract ETTask Run(T domain, A a);
        public async ETTask Handle(EcsNode domain, IDomainEvent a)
        {
            try
            {
                await Run((T)domain, (A)a);
            }
            catch (Exception e)
            {
                ConsoleLog.Error(e);
            }
        }
    }
}
