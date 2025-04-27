#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
namespace SingularityGroup.HotReload {
    public interface IServerHealthCheck {
        bool IsServerHealthy { get; }
    }
    
    internal interface IServerHealthCheckInternal : IServerHealthCheck {
        void CheckHealth();
    }
}
#endif
