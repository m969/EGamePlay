using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECS
{
    /// <summary>
    /// 普通指令接口
    /// </summary>
    public interface ICommand
    {
        public EcsEntity Entity { get; set; }
    }

    /// <summary>
    /// 执行指令接口，在发出指令的同时可携带一个可传参的Action，用于处理指令的时候按需调用
    /// </summary>
    public interface IExecuteCommand : ICommand
    {
        public Action<object> ExecuteAction { get; set; }
    }

    /// <summary>
    /// 队列执行指令接口
    /// </summary>
    public interface IQueueExecuteCommand : IExecuteCommand
    {

    }

    /// <summary>
    /// 泛型互斥执行指令接口
    /// </summary>
    public interface IRepelExecuteCommand : IExecuteCommand
    {

    }

    /// <summary>
    /// 同型互斥执行指令接口
    /// </summary>
    public interface ITypeRepelExecuteCommand : IExecuteCommand
    {

    }
}
