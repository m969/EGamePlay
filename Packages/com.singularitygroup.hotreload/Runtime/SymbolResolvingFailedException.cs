#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System;
using SingularityGroup.HotReload.DTO;

namespace SingularityGroup.HotReload {
    internal class SymbolResolvingFailedException : Exception {
        public SymbolResolvingFailedException(SMethod m, Exception inner) 
            : base($"Unable to resolve method {m.displayName} in assembly {m.assemblyName}", inner) { }
        
        public SymbolResolvingFailedException(SType t, Exception inner) 
            : base($"Unable to resolve type with name: {t.typeName} in assembly {t.assemblyName}", inner) { }
        
        public SymbolResolvingFailedException(SField t, Exception inner) 
            : base($"Unable to resolve field with name: {t.fieldName} in assembly {t.assemblyName}", inner) { }
    }
}
#endif
