namespace ECS
{
    public interface IDispatch
    {

    }

    public interface IEventHandle<T> : IDispatch where T : IEvent
    {
        void OnHandleEvent(EcsNode ecsNode, T eventContext);
    }
}