#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SingularityGroup.HotReload {
    static class BurstChecker {
        //Use names instead of the types directly for compat with older unity versions
        const string whitelistAttrName = "BurstCompileAttribute";
        const string blacklistAttrName = "BurstDiscardAttribute";
        
        public static bool IsBurstCompiled(MethodBase method) {
            //blacklist has precedence over whitelist
            if(HasAttr(method.GetCustomAttributes(), blacklistAttrName)) {
                return false;
            }
            if(HasAttr(method.GetCustomAttributes(), whitelistAttrName)) {
                return true;
            }
            //Static methods inside a [BurstCompile] type are not burst compiled by default
            if(method.DeclaringType == null || method.IsStatic) {
                return false;
            }
            if(HasAttr(method.DeclaringType.GetCustomAttributes(), whitelistAttrName)) {
                return true;
            }
            //No matching attributes
            return false;
        }
        
        static bool HasAttr(IEnumerable<Attribute> attributes, string name) {
            foreach (var attr in attributes) {
                if(attr.GetType().Name == name) {
                    return true;
                }
            }
            return false;
        }
    }
}
#endif
