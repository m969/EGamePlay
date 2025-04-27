using ECS;
using EGamePlay.Combat;

public interface IAfterActionProcess<T1, T2> where T1 : EcsEntity, new() where T2 : IActionExecute, new()
{

}