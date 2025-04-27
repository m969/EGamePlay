using System;

namespace SingularityGroup.HotReload {
    /// <summary>
    /// Method with this attribute will get invoked when it gets patched
    /// </summary>
    /// <remarks>
    /// The method with this attribute needs to have no parameters.
    /// Furthermore it needs to either be static or an instance method inside a <see cref="UnityEngine.MonoBehaviour"/>.
    /// For the latter case the method of all instances of the <see cref="UnityEngine.MonoBehaviour"/> will be called.
    /// In case the method has a return value it will be ignored.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class InvokeOnHotReloadLocal : Attribute {
        public readonly string methodToInvoke;

        public InvokeOnHotReloadLocal(string methodToInvoke = null) {
            this.methodToInvoke = methodToInvoke;
        }
    }

}
