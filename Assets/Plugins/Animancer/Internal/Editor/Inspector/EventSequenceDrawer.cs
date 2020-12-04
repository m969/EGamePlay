// Animancer // https://kybernetik.com.au/animancer // Copyright 2020 Kybernetik //

#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using EventSequence = Animancer.AnimancerEvent.Sequence.Serializable;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only] Draws the Inspector GUI for <see cref="EventSequence"/>.</summary>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/EventSequenceDrawer
    /// 
    [CustomPropertyDrawer(typeof(EventSequence), true)]
    public sealed class EventSequenceDrawer : PropertyDrawer
    {
        /************************************************************************************************************************/

        /// <summary>Details of an <see cref="EventSequence"/>.</summary>
        public sealed class Context : IDisposable
        {
            /************************************************************************************************************************/

            /// <summary>A wrapper around a <see cref="SerializedProperty"/> representing an array field.</summary>
            public sealed class SerializedArrayProperty
            {
                /************************************************************************************************************************/

                private SerializedProperty _Property;

                /// <summary>The property representing the <see cref="EventSequence._NormalizedTimes"/> field.</summary>
                public SerializedProperty Property
                {
                    get => _Property;
                    set
                    {
                        _Property = value;
                        RefreshCount();
                    }
                }

                /************************************************************************************************************************/

                private int _Count;

                /// <summary>The cached <see cref="SerializedProperty.arraySize"/> of <see cref="Property"/>.</summary>
                public int Count
                {
                    get => _Count;
                    set => Property.arraySize = _Count = value;
                }

                /// <summary>Updates the cached <see cref="Count"/>.</summary>
                public void RefreshCount()
                {
                    _Count = _Property != null ? _Property.arraySize : 0;
                }

                /************************************************************************************************************************/

                /// <summary>Shorthand for <see cref="SerializedProperty.GetArrayElementAtIndex"/> on <see cref="Property"/>.</summary>
                public SerializedProperty GetElement(int index) => Property.GetArrayElementAtIndex(index);

                /************************************************************************************************************************/
            }

            /************************************************************************************************************************/

            /// <summary>The main property representing the <see cref="EventSequence"/> field.</summary>
            public SerializedProperty Property { get; private set; }

            /// <summary>The property representing the <see cref="EventSequence._NormalizedTimes"/> field.</summary>
            public readonly SerializedArrayProperty Times = new SerializedArrayProperty();

            /// <summary>The property representing the <see cref="EventSequence._Names"/> field.</summary>
            public readonly SerializedArrayProperty Names = new SerializedArrayProperty();

            /// <summary>The property representing the <see cref="EventSequence._Callbacks"/> field.</summary>
            public readonly SerializedArrayProperty Callbacks = new SerializedArrayProperty();

            /************************************************************************************************************************/

            private int _SelectedEvent;

            /// <summary>The index of the currently selected event.</summary>
            public int SelectedEvent
            {
                get => _SelectedEvent;
                set
                {
                    if (Times != null && value >= 0 && (value < Times.Count || Times.Count == 0))
                    {
                        float normalizedTime;
                        if (Times.Count > 0)
                        {
                            normalizedTime = Times.GetElement(value).floatValue;
                        }
                        else
                        {
                            var transition = TransitionContext?.Transition;
                            var speed = transition != null ? transition.Speed : 1;
                            normalizedTime = AnimancerEvent.Sequence.GetDefaultNormalizedEndTime(speed);
                        }

                        TransitionPreviewWindow.PreviewNormalizedTime = normalizedTime;
                    }

                    if (_SelectedEvent == value &&
                        Callbacks != null)
                        return;

                    _SelectedEvent = value;
                    TemporarySettings.Instance.SetSelectedEvent(Callbacks.Property, value);
                }
            }

            /************************************************************************************************************************/

            /// <summary>The singleton instance.</summary>
            public static readonly Context Instance = new Context();

            private Context() { }

            /// <summary>
            /// Returns a <see cref="Context"/> representing the `property`.
            /// <para></para>
            /// Note that the same instance is returned every time.
            /// </summary>
            public static Context Get(SerializedProperty property)
            {
                Instance.Initialise(property);
                return Instance;
            }

            private void Initialise(SerializedProperty property)
            {
                if (Property != property)
                {
                    Property = property;

                    Times.Property = property.FindPropertyRelative(EventSequence.NormalizedTimesField);
                    Names.Property = property.FindPropertyRelative(EventSequence.NamesField);
                    Callbacks.Property = property.FindPropertyRelative(EventSequence.CallbacksField);

                    if (Names.Count > Times.Count)
                        Names.Count = Times.Count;
                    if (Callbacks.Count > Times.Count)
                        Callbacks.Count = Times.Count;

                    _SelectedEvent = TemporarySettings.Instance.GetSelectedEvent(Callbacks.Property);
                    if (_SelectedEvent > Times.Count - 1)
                        _SelectedEvent = Mathf.Max(0, Times.Count - 1);
                }

                EditorGUI.BeginChangeCheck();
            }

            /************************************************************************************************************************/

            /// <summary>[<see cref="IDisposable"/>]
            /// Reduces the <see cref="Callbacks"/> array size to remove any empty elements.
            /// </summary>
            public void Dispose()
            {
                if (Times.Count == 1 && Callbacks.Count == 0 && float.IsNaN(Times.GetElement(0).floatValue))
                {
                    Times.Count = 0;
                }
                else
                {
                    if (Names.Count > Times.Count)
                        Names.Count = Times.Count;
                    if (Callbacks.Count > Times.Count)
                        Callbacks.Count = Times.Count;

                    while (Callbacks.Count > 0)
                    {
                        var callbackProperty = Callbacks.GetElement(Callbacks.Count - 1);
                        var callback = Serialization.GetValue(callbackProperty);
                        if (callback != null && EventSequence.HasPersistentCalls(callback))
                            break;
                        else
                            Callbacks.Count--;
                    }
                }

                if (EditorGUI.EndChangeCheck())
                    Property.serializedObject.ApplyModifiedProperties();

                Property = null;
            }

            /************************************************************************************************************************/

            /// <summary>Shorthand for <see cref="TransitionDrawer.Context"/>.</summary>
            public TransitionDrawer.TransitionContext TransitionContext => TransitionDrawer.Context;

            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/

        /// <summary>Can't cache because the <see cref="TimeRuler"/> doesn't work properly.</summary>
        public override bool CanCacheInspectorGUI(SerializedProperty property) => false;

        /************************************************************************************************************************/

        /// <summary>
        /// Calculates the number of vertical pixels the `property` will occupy when it is drawn.
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            using (var context = Context.Get(property))
            {
                var height = AnimancerGUI.LineHeight;

                var fullLineHeight = AnimancerGUI.StandardSpacing + AnimancerGUI.LineHeight;

                if (property.isExpanded)// If expanded, draw all events.
                {
                    var fullDummyHeight = DummySerializableCallback.Height + AnimancerGUI.StandardSpacing;

                    if (context.Times.Count > 0)
                    {
                        height += context.Times.Count * 2 * fullLineHeight;

                        for (int i = 0; i < context.Callbacks.Count; i++)
                        {
                            var callback = context.Callbacks.GetElement(i);
                            height += EditorGUI.GetPropertyHeight(callback, null, false) + AnimancerGUI.StandardSpacing;
                        }

                        height += (context.Times.Count - context.Callbacks.Count) * fullDummyHeight;
                    }
                    else
                    {
                        height += fullLineHeight + fullDummyHeight;
                    }
                }
                else// If not expanded, only draw the selected event.
                {
                    if (context.SelectedEvent >= 0)
                    {
                        // Name (not for the End Event).
                        if (context.SelectedEvent < context.Times.Count - 1)
                            height += fullLineHeight;

                        // Time.
                        height += fullLineHeight;

                        // Callback.
                        if (context.SelectedEvent < context.Callbacks.Count)
                        {
                            var callback = context.Callbacks.GetElement(context.SelectedEvent);
                            height += EditorGUI.GetPropertyHeight(callback, null, false) + AnimancerGUI.StandardSpacing;
                        }
                        else
                        {
                            height += DummySerializableCallback.Height + AnimancerGUI.StandardSpacing;
                        }
                    }
                }

                return height;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Draws the GUI for the `property`.</summary>
        public override void OnGUI(Rect area, SerializedProperty property, GUIContent label)
        {
            using (var context = Context.Get(property))
            {
                DoHeaderGUI(ref area, label, context);

                EditorGUI.indentLevel++;
                if (property.isExpanded)
                {
                    DoAllEventsGUI(ref area, context);
                }
                else if (context.SelectedEvent >= 0)
                {
                    DoEventGUI(ref area, context, context.SelectedEvent, true, true);
                }
                EditorGUI.indentLevel--;
            }
        }

        /************************************************************************************************************************/

        private static readonly TimeRuler
            TimeRuler = new TimeRuler();

        private void DoHeaderGUI(ref Rect area, GUIContent label, Context context)
        {
            area.height = AnimancerGUI.LineHeight;
            var headerArea = area;
            AnimancerGUI.NextVerticalArea(ref area);

            label = EditorGUI.BeginProperty(headerArea, label, context.Property);

            var addEventArea = AnimancerGUI.StealFromRight(ref headerArea, headerArea.height, AnimancerGUI.StandardSpacing);
            DoAddEventButtonGUI(addEventArea, context);

            if (context.TransitionContext != null && context.TransitionContext.Transition != null)
            {
                EditorGUI.EndProperty();

                TimeRuler.DoGUI(headerArea, context, out var addEventNormalizedTime);

                if (!float.IsNaN(addEventNormalizedTime))
                {
                    AddEvent(context, addEventNormalizedTime);
                }
            }
            else
            {
                label.text = AnimancerGUI.GetNarrowText(label.text);

                var summary = AnimancerGUI.TempContent();
                if (context.Times.Count == 0)
                {
                    summary.text = "[0] End Time 1";
                }
                else
                {
                    var index = context.Times.Count - 1; ;
                    var endTime = context.Times.GetElement(index).floatValue;
                    summary.text = $"[{index}] End Time {endTime:G3}";
                }

                EditorGUI.LabelField(headerArea, label, summary);

                EditorGUI.EndProperty();
            }

            EditorGUI.BeginChangeCheck();
            context.Property.isExpanded =
                EditorGUI.Foldout(headerArea, context.Property.isExpanded, GUIContent.none, true);
            if (EditorGUI.EndChangeCheck())
                context.SelectedEvent = -1;
        }

        /************************************************************************************************************************/

        private static readonly GUIContent
            AddEventContent = EditorGUIUtility.IconContent("Animation.AddEvent", Strings.ProOnlyTag + "Add event");

        /// <summary>Draws a button to add a new event.</summary>
        public void DoAddEventButtonGUI(Rect area, Context context)
        {
            var style = ObjectPool.GetCachedResult(() => new GUIStyle(EditorStyles.miniButton)
            {
                padding = new RectOffset(-1, 1, 0, 0),
                fixedHeight = 0,
            });

            if (!GUI.Button(area, AddEventContent, style))
                return;

            // If the target is currently being previewed, add the event at the currently selected time.
            var state = TransitionPreviewWindow.GetCurrentState();
            var normalizedTime = state != null ? state.NormalizedTime : float.NaN;
            AddEvent(context, normalizedTime);
        }

        /************************************************************************************************************************/

        private void AddEvent(Context context, float normalizedTime)
        {
            // Otherwise add it halfway between the last event and the end.
            if (context.Times.Count == 0)
            {
                // Having any events means we need the end time too.
                context.Times.Count = 2;
                context.Times.GetElement(1).floatValue = float.NaN;
                if (float.IsNaN(normalizedTime))
                    normalizedTime = 0.5f;
            }
            else
            {
                context.Times.Property.InsertArrayElementAtIndex(context.Times.Count - 1);
                context.Times.Count++;

                if (float.IsNaN(normalizedTime))
                {
                    var transition = context.TransitionContext.Transition;

                    var previousTime = context.Times.Count >= 3 ?
                        context.Times.GetElement(context.Times.Count - 3).floatValue :
                        AnimancerEvent.Sequence.GetDefaultNormalizedStartTime(transition.Speed);

                    var endTime = context.Times.GetElement(context.Times.Count - 1).floatValue;
                    if (float.IsNaN(endTime))
                        endTime = AnimancerEvent.Sequence.GetDefaultNormalizedEndTime(transition.Speed);

                    normalizedTime = previousTime < endTime ?
                        (previousTime + endTime) * 0.5f :
                        previousTime;
                }
            }

            WrapEventTime(context, ref normalizedTime);

            var newEvent = context.Times.Count - 2;
            context.Times.GetElement(newEvent).floatValue = normalizedTime;
            context.SelectedEvent = context.Times.Count - 2;

            if (context.Callbacks.Count > newEvent)
            {
                context.Callbacks.Property.InsertArrayElementAtIndex(newEvent);

                // Make sure the callback starts empty rather than copying an existing value.
                var callback = context.Callbacks.GetElement(newEvent);
                callback.SetValue(null);
                context.Callbacks.Property.OnPropertyChanged();
            }

            GUI.changed = true;
            GUIUtility.ExitGUI();
        }

        /************************************************************************************************************************/

        private static void WrapEventTime(Context context, ref float normalizedTime)
        {
            var transition = context.TransitionContext.Transition;
            if (transition != null && transition.IsLooping)
            {
                if (normalizedTime == 0)
                    return;
                else if (normalizedTime % 1 == 0)
                    normalizedTime = AnimancerEvent.AlmostOne;
                else
                    normalizedTime = normalizedTime.Wrap01();
            }
        }

        /************************************************************************************************************************/

        private static readonly int EventTimeHash = "EventTime".GetHashCode();

        private static int _HotControlAdjustRoot;
        private static int _SelectedEventToHotControl;

        private static void DoAllEventsGUI(ref Rect area, Context context)
        {
            var currentGUIEvent = Event.current;
            if (currentGUIEvent.type == EventType.Used)
                return;

            var rootControlID = GUIUtility.GetControlID(EventTimeHash - 1, FocusType.Passive);

            var eventCount = Mathf.Max(1, context.Times.Count);
            for (int i = 0; i < eventCount; i++)
            {
                var controlID = GUIUtility.GetControlID(EventTimeHash + i, FocusType.Passive);

                if (rootControlID == _HotControlAdjustRoot &&
                    _SelectedEventToHotControl > 0 &&
                    i == context.SelectedEvent)
                {
                    GUIUtility.hotControl = GUIUtility.keyboardControl = controlID + _SelectedEventToHotControl;
                    _SelectedEventToHotControl = 0;
                    _HotControlAdjustRoot = -1;
                }

                DoEventGUI(ref area, context, i, false, true);

                if (currentGUIEvent.type == EventType.Used)
                {
                    context.SelectedEvent = i;

                    if (SortEvents(context))
                    {
                        _SelectedEventToHotControl = GUIUtility.keyboardControl - controlID;
                        _HotControlAdjustRoot = rootControlID;
                        AnimancerGUI.Deselect();
                    }

                    GUIUtility.ExitGUI();
                }
            }
        }

        /************************************************************************************************************************/

        /// <summary>Draws the GUI fields for the event at the specified `index`.</summary>
        public static void DoEventGUI(ref Rect area, Context context, int index, bool autoSort, bool showCallback)
        {
            GetEventLabels(index, context,
                out var nameLabel, out var timeLabel, out var callbackLabel, out var defaultTime, out var isEndEvent);

            DoNameGUI(ref area, context, index, nameLabel);
            DoTimeGUI(ref area, context, index, autoSort, timeLabel, defaultTime, isEndEvent);
            if (showCallback)
                DoCallbackGUI(ref area, context, index, autoSort, callbackLabel);
        }

        /************************************************************************************************************************/

        /// <summary>Draws the time field for the event at the specified `index`.</summary>
        public static void DoNameGUI(ref Rect area, Context context, int index, string nameLabel)
        {
            if (nameLabel == null)
                return;

            area.height = AnimancerGUI.LineHeight;
            var fieldArea = area;
            AnimancerGUI.NextVerticalArea(ref area);

            var content = AnimancerGUI.TempContent(nameLabel,
                "An optional name which can be used to identify the event in code." +
                " Leaving all names blank is recommended if you are not using them.");

            fieldArea = EditorGUI.PrefixLabel(fieldArea, content);

            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            if (index < context.Names.Count)
            {
                var nameProperty = context.Names.GetElement(index);

                EditorGUI.BeginProperty(fieldArea, GUIContent.none, nameProperty);
                nameProperty.stringValue = DoEventNameTextField(fieldArea, context, nameProperty.stringValue);
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.BeginChangeCheck();

                EditorGUI.BeginProperty(fieldArea, GUIContent.none, context.Names.Property);
                var name = DoEventNameTextField(fieldArea, context, "");
                EditorGUI.EndProperty();

                if (EditorGUI.EndChangeCheck() && !string.IsNullOrEmpty(name))
                {
                    context.Names.Count++;
                    if (context.Names.Count < index + 1)
                    {
                        var nextProperty = context.Names.GetElement(context.Names.Count - 1);
                        nextProperty.stringValue = "";
                        context.Names.Count = index + 1;
                    }

                    var nameProperty = context.Names.GetElement(index);
                    nameProperty.stringValue = name;
                }
            }

            EditorGUI.indentLevel = indentLevel;
        }

        private static string DoEventNameTextField(Rect area, Context context, string text)
        {
            var eventNames = EventNamesAttribute.GetNames(context.TransitionContext.Property);
            if (eventNames == null || eventNames.Length == 0)
                return EditorGUI.TextField(area, text);

            var index = text == "" ? 0 : Array.IndexOf(eventNames, text, 1);

            EditorGUI.BeginChangeCheck();
            index = EditorGUI.Popup(area, index, eventNames);
            if (EditorGUI.EndChangeCheck())
                return index > 0 ? eventNames[index] : "";
            else
                return text;
        }

        /************************************************************************************************************************/

        private static float _PreviousTime = float.NaN;

        /// <summary>Draws the time field for the event at the specified `index`.</summary>
        public static void DoTimeGUI(ref Rect area, Context context, int index, bool autoSort,
            string timeLabel, float defaultTime, bool isEndEvent)
        {
            EditorGUI.BeginChangeCheck();

            area.height = AnimancerGUI.LineHeight;
            var timeArea = area;
            AnimancerGUI.NextVerticalArea(ref area);

            var timeContent = AnimancerGUI.TempContent(timeLabel, isEndEvent ?
                Strings.ProOnlyTag + "The time when the End Callback will be triggered" :
                Strings.ProOnlyTag + "The time when the Event Callback will be triggered");

            var length = context.TransitionContext?.MaximumDuration ?? float.NaN;

            float normalizedTime;

            if (index < context.Times.Count)
            {
                var timeProperty = context.Times.GetElement(index);

                var wasEditingTextField = EditorGUIUtility.editingTextField;
                if (!wasEditingTextField)
                    _PreviousTime = float.NaN;

                EditorGUI.BeginChangeCheck();

                timeContent = EditorGUI.BeginProperty(area, timeContent, timeProperty);
                normalizedTime = AnimancerGUI.DoOptionalTimeField(
                    ref timeArea, timeContent, timeProperty.floatValue, true, length, defaultTime);
                EditorGUI.EndProperty();

                var isEditingTextField = EditorGUIUtility.editingTextField;
                if (EditorGUI.EndChangeCheck() || (wasEditingTextField && !isEditingTextField))
                {
                    if (isEndEvent)
                    {
                        timeProperty.floatValue = normalizedTime;
                    }
                    else if (float.IsNaN(normalizedTime))
                    {
                        RemoveEvent(context, index);
                        AnimancerGUI.Deselect();
                    }
                    else if (!autoSort && isEditingTextField)
                    {
                        _PreviousTime = normalizedTime;
                    }
                    else
                    {
                        if (!float.IsNaN(_PreviousTime))
                        {
                            if (Event.current.keyCode != KeyCode.Escape)
                            {
                                normalizedTime = _PreviousTime;
                                AnimancerGUI.Deselect();
                            }

                            _PreviousTime = float.NaN;
                        }

                        WrapEventTime(context, ref normalizedTime);

                        timeProperty.floatValue = normalizedTime;

                        if (autoSort)
                            SortEvents(context);
                    }

                    GUI.changed = true;
                }
            }
            else// Dummy End Event.
            {
                Debug.Assert(index == 0, "This is assumed to be a dummy end event, which should only be at index 0");
                EditorGUI.BeginChangeCheck();

                EditorGUI.BeginProperty(timeArea, GUIContent.none, context.Times.Property);
                normalizedTime = AnimancerGUI.DoOptionalTimeField(
                    ref timeArea, timeContent, float.NaN, true, length, defaultTime, true);
                EditorGUI.EndProperty();

                if (EditorGUI.EndChangeCheck() && !float.IsNaN(normalizedTime))
                {
                    context.Times.Count = 1;
                    var timeProperty = context.Times.GetElement(0);
                    timeProperty.floatValue = normalizedTime;
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                TransitionPreviewWindow.PreviewNormalizedTime = normalizedTime;

                if (Event.current.type != EventType.Layout)
                    GUIUtility.ExitGUI();
            }
        }

        /// <summary>Draws the time field for the event at the specified `index`.</summary>
        public static void DoTimeGUI(ref Rect area, Context context, int index, bool autoSort)
        {
            GetEventLabels(index, context, out var _, out var timeLabel, out var _, out var defaultTime, out var isEndEvent);
            DoTimeGUI(ref area, context, index, autoSort, timeLabel, defaultTime, isEndEvent);
        }

        /************************************************************************************************************************/

        /// <summary>Draws the GUI fields for the event at the specified `index`.</summary>
        public static void DoCallbackGUI(ref Rect area, Context context, int index, bool autoSort, string callbackLabel)
        {
            var label = AnimancerGUI.TempContent(callbackLabel);

            if (index < context.Callbacks.Count)
            {
                var callback = context.Callbacks.GetElement(index);
                area.height = EditorGUI.GetPropertyHeight(callback, false);
                EditorGUI.BeginProperty(area, GUIContent.none, callback);

                // UnityEvents ignore the proper indentation which makes them look terrible in a list.
                // So we force the area to be indented.
                var indentedArea = EditorGUI.IndentedRect(area);
                var indentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                EditorGUI.PropertyField(indentedArea, callback, label, false);

                EditorGUI.indentLevel = indentLevel;
                EditorGUI.EndProperty();
            }
            else if (DummySerializableCallback.DoCallbackGUI(ref area, label, context.Callbacks.Property, out var callback))
            {
                context.Callbacks.Property.ForEachTarget((callbacksProperty) =>
                {
                    var accessor = callbacksProperty.GetAccessor();
                    var oldCallbacks = (Array)accessor.GetValue(callbacksProperty.serializedObject.targetObject);

                    Array newCallbacks;
                    if (oldCallbacks == null)
                    {
                        var elementType = accessor.FieldType.GetElementType();
                        newCallbacks = Array.CreateInstance(elementType, 1);
                    }
                    else
                    {
                        var elementType = oldCallbacks.GetType().GetElementType();
                        newCallbacks = Array.CreateInstance(elementType, index + 1);
                        Array.Copy(oldCallbacks, newCallbacks, oldCallbacks.Length);
                    }

                    newCallbacks.SetValue(callback, index);
                    accessor.SetValue(callbacksProperty, newCallbacks);
                });

                context.Callbacks.Property.OnPropertyChanged();
                context.Callbacks.Count = index + 1;

                if (index >= context.Times.Count)
                {
                    context.Times.Property.InsertArrayElementAtIndex(index);
                    context.Times.Count++;
                    context.Times.GetElement(index).floatValue = float.NaN;
                }
            }

            AnimancerGUI.NextVerticalArea(ref area);
        }

        /************************************************************************************************************************/

        private static ConversionCache<int, string> _NameLabelCache, _TimeLabelCache, _CallbackLabelCache;

        private static void GetEventLabels(int index, Context context,
            out string nameLabel, out string timeLabel, out string callbackLabel, out float defaultTime, out bool isEndEvent)
        {
            if (index >= context.Times.Count - 1)
            {
                nameLabel = null;
                timeLabel = "End Time";
                callbackLabel = "End Callback";

                defaultTime = AnimancerEvent.Sequence.GetDefaultNormalizedEndTime(
                    context.TransitionContext?.Transition?.Speed ?? 1);
                isEndEvent = true;
            }
            else
            {
                if (_NameLabelCache == null)
                {
                    _NameLabelCache = new ConversionCache<int, string>((i) => $"Event {i} Name");
                    _TimeLabelCache = new ConversionCache<int, string>((i) => $"Event {i} Time");
                    _CallbackLabelCache = new ConversionCache<int, string>((i) => $"Event {i} Callback");
                }

                nameLabel = _NameLabelCache.Convert(index);
                timeLabel = _TimeLabelCache.Convert(index);
                callbackLabel = _CallbackLabelCache.Convert(index);

                defaultTime = 0;
                isEndEvent = false;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Removes the event at the specified `index`.</summary>
        public static void RemoveEvent(Context context, int index)
        {
            // Only remove the time if it is not an End Event.
            if (index < context.Times.Count - 1)
            {
                context.Times.Property.DeleteArrayElementAtIndex(index);
                context.Times.Count--;
            }
            else// If it was an End Event, prevent the selection from moving on to later GUI elements.
            {
                AnimancerGUI.Deselect();
            }

            if (index < context.Names.Count)
            {
                context.Names.Property.DeleteArrayElementAtIndex(index);
                context.Names.Count--;
            }

            if (index < context.Callbacks.Count)
            {
                context.Callbacks.Property.DeleteArrayElementAtIndex(index);
                context.Callbacks.Count--;
            }
        }

        /************************************************************************************************************************/

        private static bool SortEvents(Context context)
        {
            if (context.Times.Count <= 2)
                return false;

            // The serializable sequence sorts itself in ISerializationCallbackReceiver.OnBeforeSerialize.
            var selectedEvent = context.SelectedEvent;
            var sorted = context.Property.serializedObject.ApplyModifiedProperties();
            if (!sorted)
                return false;

            context.Property.serializedObject.Update();
            context.Times.RefreshCount();
            context.Names.RefreshCount();
            context.Callbacks.RefreshCount();
            return context.SelectedEvent != selectedEvent;
        }

        /************************************************************************************************************************/
    }
}

#endif

