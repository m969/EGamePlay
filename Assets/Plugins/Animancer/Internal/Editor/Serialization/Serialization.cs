// Serialization // Copyright 2020 Kybernetik //

#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

// Shared File Last Modified: 2020-09-11.
namespace Animancer.Editor
// namespace InspectorGadgets.Editor
// namespace UltEvents.Editor
{
    /// <summary>The possible states for a function in a <see cref="GenericMenu"/>.</summary>
    public enum MenuFunctionState
    {
        /************************************************************************************************************************/

        /// <summary>Displayed normally.</summary>
        Normal,

        /// <summary>Has a check mark next to it to show that it is selected.</summary>
        Selected,

        /// <summary>Greyed out and unusable.</summary>
        Disabled,

        /************************************************************************************************************************/
    }

    /// <summary>[Editor-Only] Various serialization utilities.</summary>
    public static partial class Serialization
    {
        /************************************************************************************************************************/
        #region Public Static API
        /************************************************************************************************************************/

        /// <summary>The text used in a <see cref="SerializedProperty.propertyPath"/> to denote array elements.</summary>
        public const string
            ArrayDataPrefix = ".Array.data[",
            ArrayDataSuffix = "]";

        /************************************************************************************************************************/

        /// <summary>Returns a user friendly version of the <see cref="SerializedProperty.propertyPath"/>.</summary>
        public static string GetFriendlyPath(this SerializedProperty property)
        {
            return property.propertyPath.Replace(ArrayDataPrefix, "[");
        }

        /************************************************************************************************************************/
        #region Get Value
        /************************************************************************************************************************/

        /// <summary>Gets the value of the specified <see cref="SerializedProperty"/>.</summary>
        public static object GetValue(this SerializedProperty property, object targetObject)
        {
            if (property.hasMultipleDifferentValues &&
                property.serializedObject.targetObject != targetObject as Object)
            {
                property = new SerializedObject(targetObject as Object).FindProperty(property.propertyPath);
            }

            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean: return property.boolValue;
                case SerializedPropertyType.Float: return property.floatValue;
                case SerializedPropertyType.Integer: return property.intValue;
                case SerializedPropertyType.String: return property.stringValue;

                case SerializedPropertyType.Vector2: return property.vector2Value;
                case SerializedPropertyType.Vector3: return property.vector3Value;
                case SerializedPropertyType.Vector4: return property.vector4Value;

                case SerializedPropertyType.Quaternion: return property.quaternionValue;
                case SerializedPropertyType.Color: return property.colorValue;
                case SerializedPropertyType.AnimationCurve: return property.animationCurveValue;

                case SerializedPropertyType.Rect: return property.rectValue;
                case SerializedPropertyType.Bounds: return property.boundsValue;

                case SerializedPropertyType.Vector2Int: return property.vector2IntValue;
                case SerializedPropertyType.Vector3Int: return property.vector3IntValue;
                case SerializedPropertyType.RectInt: return property.rectIntValue;
                case SerializedPropertyType.BoundsInt: return property.boundsIntValue;

                case SerializedPropertyType.ObjectReference: return property.objectReferenceValue;
                case SerializedPropertyType.ExposedReference: return property.exposedReferenceValue;

                case SerializedPropertyType.ArraySize: return property.intValue;
                case SerializedPropertyType.FixedBufferSize: return property.fixedBufferSize;

                case SerializedPropertyType.Generic:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.Character:
                default:
                    var accessor = GetAccessor(property);
                    if (accessor != null)
                        return accessor.GetValue(targetObject);
                    else
                        return null;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Gets the value of the <see cref="SerializedProperty"/>.</summary>
        public static object GetValue(this SerializedProperty property) => GetValue(property, property.serializedObject.targetObject);

        /// <summary>Gets the value of the <see cref="SerializedProperty"/>.</summary>
        public static T GetValue<T>(this SerializedProperty property) => (T)GetValue(property);

        /// <summary>Gets the value of the <see cref="SerializedProperty"/>.</summary>
        public static void GetValue<T>(this SerializedProperty property, out T value) => value = (T)GetValue(property);

        /************************************************************************************************************************/

        /// <summary>Gets the value of the <see cref="SerializedProperty"/> for each of its target objects.</summary>
        public static T[] GetValues<T>(this SerializedProperty property)
        {
            try
            {
                var targetObjects = property.serializedObject.targetObjects;
                var values = new T[targetObjects.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = (T)GetValue(property, targetObjects[i]);
                }

                return values;
            }
            catch
            {
                return null;
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Set Value
        /************************************************************************************************************************/

        /// <summary>Sets the value of the specified <see cref="SerializedProperty"/>.</summary>
        public static void SetValue(this SerializedProperty property, object targetObject, object value)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean: property.boolValue = (bool)value; break;
                case SerializedPropertyType.Float: property.floatValue = (float)value; break;
                case SerializedPropertyType.Integer: property.intValue = (int)value; break;
                case SerializedPropertyType.String: property.stringValue = (string)value; break;

                case SerializedPropertyType.Vector2: property.vector2Value = (Vector2)value; break;
                case SerializedPropertyType.Vector3: property.vector3Value = (Vector3)value; break;
                case SerializedPropertyType.Vector4: property.vector4Value = (Vector4)value; break;

                case SerializedPropertyType.Quaternion: property.quaternionValue = (Quaternion)value; break;
                case SerializedPropertyType.Color: property.colorValue = (Color)value; break;
                case SerializedPropertyType.AnimationCurve: property.animationCurveValue = (AnimationCurve)value; break;

                case SerializedPropertyType.Rect: property.rectValue = (Rect)value; break;
                case SerializedPropertyType.Bounds: property.boundsValue = (Bounds)value; break;

                case SerializedPropertyType.Vector2Int: property.vector2IntValue = (Vector2Int)value; break;
                case SerializedPropertyType.Vector3Int: property.vector3IntValue = (Vector3Int)value; break;
                case SerializedPropertyType.RectInt: property.rectIntValue = (RectInt)value; break;
                case SerializedPropertyType.BoundsInt: property.boundsIntValue = (BoundsInt)value; break;

                case SerializedPropertyType.ObjectReference: property.objectReferenceValue = (Object)value; break;
                case SerializedPropertyType.ExposedReference: property.exposedReferenceValue = (Object)value; break;

                case SerializedPropertyType.ArraySize: property.intValue = (int)value; break;

                case SerializedPropertyType.FixedBufferSize:
                    throw new InvalidOperationException($"{nameof(SetValue)} failed:" +
                        $" {nameof(SerializedProperty)}.{nameof(SerializedProperty.fixedBufferSize)} is read-only.");

                case SerializedPropertyType.Generic:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.Character:
                default:
                    var accessor = GetAccessor(property);
                    if (accessor != null)
                        accessor.SetValue(targetObject, value);
                    break;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Sets the value of the <see cref="SerializedProperty"/>.</summary>
        public static void SetValue(this SerializedProperty property, object value)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean: property.boolValue = (bool)value; break;
                case SerializedPropertyType.Float: property.floatValue = (float)value; break;
                case SerializedPropertyType.Integer: property.intValue = (int)value; break;
                case SerializedPropertyType.String: property.stringValue = (string)value; break;

                case SerializedPropertyType.Vector2: property.vector2Value = (Vector2)value; break;
                case SerializedPropertyType.Vector3: property.vector3Value = (Vector3)value; break;
                case SerializedPropertyType.Vector4: property.vector4Value = (Vector4)value; break;

                case SerializedPropertyType.Quaternion: property.quaternionValue = (Quaternion)value; break;
                case SerializedPropertyType.Color: property.colorValue = (Color)value; break;
                case SerializedPropertyType.AnimationCurve: property.animationCurveValue = (AnimationCurve)value; break;

                case SerializedPropertyType.Rect: property.rectValue = (Rect)value; break;
                case SerializedPropertyType.Bounds: property.boundsValue = (Bounds)value; break;

                case SerializedPropertyType.Vector2Int: property.vector2IntValue = (Vector2Int)value; break;
                case SerializedPropertyType.Vector3Int: property.vector3IntValue = (Vector3Int)value; break;
                case SerializedPropertyType.RectInt: property.rectIntValue = (RectInt)value; break;
                case SerializedPropertyType.BoundsInt: property.boundsIntValue = (BoundsInt)value; break;

                case SerializedPropertyType.ObjectReference: property.objectReferenceValue = (Object)value; break;
                case SerializedPropertyType.ExposedReference: property.exposedReferenceValue = (Object)value; break;

                case SerializedPropertyType.ArraySize: property.intValue = (int)value; break;

                case SerializedPropertyType.FixedBufferSize:
                    throw new InvalidOperationException($"{nameof(SetValue)} failed:" +
                        $" {nameof(SerializedProperty)}.{nameof(SerializedProperty.fixedBufferSize)} is read-only.");

                case SerializedPropertyType.Generic:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.Character:
                default:
                    var accessor = GetAccessor(property);
                    if (accessor != null)
                    {
                        var targets = property.serializedObject.targetObjects;
                        for (int i = 0; i < targets.Length; i++)
                        {
                            accessor.SetValue(targets[i], value);
                        }
                    }
                    break;
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/

        /// <summary>Indicates whether both properties refer to the same underlying field.</summary>
        public static bool AreSameProperty(SerializedProperty a, SerializedProperty b)
        {
            if (a == b)
                return true;

            if (a == null)
                return b == null;

            if (b == null)
                return false;

            if (a.propertyPath != b.propertyPath)
                return false;

            var aTargets = a.serializedObject.targetObjects;
            var bTargets = b.serializedObject.targetObjects;
            if (aTargets.Length != bTargets.Length)
                return false;

            for (int i = 0; i < aTargets.Length; i++)
            {
                if (aTargets[i] != bTargets[i])
                    return false;
            }

            return true;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Executes the `action` once with a new <see cref="SerializedProperty"/> for each of the
        /// <see cref="SerializedObject.targetObjects"/>. Or if there is only one target, it uses the `property`.
        /// </summary>
        public static void ForEachTarget(this SerializedProperty property, Action<SerializedProperty> function,
            string undoName = "Inspector")
        {
            var targets = property.serializedObject.targetObjects;

            if (undoName != null)
                Undo.RecordObjects(targets, undoName);

            if (targets.Length == 1)
            {
                function(property);
                property.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                var path = property.propertyPath;
                for (int i = 0; i < targets.Length; i++)
                {
                    using (var serializedObject = new SerializedObject(targets[i]))
                    {
                        property = serializedObject.FindProperty(path);
                        function(property);
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Adds a menu item to execute the specified `function` for each of the `property`s target objects.
        /// </summary>
        public static void AddPropertyModifierFunction(GenericMenu menu, SerializedProperty property, string label,
            MenuFunctionState state, Action<SerializedProperty> function)
        {
            if (state != MenuFunctionState.Disabled)
            {
                menu.AddItem(new GUIContent(label), state == MenuFunctionState.Selected, () =>
                {
                    ForEachTarget(property, function);
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(label));
            }
        }

        /// <summary>
        /// Adds a menu item to execute the specified `function` for each of the `property`s target objects.
        /// </summary>
        public static void AddPropertyModifierFunction(GenericMenu menu, SerializedProperty property, string label,
            bool enabled, Action<SerializedProperty> function)
        {
            var state = enabled ? MenuFunctionState.Normal : MenuFunctionState.Disabled;
            AddPropertyModifierFunction(menu, property, label, state, function);
        }

        /// <summary>
        /// Adds a menu item to execute the specified `function` for each of the `property`s target objects.
        /// </summary>
        public static void AddPropertyModifierFunction(GenericMenu menu, SerializedProperty property, string label,
            Action<SerializedProperty> function)
        {
            AddPropertyModifierFunction(menu, property, label, MenuFunctionState.Normal, function);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Calls the specified `method` for each of the underlying values of the `property` (in case it represents
        /// multiple selected objects) and records an undo step for any modifications made.
        /// </summary>
        public static void ModifyValues<T>(this SerializedProperty property, Action<T> method, string undoName = "Inspector")
        {
            RecordUndo(property, undoName);

            var values = GetValues<T>(property);
            for (int i = 0; i < values.Length; i++)
                method(values[i]);

            OnPropertyChanged(property);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Records the state of the specified `property` so it can be undone.
        /// </summary>
        public static void RecordUndo(this SerializedProperty property, string undoName = "Inspector")
            => Undo.RecordObjects(property.serializedObject.targetObjects, undoName);

        /************************************************************************************************************************/

        /// <summary>
        /// Updates the specified `property` and marks its target objects as dirty so any changes to a prefab will be saved.
        /// </summary>
        public static void OnPropertyChanged(this SerializedProperty property)
        {
            var targets = property.serializedObject.targetObjects;

            // If this change is made to a prefab, this makes sure that any instances in the scene will be updated.
            for (int i = 0; i < targets.Length; i++)
            {
                EditorUtility.SetDirty(targets[i]);
            }

            property.serializedObject.Update();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Returns the <see cref="SerializedPropertyType"/> that represents fields of the specified `type`.
        /// </summary>
        public static SerializedPropertyType GetPropertyType(Type type)
        {
            // Primitives.

            if (type == typeof(bool))
                return SerializedPropertyType.Boolean;

            if (type == typeof(int))
                return SerializedPropertyType.Integer;

            if (type == typeof(float))
                return SerializedPropertyType.Float;

            if (type == typeof(string))
                return SerializedPropertyType.String;

            if (type == typeof(LayerMask))
                return SerializedPropertyType.LayerMask;

            // Vectors.

            if (type == typeof(Vector2))
                return SerializedPropertyType.Vector2;
            if (type == typeof(Vector3))
                return SerializedPropertyType.Vector3;
            if (type == typeof(Vector4))
                return SerializedPropertyType.Vector4;

            if (type == typeof(Quaternion))
                return SerializedPropertyType.Quaternion;

            // Other.

            if (type == typeof(Color) || type == typeof(Color32))
                return SerializedPropertyType.Color;
            if (type == typeof(Gradient))
                return SerializedPropertyType.Gradient;

            if (type == typeof(Rect))
                return SerializedPropertyType.Rect;
            if (type == typeof(Bounds))
                return SerializedPropertyType.Bounds;

            if (type == typeof(AnimationCurve))
                return SerializedPropertyType.AnimationCurve;

            // Int Variants.

            if (type == typeof(Vector2Int))
                return SerializedPropertyType.Vector2Int;
            if (type == typeof(Vector3Int))
                return SerializedPropertyType.Vector3Int;
            if (type == typeof(RectInt))
                return SerializedPropertyType.RectInt;
            if (type == typeof(BoundsInt))
                return SerializedPropertyType.BoundsInt;

            // Special.

            if (typeof(Object).IsAssignableFrom(type))
                return SerializedPropertyType.ObjectReference;

            if (type.IsEnum)
                return SerializedPropertyType.Enum;

            return SerializedPropertyType.Generic;
        }

        /************************************************************************************************************************/

        /// <summary>Removes the specified array element from the `property`.</summary>
        /// <remarks>
        /// If the element is not at its default value, the first call to
        /// <see cref="SerializedProperty.DeleteArrayElementAtIndex"/> will only reset it, so this method will
        /// call it again if necessary to ensure that it actually gets removed.
        /// </remarks>
        public static void RemoveArrayElement(SerializedProperty property, int index)
        {
            var count = property.arraySize;
            property.DeleteArrayElementAtIndex(index);
            if (property.arraySize == count)
                property.DeleteArrayElementAtIndex(index);
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region Accessor Pool
        /************************************************************************************************************************/

        private static readonly Dictionary<Type, Dictionary<string, PropertyAccessor>>
            TypeToPathToAccessor = new Dictionary<Type, Dictionary<string, PropertyAccessor>>();

        /************************************************************************************************************************/

        /// <summary>
        /// Returns an <see cref="PropertyAccessor"/> that can be used to access the details of the specified `property`.
        /// </summary>
        public static PropertyAccessor GetAccessor(this SerializedProperty property)
        {
            var type = property.serializedObject.targetObject.GetType();
            return GetAccessor(property.propertyPath, ref type);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Returns an <see cref="PropertyAccessor"/> for a <see cref="SerializedProperty"/> with the specified `propertyPath`
        /// on the specified `type` of object.
        /// </summary>
        private static PropertyAccessor GetAccessor(string propertyPath, ref Type type)
        {
            if (!TypeToPathToAccessor.TryGetValue(type, out var pathToAccessor))
            {
                pathToAccessor = new Dictionary<string, PropertyAccessor>();
                TypeToPathToAccessor.Add(type, pathToAccessor);
            }

            if (!pathToAccessor.TryGetValue(propertyPath, out var accessor))
            {
                var nameStartIndex = propertyPath.LastIndexOf('.');
                string elementName;
                PropertyAccessor parent;

                // Array.
                if (nameStartIndex > 6 &&
                    nameStartIndex < propertyPath.Length - 7 &&
                    string.Compare(propertyPath, nameStartIndex - 6, ArrayDataPrefix, 0, 12) == 0)
                {
                    var index = int.Parse(propertyPath.Substring(nameStartIndex + 6, propertyPath.Length - nameStartIndex - 7));

                    var nameEndIndex = nameStartIndex - 6;
                    nameStartIndex = propertyPath.LastIndexOf('.', nameEndIndex - 1);

                    elementName = propertyPath.Substring(nameStartIndex + 1, nameEndIndex - nameStartIndex - 1);

                    FieldInfo field;
                    if (nameStartIndex >= 0)
                    {
                        parent = GetAccessor(propertyPath.Substring(0, nameStartIndex), ref type);
                        field = GetField(parent != null ? parent.FieldType : type, elementName);
                    }
                    else
                    {
                        parent = null;
                        field = GetField(type, elementName);
                    }

                    if (field != null)
                        accessor = new ArrayPropertyAccessor(parent, field, index);
                    else
                        accessor = null;
                }
                else// Single.
                {
                    if (nameStartIndex >= 0)
                    {
                        elementName = propertyPath.Substring(nameStartIndex + 1);
                        parent = GetAccessor(propertyPath.Substring(0, nameStartIndex), ref type);
                    }
                    else
                    {
                        elementName = propertyPath;
                        parent = null;
                    }

                    var field = GetField(parent != null ? parent.FieldType : type, elementName);

                    if (field != null)
                        accessor = new PropertyAccessor(parent, field);
                    else
                        accessor = null;
                }

                pathToAccessor.Add(propertyPath, accessor);
            }

            if (accessor != null)
                type = accessor.Field.FieldType;

            return accessor;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Returns a field with the specified `name` in the `declaringType` or any of its base types.
        /// </summary>
        private static FieldInfo GetField(Type declaringType, string name)
        {
            while (true)
            {
                var field = declaringType.GetField(name, AnimancerEditorUtilities.InstanceBindings);
                if (field != null)
                    return field;

                declaringType = declaringType.BaseType;
                if (declaringType == null)
                    return null;
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region PropertyAccessor
        /************************************************************************************************************************/

        /// <summary>[Editor-Only]
        /// A wrapper for accessing the underlying values and fields of a <see cref="SerializedProperty"/>.
        /// </summary>
        public class PropertyAccessor
        {
            /************************************************************************************************************************/

            /// <summary>The accessor for the field which this accessor is nested inside.</summary>
            public readonly PropertyAccessor Parent;

            /// <summary>The field wrapped by this accessor.</summary>
            public readonly FieldInfo Field;

            /// <summary>The type of the wrapped <see cref="Field"/>.</summary>
            public readonly Type FieldType;

            /************************************************************************************************************************/

            /// <summary>[Internal] Creates a new <see cref="PropertyAccessor"/>.</summary>
            internal PropertyAccessor(PropertyAccessor parent, FieldInfo field)
                : this(parent, field, field.FieldType)
            { }

            /// <summary>Creates a new <see cref="PropertyAccessor"/>.</summary>
            protected PropertyAccessor(PropertyAccessor parent, FieldInfo field, Type fieldType)
            {
                Parent = parent;
                Field = field;
                FieldType = fieldType;
            }

            /************************************************************************************************************************/

            /// <summary>
            /// Gets the value of the from the <see cref="Parent"/> (if there is one), then uses it to get and return
            /// the value of the <see cref="Field"/>.
            /// </summary>
            public virtual object GetValue(object obj)
            {
                if (Parent != null)
                    obj = Parent.GetValue(obj);

                if (ReferenceEquals(obj, null))
                    return null;

                return Field.GetValue(obj);
            }

            /// <summary>
            /// Gets the value of the from the <see cref="Parent"/> (if there is one), then uses it to get and return
            /// the value of the <see cref="Field"/>.
            /// </summary>
            public object GetValue(SerializedObject serializedObject)
                => serializedObject != null ? GetValue(serializedObject.targetObject) : null;

            /// <summary>
            /// Gets the value of the from the <see cref="Parent"/> (if there is one), then uses it to get and return
            /// the value of the <see cref="Field"/>.
            /// </summary>
            public object GetValue(SerializedProperty serializedProperty)
                => serializedProperty != null ? GetValue(serializedProperty.serializedObject) : null;

            /************************************************************************************************************************/

            /// <summary>
            /// Gets the value of the from the <see cref="Parent"/> (if there is one), then uses it to set the value
            /// of the <see cref="Field"/>.
            /// </summary>
            public virtual void SetValue(object obj, object value)
            {
                if (Parent != null)
                    obj = Parent.GetValue(obj);

                if (obj is null)
                    return;

                Field.SetValue(obj, value);
            }

            /// <summary>
            /// Gets the value of the from the <see cref="Parent"/> (if there is one), then uses it to set the value
            /// of the <see cref="Field"/>.
            /// </summary>
            public void SetValue(SerializedObject serializedObject, object value)
            {
                if (serializedObject != null)
                    SetValue(serializedObject.targetObject, value);
            }

            /// <summary>
            /// Gets the value of the from the <see cref="Parent"/> (if there is one), then uses it to set the value
            /// of the <see cref="Field"/>.
            /// </summary>
            public void SetValue(SerializedProperty serializedProperty, object value)
            {
                if (serializedProperty != null)
                    SetValue(serializedProperty.serializedObject, value);
            }

            /************************************************************************************************************************/

            /// <summary>Returns a description of this accessor's path.</summary>
            public override string ToString()
            {
                if (Parent != null)
                    return $"{Parent}.{Field.Name}";
                else
                    return Field.Name;
            }

            /************************************************************************************************************************/

            /// <summary>Returns a this accessor's <see cref="SerializedProperty.propertyPath"/>.</summary>
            public virtual string GetPath()
            {
                if (Parent != null)
                    return $"{Parent.GetPath()}.{Field.Name}";
                else
                    return Field.Name;
            }

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
        #region ArrayPropertyAccessor
        /************************************************************************************************************************/

        /// <summary>[Editor-Only] A <see cref="PropertyAccessor"/> for a particular element in an array.</summary>
        public class ArrayPropertyAccessor : PropertyAccessor
        {
            /************************************************************************************************************************/

            /// <summary>The index of the array element this accessor targets.</summary>
            public readonly int ElementIndex;

            /************************************************************************************************************************/

            /// <summary>[Internal] Creates a new <see cref="ArrayPropertyAccessor"/>.</summary>
            internal ArrayPropertyAccessor(PropertyAccessor parent, FieldInfo field, int elementIndex)
                : base(parent, field, GetElementType(field.FieldType))
            {
                ElementIndex = elementIndex;
            }

            /************************************************************************************************************************/

            /// <summary>Returns the type of elements in the array.</summary>
            public static Type GetElementType(Type fieldType)
            {
                if (fieldType.IsArray)
                {
                    return fieldType.GetElementType();
                }
                else if (fieldType.IsGenericType)
                {
                    return fieldType.GetGenericArguments()[0];
                }
                else
                {
                    Debug.LogWarning($"{nameof(Serialization)}.{nameof(ArrayPropertyAccessor)}:" +
                        $" unable to determine element type for {fieldType}");
                    return fieldType;
                }
            }

            /************************************************************************************************************************/

            /// <inheritdoc/>
            public override object GetValue(object obj)
            {
                var collection = base.GetValue(obj);
                if (collection == null)
                    return null;

                var list = collection as IList;
                if (list != null)
                {
                    if (ElementIndex < list.Count)
                        return list[ElementIndex];
                    else
                        return null;
                }

                var enumerator = ((IEnumerable)collection).GetEnumerator();

                for (int i = 0; i < ElementIndex; i++)
                {
                    if (!enumerator.MoveNext())
                        return null;
                }

                return enumerator.Current;
            }

            /************************************************************************************************************************/

            /// <inheritdoc/>
            public override void SetValue(object obj, object value)
            {
                var collection = base.GetValue(obj);
                if (collection == null)
                    return;

                var list = collection as IList;
                if (list != null)
                {
                    if (ElementIndex < list.Count)
                        list[ElementIndex] = value;

                    return;
                }

                throw new InvalidOperationException($"{nameof(SetValue)} failed: {Field} doesn't implement {nameof(IList)}.");
            }

            /************************************************************************************************************************/

            /// <summary>Returns a description of this accessor's path.</summary>
            public override string ToString() => $"{base.ToString()}[{ElementIndex}]";

            /************************************************************************************************************************/

            /// <summary>Returns a this accessor's <see cref="SerializedProperty.propertyPath"/>.</summary>
            public override string GetPath() => $"{base.GetPath()}{ArrayDataPrefix}{ElementIndex}{ArrayDataSuffix}";

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

#endif
