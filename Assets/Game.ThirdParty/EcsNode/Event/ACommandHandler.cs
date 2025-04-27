using System;
using ET;

namespace ECS
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CommandAttribute : Attribute
    {

    }

    public interface ICommandHandler
    {
        Type Type { get; }
        public ETTask HandleCmd(EcsNode ecsNode, ICommand cmd);
    }

    public abstract class ACommandHandler<T> : ICommandHandler where T : ICommand
    {
        public Type Type
        {
            get
            {
                return typeof(T);
            }
        }

        protected abstract ETTask Handle(EcsNode ecsNode, T cmd);

        public async ETTask HandleCmd(EcsNode ecsNode, ICommand cmd)
        {
            try
            {
                await Handle(ecsNode, (T)cmd);
            }
            catch (Exception e)
            {
                ConsoleLog.Error(e);
            }
        }
    }
}