#if ODIN_INSPECTOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SingularityGroup.HotReload.EditorDependencies;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

namespace SingularityGroup.HotReload.Editor {
	public class HotReloadAttributeProcessor : OdinAttributeProcessor {
		public override bool CanProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member) {
			return member is FieldInfo;
		}

		static object nullObject = new object();
		public override void ProcessChildMemberAttributes(InspectorProperty property, MemberInfo member, List<Attribute> attributes) {
			var field = member as FieldInfo;
			if (field?.DeclaringType == null) {
				return;
			}
			if (UnityFieldHelper.TryGetFieldAttributes(field, out var fieldAttributes)) {
				attributes.Clear();
				attributes.AddRange(fieldAttributes);
			}
			if (UnityFieldHelper.IsFieldHidden(field.DeclaringType, field.Name)) {
				attributes.Add(new HideIfAttribute("@true"));
			}
			// we assume this is always not null. Most of the times it will not be. If it is the side effect is some memory footprint which hopefully gets cleared when enough objects
			var key = property.ParentValues.FirstOrDefault() ?? nullObject;
			UnityFieldHelper.CacheFieldInvalidation(key, field, property.RefreshSetup);
		}
	}
}
#endif
