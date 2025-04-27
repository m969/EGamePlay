#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)

using System;
using System.Reflection;
using SingularityGroup.HotReload.MonoMod.Utils;

namespace SingularityGroup.HotReload {
    static class MethodCompatiblity {
        internal static string CheckCompatibility(MethodBase previousMethod, MethodBase patchMethod) { 
            var previousConstructor  = previousMethod as ConstructorInfo;
            var patchConstructor = patchMethod as ConstructorInfo;
            if(previousConstructor != null && !ReferenceEquals(patchConstructor, null)) {
                return AreConstructorsCompatible(previousConstructor, patchConstructor);
            }
            var previousMethodInfo = previousMethod as MethodInfo;
            var patchMethodInfo = patchMethod as MethodInfo;
            if(!ReferenceEquals(previousMethodInfo, null) && !ReferenceEquals(patchMethodInfo, null)) {
                return AreMethodInfosCompatible(previousMethodInfo, patchMethodInfo);
            }
            return "unknown issue";
        }
            
        static string AreMethodBasesCompatible(MethodBase previousMethod, MethodBase patchMethod) {
            if(previousMethod.Name != patchMethod.Name) {
                return "Method name mismatch";
            }
            //Declaring type of patch method is different from the target method but their full name (namespace + name) is equal
            bool isDeclaringTypeCompatible = false;
            var declaringType = patchMethod.DeclaringType;
            while (declaringType != null) {
                if(previousMethod.DeclaringType?.FullName == declaringType.FullName) {
                    isDeclaringTypeCompatible = true;
                    break;
                }
                declaringType = declaringType.BaseType;
            }
            if (!isDeclaringTypeCompatible) {
                return "Declaring type name mismatch";
            }
            //Check in case type parameter overloads to distinguish between: void M<T>() { } <-> void M() { }
            if(previousMethod.IsGenericMethodDefinition != patchMethod.IsGenericMethodDefinition) {
                return "IsGenericMethodDefinition mismatch";
            }
            
            var prevParams = previousMethod.GetParameters();
            var patchParams = patchMethod.GetParameters();
            ArraySegment<ParameterInfo> patchParamsSegment;
            bool patchMethodHasExplicitThis;
            if(previousMethod.IsStatic || previousMethod.Name.Contains("<") && !patchMethod.IsStatic) {
                patchMethodHasExplicitThis = false;
            } else {
                patchMethodHasExplicitThis = true;
            }
            if(LikelyHasExplicitThis(prevParams, patchParams, previousMethod)) {
                patchMethodHasExplicitThis = true;
            }
            //Special edge case: User added static keyword to method. No explicit this will be generated in that case
            if(!previousMethod.IsStatic && patchMethod.IsStatic && !LikelyHasExplicitThis(prevParams, patchParams, previousMethod)) {
                patchMethodHasExplicitThis = false;
            }
            if(patchMethodHasExplicitThis) {
                //Special case: patch method for an instance method is static and has an explicit this parameter.
                //If the patch method doesn't have any parameters it is not compatible.
                if(patchParams.Length == 0) {
                    return "missing this parameter";
                }
                //this parameter has to be the declaring type
                if(!ParamTypeMatches(patchParams[0].ParameterType, previousMethod.DeclaringType)) {
                    return "this parameter type mismatch";
                }
                //Ignore the this parameter and compare the remaining ones.
                patchParamsSegment = new ArraySegment<ParameterInfo>(patchParams, 1, patchParams.Length - 1);
            } else {
                patchParamsSegment = new ArraySegment<ParameterInfo>(patchParams);
            }
            return CompareParameters(new ArraySegment<ParameterInfo>(prevParams), patchParamsSegment);
        }
        
        static bool LikelyHasExplicitThis(ParameterInfo[] prevParams, ParameterInfo[] patchParams, MethodBase previousMethod) {
            if (patchParams.Length != prevParams.Length + 1) {
                return false;
            }
            var patchT = patchParams[0].ParameterType;
            if (!ParamTypeMatches(patchT, previousMethod.DeclaringType)) {
                return false;
            }
            return patchParams[0].Name == "this";
        }
        
        static bool ParamTypeMatches(Type patchT, Type originalT) {
            return patchT == originalT || patchT.IsByRef && patchT.GetElementType() == originalT;
        }
        
        static string CompareParameters(ArraySegment<ParameterInfo> x, ArraySegment<ParameterInfo> y) {
            if(x.Count != y.Count) {
                return "parameter count mismatch";
            }
            for (var i = 0; i < x.Count; i++) {
                if(x.Array[i + x.Offset].ParameterType != y.Array[i + y.Offset].ParameterType) {
                    return "parameter type mismatch";
                }
            }
            return null;
        }
            

        static string AreConstructorsCompatible(ConstructorInfo x, ConstructorInfo y) {
            return AreMethodBasesCompatible(x, y);
        }
            
        static string AreMethodInfosCompatible(MethodInfo x, MethodInfo y) {
            return AreMethodBasesCompatible(x, y) ?? (x.ReturnType == y.ReturnType ? null : "Return type mismatch");
        }
    }
}
#endif
