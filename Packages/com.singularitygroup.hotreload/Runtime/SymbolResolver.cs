#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)

using System;
using System.Collections.Generic;
using System.Reflection;
using SingularityGroup.HotReload.DTO;
using SingularityGroup.HotReload.RuntimeDependencies;

namespace SingularityGroup.HotReload {
    internal class SymbolResolver {
        readonly Dictionary<string, List<Assembly>> assembliesByName;

        public SymbolResolver(Dictionary<string, List<Assembly>> assembliesByName) {
            this.assembliesByName = assembliesByName;
        }
        
        public void AddAssembly(Assembly asm) {
            var asmName = asm.GetNameSafe();
            List<Assembly> assemblies;
            if(!assembliesByName.TryGetValue(asmName, out assemblies)) {
                assembliesByName.Add(asmName, assemblies = new List<Assembly>());
            }
            assemblies.Add(asm);
        }

         public Type Resolve(SType t) {
            var assmeblies = Resolve(t.assemblyName);
            Type result = null;
            Exception lastException = null;
            for (var i = 0; i < assmeblies.Count; i++) {
                try {
                    result = assmeblies[i].GetLoadedModules()[0].ResolveType(t.metadataToken);
                    if (t.isGenericParameter) {
                        if (!result.IsGenericTypeDefinition) {
                            throw new SymbolResolvingFailedException(t, new ApplicationException("Generic parameter did not resolve to generic type definition"));
                        }
                        var genericParameters = result.GetGenericArguments();
                        if (t.genericParameterPosition >= genericParameters.Length) {
                            throw new SymbolResolvingFailedException(t, new ApplicationException("Generic parameter did not exist on the generic type definition"));
                        }
                        result = genericParameters[t.genericParameterPosition];
                    }
                    break;
                } catch(Exception ex) {
                    lastException = ex;
                }
            }
            if(result == null) {
                throw new SymbolResolvingFailedException(t, lastException);
            }
            return result;
        }
        
         public FieldInfo Resolve(SField t) {
            var assmeblies = Resolve(t.assemblyName);
            FieldInfo result = null;
            Exception lastException = null;
            for (var i = 0; i < assmeblies.Count; i++) {
                try {
                    result = assmeblies[i].GetLoadedModules()[0].ResolveField(t.metadataToken);
                    break;
                } catch(Exception ex) {
                    lastException = ex;
                }
            }
            if(result == null) {
                throw new SymbolResolvingFailedException(t, lastException);
            }
            return result;
        }
        
        public IReadOnlyList<Assembly> Resolve(string assembly) {
            List<Assembly> list;
            if(assembliesByName.TryGetValue(assembly, out list)) {
                return list;
            }
            return Array.Empty<Assembly>();
        }
        
        public MethodBase Resolve(SMethod m) {
            var assmeblies = Resolve(m.assemblyName);
            MethodBase result = null;
            Exception lastException = null;
            for (var i = 0; i < assmeblies.Count; i++) {
                try {
                    result = assmeblies[i].GetLoadedModules()[0].ResolveMethod(m.metadataToken);
                    break;
                } catch(Exception ex) {
                    lastException = ex;
                }
            }
            if(result == null) {
                throw new SymbolResolvingFailedException(m, lastException);
            }
            return result;
        }
    }
}
#endif
