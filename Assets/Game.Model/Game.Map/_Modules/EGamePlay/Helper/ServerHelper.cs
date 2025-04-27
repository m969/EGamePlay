using System;
using System.Diagnostics;
using Sirenix.OdinInspector;

#if EGAMEPLAY_ET

#else
public class JsonIgnoreAttribute : Attribute
{

}
#endif

#if NOT_UNITY
namespace UnityEngine
{
    public class Time
    {
        public static long FrameEndTime;
        public static long FrameTime;
        public static float deltaTime { get; set; } = FrameTime / 1000f;

        public static long GameTime;
        public static long DeltaTime;
        public static float unscaledDeltaTime => deltaTime;
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class TooltipAttribute : Attribute
    {
        public TooltipAttribute(string tip) { }
    }
}

namespace Unity.Plastic.Newtonsoft.Json
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class JsonIgnoreAttribute : Attribute
    {

    }
}

namespace Sirenix.OdinInspector
{
    public class SerializedMonoBehaviour
    {
    }

    [Conditional("UNITY_EDITOR")]
    public class LabelTextAttribute : Attribute
    {
        public LabelTextAttribute(string labelName)
        {
            
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    [Conditional("UNITY_EDITOR")]
    public sealed class SuffixLabelAttribute : Attribute
    {
        /// <summary>The label displayed at the end of the property.</summary>
        public string Label;
        /// <summary>
        /// If <c>true</c> the suffix label will be drawn on top of the property, instead of after.
        /// </summary>
        public bool Overlay;

        /// <summary>Draws a label at the end of the property.</summary>
        /// <param name="label">The text of the label.</param>
        /// <param name="overlay">If <c>true</c> the suffix label will be drawn on top of the property, instead of after.</param>
        public SuffixLabelAttribute(string label, bool overlay = false)
        {
            this.Label = label;
            this.Overlay = overlay;
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public class DelayedPropertyAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class ShowIfAttribute : Attribute
    {
        /// <summary>
        /// Name of a bool field, property or function to show or hide the property.
        /// </summary>
        public string MemberName;
        /// <summary>
        /// Whether or not to slide the property in and out when the state changes.
        /// </summary>
        public bool Animate;
        /// <summary>The optional member value.</summary>
        public object Value;

        /// <summary>
        /// Shows a property in the inspector, if the specified member returns true.
        /// </summary>
        /// <param name="memberName">Name of a bool field, property or function to show or hide the property.</param>
        /// <param name="animate">Whether or not to slide the property in and out when the state changes.</param>
        public ShowIfAttribute(string memberName, bool animate = true)
        {
            this.MemberName = memberName;
            this.Animate = animate;
        }

        /// <summary>
        /// Shows a property in the inspector, if the specified member returns the specified value.
        /// </summary>
        /// <param name="memberName">Name of a bool field, property or method to test the value of.</param>
        /// <param name="optionalValue">The value the member should equal for the property to shown.</param>
        /// <param name="animate">Whether or not to slide the property in and out when the state changes.</param>
        public ShowIfAttribute(string memberName, object optionalValue, bool animate = true)
        {
            this.MemberName = memberName;
            this.Value = optionalValue;
            this.Animate = animate;
        }
    }
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    [Conditional("UNITY_EDITOR")]
    public class ShowInInspectorAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class OnInspectorGUIAttribute : ShowInInspectorAttribute
    {
        /// <summary>
        /// The name of the method to be called before the property is drawn, if any.
        /// </summary>
        public string PrependMethodName;
        /// <summary>
        /// The name of the method to be called after the property is drawn, if any.
        /// </summary>
        public string AppendMethodName;

        /// <summary>Calls the function when the inspector is being drawn.</summary>
        public OnInspectorGUIAttribute()
        {
        }

        /// <summary>
        /// Adds callback to the specified method when the property is being drawn.
        /// </summary>
        /// <param name="methodName">The name of the member function.</param>
        /// <param name="append">If <c>true</c> the method will be called after the property has been drawn. Otherwise the method will be called before.</param>
        public OnInspectorGUIAttribute(string methodName, bool append = true)
        {
            if (append)
                this.AppendMethodName = methodName;
            else
                this.PrependMethodName = methodName;
        }

        /// <summary>
        /// Adds callback to the specified method when the property is being drawn.
        /// </summary>
        /// <param name="prependMethodName">The name of the member function to invoke before the property is drawn.</param>
        /// <param name="appendMethodName">The name of the member function to invoke after the property is drawn.</param>
        public OnInspectorGUIAttribute(string prependMethodName, string appendMethodName)
        {
            this.PrependMethodName = prependMethodName;
            this.AppendMethodName = appendMethodName;
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public class HideReferenceObjectPickerAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class ListDrawerSettingsAttribute : Attribute
    {
        public bool DefaultExpandedState;
        /// <summary>
        /// If true, the add button will not be rendered in the title toolbar. You can use OnTitleBarGUI to implement your own add button.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [hide add button]; otherwise, <c>false</c>.
        /// </value>
        public bool HideAddButton;
        /// <summary>
        /// If true, the remove button  will not be rendered on list items. You can use OnBeginListElementGUI and OnEndListElementGUI to implement your own remove button.
        /// </summary>
        /// <value>
        ///     <c>true</c> if [hide remove button]; otherwise, <c>false</c>.
        /// </value>
        public bool HideRemoveButton;
        /// <summary>
        /// Specify the name of a member inside each list element which defines the label being drawn for each list element.
        /// </summary>
        public string ListElementLabelName;
        /// <summary>
        /// Override the default behaviour for adding objects to the list.
        /// If the referenced member returns the list type element, it will be called once per selected object.
        /// If the referenced method returns void, it will only be called once regardless of how many objects are selected.
        /// </summary>
        public string CustomAddFunction;
        public string CustomRemoveIndexFunction;
        public string CustomRemoveElementFunction;
        /// <summary>
        /// Calls a method before each list element. The member referenced must have a return type of void, and an index parameter of type int which represents the element index being drawn.
        /// </summary>
        public string OnBeginListElementGUI;
        /// <summary>
        /// Calls a method after each list element. The member referenced must have a return type of void, and an index parameter of type int which represents the element index being drawn.
        /// </summary>
        public string OnEndListElementGUI;
        /// <summary>
        /// If true, object/type pickers will never be shown when the list add button is clicked, and default(T) will always be added instantly instead, where T is the element type of the list.
        /// </summary>
        public bool AlwaysAddDefaultValue;
        /// <summary>
        /// Whether adding a new element should copy the last element. False by default.
        /// </summary>
        public bool AddCopiesLastElement;
        private string onTitleBarGUI;
        private int numberOfItemsPerPage;
        private bool paging;
        private bool draggable;
        private bool isReadOnly;
        private bool showItemCount;
        private bool pagingHasValue;
        private bool draggableHasValue;
        private bool isReadOnlyHasValue;
        private bool showItemCountHasValue;
        private bool expanded;
        private bool expandedHasValue;
        private bool numberOfItemsPerPageHasValue;
        private bool showIndexLabels;
        private bool showIndexLabelsHasValue;

        /// <summary>
        /// Override the default setting specified in the Advanced Odin Preferences window and explicitly tell whether paging should be enabled or not.
        /// </summary>
        public bool ShowPaging
        {
            get
            {
                return this.paging;
            }
            set
            {
                this.paging = value;
                this.pagingHasValue = true;
            }
        }

        /// <summary>
        /// Override the default setting specified in the Advanced Odin Preferences window and explicitly tell whether items should be draggable or not.
        /// </summary>
        public bool DraggableItems
        {
            get
            {
                return this.draggable;
            }
            set
            {
                this.draggable = value;
                this.draggableHasValue = true;
            }
        }

        /// <summary>
        /// Override the default setting specified in the Advanced Odin Preferences window and explicitly tells how many items each page should contain.
        /// </summary>
        public int NumberOfItemsPerPage
        {
            get
            {
                return this.numberOfItemsPerPage;
            }
            set
            {
                this.numberOfItemsPerPage = value;
                this.numberOfItemsPerPageHasValue = true;
            }
        }

        /// <summary>
        /// Mark a list as read-only. This removes all editing capabilities from the list such as Add, Drag and delete,
        /// but without disabling GUI for each element drawn as otherwise would be the case if the <see cref="T:Sirenix.OdinInspector.ReadOnlyAttribute" /> was used.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return this.isReadOnly;
            }
            set
            {
                this.isReadOnly = value;
                this.isReadOnlyHasValue = true;
            }
        }

        /// <summary>
        /// Override the default setting specified in the Advanced Odin Preferences window and explicitly tell whether or not item count should be shown.
        /// </summary>
        public bool ShowItemCount
        {
            get
            {
                return this.showItemCount;
            }
            set
            {
                this.showItemCount = value;
                this.showItemCountHasValue = true;
            }
        }

        /// <summary>
        /// Override the default setting specified in the Advanced Odin Preferences window and explicitly tell whether or not the list should be expanded or collapsed by default.
        /// </summary>
        public bool Expanded
        {
            get
            {
                return this.expanded;
            }
            set
            {
                this.expanded = value;
                this.expandedHasValue = true;
            }
        }

        /// <summary>
        /// If true, a label is drawn for each element which shows the index of the element.
        /// </summary>
        public bool ShowIndexLabels
        {
            get
            {
                return this.showIndexLabels;
            }
            set
            {
                this.showIndexLabels = value;
                this.showIndexLabelsHasValue = true;
            }
        }

        /// <summary>
        /// Use this to inject custom GUI into the title-bar of the list.
        /// </summary>
        public string OnTitleBarGUI
        {
            get
            {
                return this.onTitleBarGUI;
            }
            set
            {
                this.onTitleBarGUI = value;
            }
        }

        /// <summary>Whether the Paging property is set.</summary>
        public bool PagingHasValue
        {
            get
            {
                return this.pagingHasValue;
            }
        }

        /// <summary>Whether the ShowItemCount property is set.</summary>
        public bool ShowItemCountHasValue
        {
            get
            {
                return this.showItemCountHasValue;
            }
        }

        /// <summary>Whether the NumberOfItemsPerPage property is set.</summary>
        public bool NumberOfItemsPerPageHasValue
        {
            get
            {
                return this.numberOfItemsPerPageHasValue;
            }
        }

        /// <summary>Whether the Draggable property is set.</summary>
        public bool DraggableHasValue
        {
            get
            {
                return this.draggableHasValue;
            }
        }

        /// <summary>Whether the IsReadOnly property is set.</summary>
        public bool IsReadOnlyHasValue
        {
            get
            {
                return this.isReadOnlyHasValue;
            }
        }

        /// <summary>Whether the Expanded property is set.</summary>
        public bool ExpandedHasValue
        {
            get
            {
                return this.expandedHasValue;
            }
        }

        /// <summary>Whether the ShowIndexLabels property is set.</summary>
        public bool ShowIndexLabelsHasValue
        {
            get
            {
                return this.showIndexLabelsHasValue;
            }
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public abstract class PropertyGroupAttribute : Attribute
    {
        /// <summary>The ID used to grouping properties together.</summary>
        public string GroupID;
        /// <summary>
        /// The name of the group. This is the last part of the group ID if there is a path, otherwise it is just the group ID.
        /// </summary>
        public string GroupName;
        /// <summary>The order of the group.</summary>
        public int Order;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sirenix.OdinInspector.PropertyGroupAttribute" /> class.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="order">The group order.</param>
        public PropertyGroupAttribute(string groupId, int order)
        {
            this.GroupID = groupId;
            this.Order = order;
            int num = groupId.LastIndexOf('/');
            this.GroupName = num < 0 || num >= groupId.Length ? groupId : groupId.Substring(num + 1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sirenix.OdinInspector.PropertyGroupAttribute" /> class.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        public PropertyGroupAttribute(string groupId)
          : this(groupId, 0)
        {
        }

        /// <summary>
        /// <para>Combines this attribute with another attribute of the same type.
        /// This method invokes the virtual <see cref="M:Sirenix.OdinInspector.PropertyGroupAttribute.CombineValuesWith(Sirenix.OdinInspector.PropertyGroupAttribute)" /> method to invoke custom combine logic.</para>
        /// <para>All group attributes are combined to one attribute used by a single OdinGroupDrawer.</para>
        /// <para>Example: <code>protected override void CombineValuesWith(PropertyGroupAttribute other) { this.Title = this.Title ?? (other as MyGroupAttribute).Title; }</code></para>
        /// </summary>
        /// <param name="other">The attribute to combine with.</param>
        /// <returns>The instance that the method was invoked on.</returns>
        /// <exception cref="T:System.ArgumentNullException">The argument 'other' was null.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// Attributes to combine are not of the same type.
        /// or
        /// PropertyGroupAttributes to combine must have the same group id.
        /// </exception>
        public PropertyGroupAttribute Combine(PropertyGroupAttribute other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            if (other.GetType() != this.GetType())
                throw new ArgumentException("Attributes to combine are not of the same type.");
            if (other.GroupID != this.GroupID)
                throw new ArgumentException("PropertyGroupAttributes to combine must have the same group id.");
            if (this.Order == 0)
                this.Order = other.Order;
            else if (other.Order != 0)
                this.Order = Math.Min(this.Order, other.Order);
            this.CombineValuesWith(other);
            return this;
        }

        /// <summary>
        /// <para>Override this method to add custom combine logic to your group attribute. This method determines how your group's parameters combine when spread across multiple attribute declarations in the same class.</para>
        /// <para>Remember, in .NET, member order is not guaranteed, so you never know which order your attributes will be combined in.</para>
        /// </summary>
        /// <param name="other">The attribute to combine with. This parameter is guaranteed to be of the correct attribute type.</param>
        /// <example>
        /// <para>This example shows how <see cref="T:Sirenix.OdinInspector.BoxGroupAttribute" /> attributes are combined.</para>
        /// <code>
        /// protected override void CombineValuesWith(PropertyGroupAttribute other)
        /// {
        ///     // The given attribute parameter is *guaranteed* to be of type BoxGroupAttribute.
        ///     var attr = other as BoxGroupAttribute;
        /// 
        ///     // If this attribute has no label, we the other group's label, thus preserving the label across combines.
        ///     if (this.Label == null)
        ///     {
        ///         this.Label = attr.Label;
        ///     }
        /// 
        ///     // Combine ShowLabel and CenterLabel parameters.
        ///     this.ShowLabel |= attr.ShowLabel;
        ///     this.CenterLabel |= attr.CenterLabel;
        /// }
        /// </code>
        /// </example>
        protected virtual void CombineValuesWith(PropertyGroupAttribute other)
        {
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class ToggleGroupAttribute : PropertyGroupAttribute
    {
        /// <summary>
        /// Title of the toggle group in the inspector.
        /// If <c>null</c> <see cref="P:Sirenix.OdinInspector.ToggleGroupAttribute.ToggleMemberName" /> will be used instead.
        /// </summary>
        public string ToggleGroupTitle;
        /// <summary>
        /// If true, all other open toggle groups will collapse once another one opens.
        /// </summary>
        public bool CollapseOthersOnExpand;

        /// <summary>
        /// Creates a ToggleGroup. See <see cref="T:Sirenix.OdinInspector.ToggleGroupAttribute" />.
        /// </summary>
        /// <param name="toggleMemberName">Name of any bool field or property to enable or disable the ToggleGroup.</param>
        /// <param name="order">The order of the group.</param>
        /// <param name="groupTitle">Use this to name the group differently than toggleMemberName.</param>
        public ToggleGroupAttribute(string toggleMemberName, int order = 0, string groupTitle = null)
          : base(toggleMemberName, order)
        {
            this.ToggleGroupTitle = groupTitle;
            this.CollapseOthersOnExpand = true;
        }

        /// <summary>
        /// Creates a ToggleGroup. See <see cref="T:Sirenix.OdinInspector.ToggleGroupAttribute" />.
        /// </summary>
        /// <param name="toggleMemberName">Name of any bool field or property to enable or disable the ToggleGroup.</param>
        /// <param name="groupTitle">Use this to name the group differently than toggleMemberName.</param>
        public ToggleGroupAttribute(string toggleMemberName, string groupTitle)
          : this(toggleMemberName, 0, groupTitle)
        {
        }

        /// <summary>Obsolete constructor overload.</summary>
        /// <param name="toggleMemberName">Obsolete overload.</param>
        /// <param name="order">Obsolete overload.</param>
        /// <param name="groupTitle">Obsolete overload.</param>
        /// <param name="titleStringMemberName">Obsolete overload.</param>
        [Obsolete("Use [ToggleGroup(\"toggleMemberName\", groupTitle: \"$titleStringMemberName\")] instead")]
        public ToggleGroupAttribute(
          string toggleMemberName,
          int order,
          string groupTitle,
          string titleStringMemberName)
          : base(toggleMemberName, order)
        {
            this.ToggleGroupTitle = groupTitle;
            this.CollapseOthersOnExpand = true;
        }

        /// <summary>
        /// Name of any bool field, property or function to enable or disable the ToggleGroup.
        /// </summary>
        public string ToggleMemberName
        {
            get
            {
                return this.GroupName;
            }
        }

        /// <summary>
        /// Name of any string field, property or function, to title the toggle group in the inspector.
        /// If <c>null</c> <see cref="F:Sirenix.OdinInspector.ToggleGroupAttribute.ToggleGroupTitle" /> will be used instead.
        /// </summary>
        [Obsolete("Add a $ infront of group title instead, i.e: \"$MyStringMember\".")]
        public string TitleStringMemberName { get; set; }

        /// <summary>Combines the ToggleGroup with another ToggleGroup.</summary>
        /// <param name="other">Another ToggleGroup.</param>
        protected override void CombineValuesWith(PropertyGroupAttribute other)
        {
            ToggleGroupAttribute toggleGroupAttribute = other as ToggleGroupAttribute;
            if (this.ToggleGroupTitle == null)
                this.ToggleGroupTitle = toggleGroupAttribute.ToggleGroupTitle;
            else if (toggleGroupAttribute.ToggleGroupTitle == null)
                toggleGroupAttribute.ToggleGroupTitle = this.ToggleGroupTitle;
            this.CollapseOthersOnExpand = this.CollapseOthersOnExpand || toggleGroupAttribute.CollapseOthersOnExpand;
            toggleGroupAttribute.CollapseOthersOnExpand = this.CollapseOthersOnExpand;
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public class HorizontalGroupAttribute : PropertyGroupAttribute
    {
        /// <summary>
        /// The width. Values between 0 and 1 will be treated as percentage, 0 = auto, otherwise pixels.
        /// </summary>
        public float Width;
        /// <summary>
        /// The margin left. Values between 0 and 1 will be treated as percentage, 0 = ignore, otherwise pixels.
        /// </summary>
        public float MarginLeft;
        /// <summary>
        /// The margin right. Values between 0 and 1 will be treated as percentage, 0 = ignore, otherwise pixels.
        /// </summary>
        public float MarginRight;
        /// <summary>
        /// The padding left. Values between 0 and 1 will be treated as percentage, 0 = ignore, otherwise pixels.
        /// </summary>
        public float PaddingLeft;
        /// <summary>
        /// The padding right. Values between 0 and 1 will be treated as percentage, 0 = ignore, otherwise pixels.
        /// </summary>
        public float PaddingRight;
        /// <summary>
        /// The minimum Width. Values between 0 and 1 will be treated as percentage, 0 = ignore, otherwise pixels.
        /// </summary>
        public float MinWidth;
        /// <summary>
        /// The maximum Width. Values between 0 and 1 will be treated as percentage, 0 = ignore, otherwise pixels.
        /// </summary>
        public float MaxWidth;
        /// <summary>Adds a title above the horizontal group.</summary>
        public string Title;
        /// <summary>The label width, 0 = auto.</summary>
        public float LabelWidth;

        /// <summary>Organizes the property in a horizontal group.</summary>
        /// <param name="group">The group for the property.</param>
        /// <param name="width">The width of the property. Values between 0 and 1 are interpolated as a percentage, otherwise pixels.</param>
        /// <param name="marginLeft">The left margin in pixels.</param>
        /// <param name="marginRight">The right margin in pixels.</param>
        /// <param name="order">The order of the group in the inspector.</param>
        public HorizontalGroupAttribute(
          string group,
          float width = 0.0f,
          int marginLeft = 0,
          int marginRight = 0,
          int order = 0)
          : base(group, order)
        {
            this.Width = width;
            this.MarginLeft = (float)marginLeft;
            this.MarginRight = (float)marginRight;
        }

        /// <summary>Organizes the property in a horizontal group.</summary>
        /// <param name="width">The width of the property. Values between 0 and 1 are interpolated as a percentage, otherwise pixels.</param>
        /// <param name="marginLeft">The left margin in pixels.</param>
        /// <param name="marginRight">The right margin in pixels.</param>
        /// <param name="order">The order of the group in the inspector.</param>
        public HorizontalGroupAttribute(float width = 0.0f, int marginLeft = 0, int marginRight = 0, int order = 0)
          : this("_DefaultHorizontalGroup", width, marginLeft, marginRight, order)
        {
        }

        /// <summary>Merges the values of two group attributes together.</summary>
        /// <param name="other">The other group to combine with.</param>
        protected override void CombineValuesWith(PropertyGroupAttribute other)
        {
            this.Title = this.Title ?? (other as HorizontalGroupAttribute).Title;
            this.LabelWidth = Math.Max(this.LabelWidth, (other as HorizontalGroupAttribute).LabelWidth);
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    [Conditional("UNITY_EDITOR")]
    public class HideLabelAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class OnValueChangedAttribute : Attribute
    {
        /// <summary>Name of callback member function.</summary>
        public string MethodName;
        /// <summary>
        /// Whether to invoke the method when a child value of the property is changed.
        /// </summary>
        public bool IncludeChildren;

        /// <summary>
        /// Adds a callback for when the property's value is changed.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="includeChildren">Whether to invoke the method when a child value of the property is changed.</param>
        public OnValueChangedAttribute(string methodName, bool includeChildren = false)
        {
            this.MethodName = methodName;
            this.IncludeChildren = includeChildren;
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public class ValueDropdownAttribute : Attribute
    {
        /// <summary>
        /// Name of any field, property or method member that implements IList. E.g. arrays or Lists.
        /// </summary>
        public string MemberName;
        /// <summary>
        /// The number of items before enabling search. Default is 10.
        /// </summary>
        public int NumberOfItemsBeforeEnablingSearch;
        /// <summary>False by default.</summary>
        public bool IsUniqueList;
        /// <summary>
        /// True by default. If the ValueDropdown attribute is applied to a list, then disabling this,
        /// will render all child elements normally without using the ValueDropdown. The ValueDropdown will
        /// still show up when you click the add button on the list drawer, unless <see cref="F:Sirenix.OdinInspector.ValueDropdownAttribute.DisableListAddButtonBehaviour" /> is true.
        /// </summary>
        public bool DrawDropdownForListElements;
        /// <summary>False by default.</summary>
        public bool DisableListAddButtonBehaviour;
        /// <summary>
        /// If the ValueDropdown attribute is applied to a list, and <see cref="F:Sirenix.OdinInspector.ValueDropdownAttribute.IsUniqueList" /> is set to true, then enabling this,
        /// will exclude existing values, instead of rendering a checkbox indicating whether the item is already included or not.
        /// </summary>
        public bool ExcludeExistingValuesInList;
        /// <summary>
        /// If the dropdown renders a tree-view, then setting this to true will ensure everything is expanded by default.
        /// </summary>
        public bool ExpandAllMenuItems;
        /// <summary>
        /// If true, instead of replacing the drawer with a wide dropdown-field, the dropdown button will be a little button, drawn next to the other drawer.
        /// </summary>
        public bool AppendNextDrawer;
        /// <summary>
        /// Disables the the GUI for the appended drawer. False by default.
        /// </summary>
        public bool DisableGUIInAppendedDrawer;
        /// <summary>
        /// By default, a single click selects and confirms the selection.
        /// </summary>
        public bool DoubleClickToConfirm;
        /// <summary>By default, the dropdown will create a tree view.</summary>
        public bool FlattenTreeView;
        /// <summary>
        /// Gets or sets the width of the dropdown. Default is zero.
        /// </summary>
        public int DropdownWidth;
        /// <summary>
        /// Gets or sets the height of the dropdown. Default is zero.
        /// </summary>
        public int DropdownHeight;
        /// <summary>
        /// Gets or sets the title for the dropdown. Null by default.
        /// </summary>
        public string DropdownTitle;
        /// <summary>False by default.</summary>
        public bool SortDropdownItems;
        /// <summary>Whether to draw all child properties in a foldout.</summary>
        public bool HideChildProperties;

        /// <summary>Creates a dropdown menu for a property.</summary>
        /// <param name="memberName">Name of any field, property or method member that implements IList. E.g. arrays or Lists.</param>
        public ValueDropdownAttribute(string memberName)
        {
            this.NumberOfItemsBeforeEnablingSearch = 10;
            this.MemberName = memberName;
            this.DrawDropdownForListElements = true;
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public sealed class HideIfAttribute : Attribute
    {
        /// <summary>
        /// Name of a bool field, property or function to show or hide the property.
        /// </summary>
        public string MemberName;
        /// <summary>The optional member value.</summary>
        public object Value;
        /// <summary>
        /// Whether or not to slide the property in and out when the state changes.
        /// </summary>
        public bool Animate;

        /// <summary>
        /// Hides a property in the inspector, if the specified member returns true.
        /// </summary>
        /// <param name="memberName">Name of a bool field, property or function to show or hide the property.</param>
        /// <param name="animate">Whether or not to slide the property in and out when the state changes.</param>
        public HideIfAttribute(string memberName, bool animate = true)
        {
            this.MemberName = memberName;
            this.Animate = animate;
        }

        /// <summary>
        /// Hides a property in the inspector, if the specified member returns the specified value.
        /// </summary>
        /// <param name="memberName">Name of member to check the value of.</param>
        /// <param name="optionalValue">The value to check for.</param>
        /// <param name="animate">Whether or not to slide the property in and out when the state changes.</param>
        public HideIfAttribute(string memberName, object optionalValue, bool animate = true)
        {
            this.MemberName = memberName;
            this.Value = optionalValue;
            this.Animate = animate;
        }
    }
}

namespace UnityEngine
{
    public class GameObject
    {
        public bool activeSelf { get; set; }
    }

    public class Transform
    {

    }

    public class Animation
    {

    }

    public class AnimationClip
    {

    }

    public class Resources
    {
        public static T Load<T>(string path) where T : class
        {
            return default(T);
        }
    }

    public sealed class SerializeField : Attribute
    {

    }

    /// <summary>
    ///   <para>Mark a ScriptableObject-derived type to be automatically listed in the Assets/Create submenu, so that instances of the type can be easily created and stored in the project as ".asset" files.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [Conditional("UNITY_EDITOR")]
    public sealed class CreateAssetMenuAttribute : Attribute
    {
        /// <summary>
        ///   <para>The display name for this type shown in the Assets/Create menu.</para>
        /// </summary>
        public string menuName { get; set; }

        /// <summary>
        ///   <para>The default file name used by newly created instances of this type.</para>
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        ///   <para>The position of the menu item within the Assets/Create menu.</para>
        /// </summary>
        public int order { get; set; }
    }

    /// <summary>
    ///   <para>Makes a variable not show up in the inspector but be serialized.</para>
    /// </summary>
    public sealed class HideInInspector : Attribute
    {
    }

    public sealed class ReadOnlyAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class FoldoutGroupAttribute : PropertyAttribute
    {
        public readonly string tooltip;
        public string GroupName;

        public FoldoutGroupAttribute(string tooltip)
        {
            this.tooltip = tooltip;
        }
    }

    /// <summary>
    ///   <para>Base class to derive custom property attributes from. Use this to create custom attributes for script variables.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public abstract class PropertyAttribute : Attribute
    {
        /// <summary>
        ///   <para>Optional field to specify the order that multiple DecorationDrawers should be drawn in.</para>
        /// </summary>
        public int order { get; set; }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class PropertyOrderAttribute : Attribute
    {
        /// <summary>
        ///   <para>Optional field to specify the order that multiple DecorationDrawers should be drawn in.</para>
        /// </summary>
        public int order { get; set; }

        public PropertyOrderAttribute(int order)
        {
            this.order = order;
        }
    }

    /// <summary>
    ///   <para>Attribute used to make a float or int variable in a script be restricted to a specific range.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class RangeAttribute : PropertyAttribute
    {
        public readonly float min;
        public readonly float max;

        /// <summary>
        ///   <para>Attribute used to make a float or int variable in a script be restricted to a specific range.</para>
        /// </summary>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        public RangeAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }

    /// <summary>
    ///   <para>Use this PropertyAttribute to add some spacing in the Inspector.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class SpaceAttribute : PropertyAttribute
    {
        /// <summary>
        ///   <para>The spacing in pixels.</para>
        /// </summary>
        public readonly float height;

        public SpaceAttribute()
        {
            this.height = 8f;
        }

        /// <summary>
        ///   <para>Use this DecoratorDrawer to add some spacing in the Inspector.</para>
        /// </summary>
        /// <param name="height">The spacing in pixels.</param>
        public SpaceAttribute(float height)
        {
            this.height = height;
        }
    }

    public enum KeyCode
    {
        /// <summary>
        ///   <para>Not assigned (never returned as the result of a keystroke).</para>
        /// </summary>
        None = 0,
        /// <summary>
        ///   <para>The backspace key.</para>
        /// </summary>
        Backspace = 8,
        /// <summary>
        ///   <para>The tab key.</para>
        /// </summary>
        Tab = 9,
        /// <summary>
        ///   <para>The Clear key.</para>
        /// </summary>
        Clear = 12, // 0x0000000C
        /// <summary>
        ///   <para>Return key.</para>
        /// </summary>
        Return = 13, // 0x0000000D
        /// <summary>
        ///   <para>Pause on PC machines.</para>
        /// </summary>
        Pause = 19, // 0x00000013
        /// <summary>
        ///   <para>Escape key.</para>
        /// </summary>
        Escape = 27, // 0x0000001B
        /// <summary>
        ///   <para>Space key.</para>
        /// </summary>
        Space = 32, // 0x00000020
        /// <summary>
        ///   <para>Exclamation mark key '!'.</para>
        /// </summary>
        Exclaim = 33, // 0x00000021
        /// <summary>
        ///   <para>Double quote key '"'.</para>
        /// </summary>
        DoubleQuote = 34, // 0x00000022
        /// <summary>
        ///   <para>Hash key '#'.</para>
        /// </summary>
        Hash = 35, // 0x00000023
        /// <summary>
        ///   <para>Dollar sign key '$'.</para>
        /// </summary>
        Dollar = 36, // 0x00000024
        /// <summary>
        ///   <para>Percent '%' key.</para>
        /// </summary>
        Percent = 37, // 0x00000025
        /// <summary>
        ///   <para>Ampersand key '&amp;'.</para>
        /// </summary>
        Ampersand = 38, // 0x00000026
        /// <summary>
        ///   <para>Quote key '.</para>
        /// </summary>
        Quote = 39, // 0x00000027
        /// <summary>
        ///   <para>Left Parenthesis key '('.</para>
        /// </summary>
        LeftParen = 40, // 0x00000028
        /// <summary>
        ///   <para>Right Parenthesis key ')'.</para>
        /// </summary>
        RightParen = 41, // 0x00000029
        /// <summary>
        ///   <para>Asterisk key '*'.</para>
        /// </summary>
        Asterisk = 42, // 0x0000002A
        /// <summary>
        ///   <para>Plus key '+'.</para>
        /// </summary>
        Plus = 43, // 0x0000002B
        /// <summary>
        ///   <para>Comma ',' key.</para>
        /// </summary>
        Comma = 44, // 0x0000002C
        /// <summary>
        ///   <para>Minus '-' key.</para>
        /// </summary>
        Minus = 45, // 0x0000002D
        /// <summary>
        ///   <para>Period '.' key.</para>
        /// </summary>
        Period = 46, // 0x0000002E
        /// <summary>
        ///   <para>Slash '/' key.</para>
        /// </summary>
        Slash = 47, // 0x0000002F
        /// <summary>
        ///   <para>The '0' key on the top of the alphanumeric keyboard.</para>
        /// </summary>
        Alpha0 = 48, // 0x00000030
        /// <summary>
        ///   <para>The '1' key on the top of the alphanumeric keyboard.</para>
        /// </summary>
        Alpha1 = 49, // 0x00000031
        /// <summary>
        ///   <para>The '2' key on the top of the alphanumeric keyboard.</para>
        /// </summary>
        Alpha2 = 50, // 0x00000032
        /// <summary>
        ///   <para>The '3' key on the top of the alphanumeric keyboard.</para>
        /// </summary>
        Alpha3 = 51, // 0x00000033
        /// <summary>
        ///   <para>The '4' key on the top of the alphanumeric keyboard.</para>
        /// </summary>
        Alpha4 = 52, // 0x00000034
        /// <summary>
        ///   <para>The '5' key on the top of the alphanumeric keyboard.</para>
        /// </summary>
        Alpha5 = 53, // 0x00000035
        /// <summary>
        ///   <para>The '6' key on the top of the alphanumeric keyboard.</para>
        /// </summary>
        Alpha6 = 54, // 0x00000036
        /// <summary>
        ///   <para>The '7' key on the top of the alphanumeric keyboard.</para>
        /// </summary>
        Alpha7 = 55, // 0x00000037
        /// <summary>
        ///   <para>The '8' key on the top of the alphanumeric keyboard.</para>
        /// </summary>
        Alpha8 = 56, // 0x00000038
        /// <summary>
        ///   <para>The '9' key on the top of the alphanumeric keyboard.</para>
        /// </summary>
        Alpha9 = 57, // 0x00000039
        /// <summary>
        ///   <para>Colon ':' key.</para>
        /// </summary>
        Colon = 58, // 0x0000003A
        /// <summary>
        ///   <para>Semicolon ';' key.</para>
        /// </summary>
        Semicolon = 59, // 0x0000003B
        /// <summary>
        ///   <para>Less than '&lt;' key.</para>
        /// </summary>
        Less = 60, // 0x0000003C
        /// <summary>
        ///   <para>Equals '=' key.</para>
        /// </summary>
        Equals = 61, // 0x0000003D
        /// <summary>
        ///   <para>Greater than '&gt;' key.</para>
        /// </summary>
        Greater = 62, // 0x0000003E
        /// <summary>
        ///   <para>Question mark '?' key.</para>
        /// </summary>
        Question = 63, // 0x0000003F
        /// <summary>
        ///   <para>At key '@'.</para>
        /// </summary>
        At = 64, // 0x00000040
        /// <summary>
        ///   <para>Left square bracket key '['.</para>
        /// </summary>
        LeftBracket = 91, // 0x0000005B
        /// <summary>
        ///   <para>Backslash key '\'.</para>
        /// </summary>
        Backslash = 92, // 0x0000005C
        /// <summary>
        ///   <para>Right square bracket key ']'.</para>
        /// </summary>
        RightBracket = 93, // 0x0000005D
        /// <summary>
        ///   <para>Caret key '^'.</para>
        /// </summary>
        Caret = 94, // 0x0000005E
        /// <summary>
        ///   <para>Underscore '_' key.</para>
        /// </summary>
        Underscore = 95, // 0x0000005F
        /// <summary>
        ///   <para>Back quote key '`'.</para>
        /// </summary>
        BackQuote = 96, // 0x00000060
        /// <summary>
        ///   <para>'a' key.</para>
        /// </summary>
        A = 97, // 0x00000061
        /// <summary>
        ///   <para>'b' key.</para>
        /// </summary>
        B = 98, // 0x00000062
        /// <summary>
        ///   <para>'c' key.</para>
        /// </summary>
        C = 99, // 0x00000063
        /// <summary>
        ///   <para>'d' key.</para>
        /// </summary>
        D = 100, // 0x00000064
        /// <summary>
        ///   <para>'e' key.</para>
        /// </summary>
        E = 101, // 0x00000065
        /// <summary>
        ///   <para>'f' key.</para>
        /// </summary>
        F = 102, // 0x00000066
        /// <summary>
        ///   <para>'g' key.</para>
        /// </summary>
        G = 103, // 0x00000067
        /// <summary>
        ///   <para>'h' key.</para>
        /// </summary>
        H = 104, // 0x00000068
        /// <summary>
        ///   <para>'i' key.</para>
        /// </summary>
        I = 105, // 0x00000069
        /// <summary>
        ///   <para>'j' key.</para>
        /// </summary>
        J = 106, // 0x0000006A
        /// <summary>
        ///   <para>'k' key.</para>
        /// </summary>
        K = 107, // 0x0000006B
        /// <summary>
        ///   <para>'l' key.</para>
        /// </summary>
        L = 108, // 0x0000006C
        /// <summary>
        ///   <para>'m' key.</para>
        /// </summary>
        M = 109, // 0x0000006D
        /// <summary>
        ///   <para>'n' key.</para>
        /// </summary>
        N = 110, // 0x0000006E
        /// <summary>
        ///   <para>'o' key.</para>
        /// </summary>
        O = 111, // 0x0000006F
        /// <summary>
        ///   <para>'p' key.</para>
        /// </summary>
        P = 112, // 0x00000070
        /// <summary>
        ///   <para>'q' key.</para>
        /// </summary>
        Q = 113, // 0x00000071
        /// <summary>
        ///   <para>'r' key.</para>
        /// </summary>
        R = 114, // 0x00000072
        /// <summary>
        ///   <para>'s' key.</para>
        /// </summary>
        S = 115, // 0x00000073
        /// <summary>
        ///   <para>'t' key.</para>
        /// </summary>
        T = 116, // 0x00000074
        /// <summary>
        ///   <para>'u' key.</para>
        /// </summary>
        U = 117, // 0x00000075
        /// <summary>
        ///   <para>'v' key.</para>
        /// </summary>
        V = 118, // 0x00000076
        /// <summary>
        ///   <para>'w' key.</para>
        /// </summary>
        W = 119, // 0x00000077
        /// <summary>
        ///   <para>'x' key.</para>
        /// </summary>
        X = 120, // 0x00000078
        /// <summary>
        ///   <para>'y' key.</para>
        /// </summary>
        Y = 121, // 0x00000079
        /// <summary>
        ///   <para>'z' key.</para>
        /// </summary>
        Z = 122, // 0x0000007A
        /// <summary>
        ///   <para>Left curly bracket key '{'.</para>
        /// </summary>
        LeftCurlyBracket = 123, // 0x0000007B
        /// <summary>
        ///   <para>Pipe '|' key.</para>
        /// </summary>
        Pipe = 124, // 0x0000007C
        /// <summary>
        ///   <para>Right curly bracket key '}'.</para>
        /// </summary>
        RightCurlyBracket = 125, // 0x0000007D
        /// <summary>
        ///   <para>Tilde '~' key.</para>
        /// </summary>
        Tilde = 126, // 0x0000007E
        /// <summary>
        ///   <para>The forward delete key.</para>
        /// </summary>
        Delete = 127, // 0x0000007F
        /// <summary>
        ///   <para>Numeric keypad 0.</para>
        /// </summary>
        Keypad0 = 256, // 0x00000100
        /// <summary>
        ///   <para>Numeric keypad 1.</para>
        /// </summary>
        Keypad1 = 257, // 0x00000101
        /// <summary>
        ///   <para>Numeric keypad 2.</para>
        /// </summary>
        Keypad2 = 258, // 0x00000102
        /// <summary>
        ///   <para>Numeric keypad 3.</para>
        /// </summary>
        Keypad3 = 259, // 0x00000103
        /// <summary>
        ///   <para>Numeric keypad 4.</para>
        /// </summary>
        Keypad4 = 260, // 0x00000104
        /// <summary>
        ///   <para>Numeric keypad 5.</para>
        /// </summary>
        Keypad5 = 261, // 0x00000105
        /// <summary>
        ///   <para>Numeric keypad 6.</para>
        /// </summary>
        Keypad6 = 262, // 0x00000106
        /// <summary>
        ///   <para>Numeric keypad 7.</para>
        /// </summary>
        Keypad7 = 263, // 0x00000107
        /// <summary>
        ///   <para>Numeric keypad 8.</para>
        /// </summary>
        Keypad8 = 264, // 0x00000108
        /// <summary>
        ///   <para>Numeric keypad 9.</para>
        /// </summary>
        Keypad9 = 265, // 0x00000109
        /// <summary>
        ///   <para>Numeric keypad '.'.</para>
        /// </summary>
        KeypadPeriod = 266, // 0x0000010A
        /// <summary>
        ///   <para>Numeric keypad '/'.</para>
        /// </summary>
        KeypadDivide = 267, // 0x0000010B
        /// <summary>
        ///   <para>Numeric keypad '*'.</para>
        /// </summary>
        KeypadMultiply = 268, // 0x0000010C
        /// <summary>
        ///   <para>Numeric keypad '-'.</para>
        /// </summary>
        KeypadMinus = 269, // 0x0000010D
        /// <summary>
        ///   <para>Numeric keypad '+'.</para>
        /// </summary>
        KeypadPlus = 270, // 0x0000010E
        /// <summary>
        ///   <para>Numeric keypad Enter.</para>
        /// </summary>
        KeypadEnter = 271, // 0x0000010F
        /// <summary>
        ///   <para>Numeric keypad '='.</para>
        /// </summary>
        KeypadEquals = 272, // 0x00000110
        /// <summary>
        ///   <para>Up arrow key.</para>
        /// </summary>
        UpArrow = 273, // 0x00000111
        /// <summary>
        ///   <para>Down arrow key.</para>
        /// </summary>
        DownArrow = 274, // 0x00000112
        /// <summary>
        ///   <para>Right arrow key.</para>
        /// </summary>
        RightArrow = 275, // 0x00000113
        /// <summary>
        ///   <para>Left arrow key.</para>
        /// </summary>
        LeftArrow = 276, // 0x00000114
        /// <summary>
        ///   <para>Insert key key.</para>
        /// </summary>
        Insert = 277, // 0x00000115
        /// <summary>
        ///   <para>Home key.</para>
        /// </summary>
        Home = 278, // 0x00000116
        /// <summary>
        ///   <para>End key.</para>
        /// </summary>
        End = 279, // 0x00000117
        /// <summary>
        ///   <para>Page up.</para>
        /// </summary>
        PageUp = 280, // 0x00000118
        /// <summary>
        ///   <para>Page down.</para>
        /// </summary>
        PageDown = 281, // 0x00000119
        /// <summary>
        ///   <para>F1 function key.</para>
        /// </summary>
        F1 = 282, // 0x0000011A
        /// <summary>
        ///   <para>F2 function key.</para>
        /// </summary>
        F2 = 283, // 0x0000011B
        /// <summary>
        ///   <para>F3 function key.</para>
        /// </summary>
        F3 = 284, // 0x0000011C
        /// <summary>
        ///   <para>F4 function key.</para>
        /// </summary>
        F4 = 285, // 0x0000011D
        /// <summary>
        ///   <para>F5 function key.</para>
        /// </summary>
        F5 = 286, // 0x0000011E
        /// <summary>
        ///   <para>F6 function key.</para>
        /// </summary>
        F6 = 287, // 0x0000011F
        /// <summary>
        ///   <para>F7 function key.</para>
        /// </summary>
        F7 = 288, // 0x00000120
        /// <summary>
        ///   <para>F8 function key.</para>
        /// </summary>
        F8 = 289, // 0x00000121
        /// <summary>
        ///   <para>F9 function key.</para>
        /// </summary>
        F9 = 290, // 0x00000122
        /// <summary>
        ///   <para>F10 function key.</para>
        /// </summary>
        F10 = 291, // 0x00000123
        /// <summary>
        ///   <para>F11 function key.</para>
        /// </summary>
        F11 = 292, // 0x00000124
        /// <summary>
        ///   <para>F12 function key.</para>
        /// </summary>
        F12 = 293, // 0x00000125
        /// <summary>
        ///   <para>F13 function key.</para>
        /// </summary>
        F13 = 294, // 0x00000126
        /// <summary>
        ///   <para>F14 function key.</para>
        /// </summary>
        F14 = 295, // 0x00000127
        /// <summary>
        ///   <para>F15 function key.</para>
        /// </summary>
        F15 = 296, // 0x00000128
        /// <summary>
        ///   <para>Numlock key.</para>
        /// </summary>
        Numlock = 300, // 0x0000012C
        /// <summary>
        ///   <para>Capslock key.</para>
        /// </summary>
        CapsLock = 301, // 0x0000012D
        /// <summary>
        ///   <para>Scroll lock key.</para>
        /// </summary>
        ScrollLock = 302, // 0x0000012E
        /// <summary>
        ///   <para>Right shift key.</para>
        /// </summary>
        RightShift = 303, // 0x0000012F
        /// <summary>
        ///   <para>Left shift key.</para>
        /// </summary>
        LeftShift = 304, // 0x00000130
        /// <summary>
        ///   <para>Right Control key.</para>
        /// </summary>
        RightControl = 305, // 0x00000131
        /// <summary>
        ///   <para>Left Control key.</para>
        /// </summary>
        LeftControl = 306, // 0x00000132
        /// <summary>
        ///   <para>Right Alt key.</para>
        /// </summary>
        RightAlt = 307, // 0x00000133
        /// <summary>
        ///   <para>Left Alt key.</para>
        /// </summary>
        LeftAlt = 308, // 0x00000134
        /// <summary>
        ///   <para>Right Command key.</para>
        /// </summary>
        RightApple = 309, // 0x00000135
        /// <summary>
        ///   <para>Right Command key.</para>
        /// </summary>
        RightCommand = 309, // 0x00000135
        /// <summary>
        ///   <para>Left Command key.</para>
        /// </summary>
        LeftApple = 310, // 0x00000136
        /// <summary>
        ///   <para>Left Command key.</para>
        /// </summary>
        LeftCommand = 310, // 0x00000136
        /// <summary>
        ///   <para>Left Windows key.</para>
        /// </summary>
        LeftWindows = 311, // 0x00000137
        /// <summary>
        ///   <para>Right Windows key.</para>
        /// </summary>
        RightWindows = 312, // 0x00000138
        /// <summary>
        ///   <para>Alt Gr key.</para>
        /// </summary>
        AltGr = 313, // 0x00000139
        /// <summary>
        ///   <para>Help key.</para>
        /// </summary>
        Help = 315, // 0x0000013B
        /// <summary>
        ///   <para>Print key.</para>
        /// </summary>
        Print = 316, // 0x0000013C
        /// <summary>
        ///   <para>Sys Req key.</para>
        /// </summary>
        SysReq = 317, // 0x0000013D
        /// <summary>
        ///   <para>Break key.</para>
        /// </summary>
        Break = 318, // 0x0000013E
        /// <summary>
        ///   <para>Menu key.</para>
        /// </summary>
        Menu = 319, // 0x0000013F
        /// <summary>
        ///   <para>The Left (or primary) mouse button.</para>
        /// </summary>
        Mouse0 = 323, // 0x00000143
        /// <summary>
        ///   <para>Right mouse button (or secondary mouse button).</para>
        /// </summary>
        Mouse1 = 324, // 0x00000144
        /// <summary>
        ///   <para>Middle mouse button (or third button).</para>
        /// </summary>
        Mouse2 = 325, // 0x00000145
        /// <summary>
        ///   <para>Additional (fourth) mouse button.</para>
        /// </summary>
        Mouse3 = 326, // 0x00000146
        /// <summary>
        ///   <para>Additional (fifth) mouse button.</para>
        /// </summary>
        Mouse4 = 327, // 0x00000147
        /// <summary>
        ///   <para>Additional (or sixth) mouse button.</para>
        /// </summary>
        Mouse5 = 328, // 0x00000148
        /// <summary>
        ///   <para>Additional (or seventh) mouse button.</para>
        /// </summary>
        Mouse6 = 329, // 0x00000149
    }
}

namespace UnityEngine.Serialization
{
    /// <summary>
    ///   <para>Use this attribute to rename a field without losing its serialized value.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class FormerlySerializedAsAttribute : Attribute
    {
        private string m_oldName;

        /// <summary>
        ///   <para></para>
        /// </summary>
        /// <param name="oldName">The name of the field before renaming.</param>
        public FormerlySerializedAsAttribute(string oldName)
        {
            this.m_oldName = oldName;
        }

        /// <summary>
        ///   <para>The name of the field before the rename.</para>
        /// </summary>
        public string oldName
        {
            get
            {
                return this.m_oldName;
            }
        }
    }
}

#endif