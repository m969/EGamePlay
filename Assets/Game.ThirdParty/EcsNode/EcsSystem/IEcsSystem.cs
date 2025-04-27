using System;

namespace ECS
{
    /// <summary>
    /// 系统是包含的机制，底层系统包含上层系统，上层系统包含更上层的系统
    /// </summary>
    public interface IEcsSystem
    {
        Type EntityType { get; }
    }

    public interface IEcsEntitySystem : IEcsSystem
    {
    }

    public interface IEcsComponentSystem : IEcsSystem
    {
        Type ComponentType { get; }
    }
}