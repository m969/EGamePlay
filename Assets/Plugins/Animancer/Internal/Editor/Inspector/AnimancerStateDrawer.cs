// Animancer // https://kybernetik.com.au/animancer // Copyright 2020 Kybernetik //

#if UNITY_EDITOR

#pragma warning disable IDE0041 // Use 'is null' check.

using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using static Animancer.Editor.AnimancerPlayableDrawer;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only] Draws the Inspector GUI for an <see cref="AnimancerState"/>.</summary>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/AnimancerStateDrawer_1
    /// 
    public class AnimancerStateDrawer<T> : AnimancerNodeDrawer<T> where T : AnimancerState
    {
        /************************************************************************************************************************/

        /// <summary>
        /// Creates a new <see cref="AnimancerStateDrawer{T}"/> to manage the Inspector GUI for the `target`.
        /// </summary>
        public AnimancerStateDrawer(T target) => Target = target;

        /************************************************************************************************************************/

        /// <summary>The <see cref="GUIStyle"/> used for the area encompassing this drawer is <c>null</c>.</summary>
        protected override GUIStyle RegionStyle => null;

        /************************************************************************************************************************/

        /// <summary>Determines whether the <see cref="AnimancerState.MainObject"/> field can occupy the whole line.</summary>
        private bool IsAssetUsedAsKey =>
            Target.EditorName == null &&
            (Target.Key == null || ReferenceEquals(Target.Key, Target.MainObject));

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override bool AutoNormalizeSiblingWeights => AutoNormalizeWeights;

        /************************************************************************************************************************/

        /// <summary>
        /// Draws the state's main label: an <see cref="Object"/> field if it has a
        /// <see cref="AnimancerState.MainObject"/>, otherwise just a simple text label.
        /// <para></para>
        /// Also shows a bar to indicate its progress.
        /// </summary>
        protected override void DoLabelGUI(Rect area)
        {
            string label;
            if (Target.EditorName != null)
            {
                label = Target.EditorName;
            }
            else if (IsAssetUsedAsKey)
            {
                label = "";
            }
            else
            {
                var key = Target.Key;
                if (key is string)
                    label = $"\"{key}\"";
                else
                    label = key.ToString();
            }

            HandleLabelClick(area);

            AnimancerGUI.DoWeightLabel(ref area, Target.Weight);

            AnimationBindings.DoBindingMatchGUI(ref area, Target);

            var mainObject = Target.MainObject;
            if (!ReferenceEquals(mainObject, null))
            {
                EditorGUI.BeginChangeCheck();

                mainObject = EditorGUI.ObjectField(area, label, mainObject, typeof(Object), false);

                if (EditorGUI.EndChangeCheck())
                    Target.MainObject = mainObject;
            }
            else if (Target.EditorName != null)
            {
                EditorGUI.LabelField(area, Target.EditorName);
            }
            else
            {
                EditorGUI.LabelField(area, label, Target.ToString());
            }

            // Highlight a section of the label based on the time like a loading bar.
            if (Target.IsPlaying || Target.Time != 0)
            {
                var color = GUI.color;

                // Green = Playing, Yelow = Paused.
                GUI.color = Target.IsPlaying ? new Color(0.15f, 0.7f, 0.15f, 0.35f) : new Color(0.7f, 0.7f, 0.15f, 0.35f);

                area = EditorGUI.IndentedRect(area);
                area.width -= 18;

                var wrappedTime = GetWrappedTime(out var length);
                if (length > 0)
                    area.width *= Mathf.Clamp01(wrappedTime / length);

                GUI.DrawTexture(area, Texture2D.whiteTexture);

                GUI.color = color;
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Handles Ctrl + Click on the label to CrossFade the animation.
        /// <para></para>
        /// If Shift is also held, the effect will be queued until after the previous animation finishes.
        /// </summary>
        private void HandleLabelClick(Rect area)
        {
            var currentEvent = Event.current;
            if (currentEvent.type != EventType.MouseUp ||
                !currentEvent.control ||
                !area.Contains(currentEvent.mousePosition))
                return;

            currentEvent.Use();

            if (currentEvent.shift)
            {
                AnimationQueue.CrossFadeQueued(Target);
                return;
            }

            AnimationQueue.ClearQueue(Target.Layer);

            Target.Root.UnpauseGraph();
            var fadeDuration = Target.CalculateEditorFadeDuration(AnimancerPlayable.DefaultFadeDuration);
            Target.Root.Play(Target, fadeDuration);
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override void DoFoldoutGUI(Rect area)
        {
            float foldoutWidth;
            if (IsAssetUsedAsKey)
            {
                foldoutWidth = EditorGUI.indentLevel * AnimancerGUI.IndentSize;
            }
            else
            {
                foldoutWidth = EditorGUIUtility.labelWidth;
            }

            area.xMin -= 2;
            area.width = foldoutWidth;

            var hierarchyMode = EditorGUIUtility.hierarchyMode;
            EditorGUIUtility.hierarchyMode = true;

            IsExpanded = EditorGUI.Foldout(area, IsExpanded, GUIContent.none, true);

            EditorGUIUtility.hierarchyMode = hierarchyMode;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Gets the current <see cref="AnimancerState.Time"/>.
        /// If the state is looping, the value is modulo by the <see cref="AnimancerState.Length"/>.
        /// </summary>
        private float GetWrappedTime(out float length)
        {
            var time = Target.Time;
            length = Target.Length;

            var wrappedTime = time;

            if (Target.IsLooping)
            {
                wrappedTime = Mathf.Repeat(wrappedTime, length);
                if (wrappedTime == 0 && time != 0)
                    wrappedTime = length;
            }

            return wrappedTime;
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override void DoDetailsGUI()
        {
            if (!IsExpanded)
                return;

            EditorGUI.indentLevel++;
            DoTimeSliderGUI();
            DoNodeDetailsGUI();
            DoOnEndGUI();
            EditorGUI.indentLevel--;
        }

        /************************************************************************************************************************/

        /// <summary>Draws a slider for controlling the current <see cref="AnimancerState.Time"/>.</summary>
        private void DoTimeSliderGUI()
        {
            if (Target.Length <= 0)
                return;

            var time = GetWrappedTime(out var length);

            if (length == 0)
                return;

            var area = AnimancerGUI.LayoutSingleLineRect(AnimancerGUI.SpacingMode.Before);

            var normalized = DoNormalizedTimeToggle(ref area);

            string label;
            float max;
            if (normalized)
            {
                label = "Normalized Time";
                time /= length;
                max = 1;
            }
            else
            {
                label = "Time";
                max = length;
            }

            DoLoopCounterGUI(ref area, length);

            EditorGUI.BeginChangeCheck();
            label = AnimancerGUI.BeginTightLabel(label);
            time = EditorGUI.Slider(area, label, time, 0, max);
            AnimancerGUI.EndTightLabel();
            if (AnimancerGUI.TryUseClickEvent(area, 2))
                time = 0;
            if (EditorGUI.EndChangeCheck())
            {
                if (normalized)
                    Target.NormalizedTime = time;
                else
                    Target.Time = time;
            }
        }

        /************************************************************************************************************************/

        private bool DoNormalizedTimeToggle(ref Rect area)
        {
            var content = AnimancerGUI.TempContent("N");
            var style = AnimancerGUI.MiniButton;

            var width = UseNormalizedTimeSlidersWidth.GetWidth(style, content.text);
            var toggleArea = AnimancerGUI.StealFromRight(ref area, width);

            UseNormalizedTimeSliders.Value = GUI.Toggle(toggleArea, UseNormalizedTimeSliders, content, style);
            return UseNormalizedTimeSliders;
        }

        /************************************************************************************************************************/

        private static ConversionCache<int, string> _LoopCounterCache;

        private void DoLoopCounterGUI(ref Rect area, float length)
        {
            if (_LoopCounterCache == null)
                _LoopCounterCache = new ConversionCache<int, string>((x) => "x" + x);

            string label;
            var normalizedTime = Target.Time / length;
            if (float.IsNaN(normalizedTime))
            {
                label = "NaN";
            }
            else
            {
                var loops = Mathf.FloorToInt(Target.Time / length);
                label = _LoopCounterCache.Convert(loops);
            }

            var width = AnimancerGUI.CalculateLabelWidth(label);

            var labelArea = AnimancerGUI.StealFromRight(ref area, width);

            GUI.Label(labelArea, label);
        }

        /************************************************************************************************************************/

        private void DoOnEndGUI()
        {
            if (!Target.HasEvents || Target.Events.OnEnd == null)
                return;

            var area = AnimancerGUI.LayoutSingleLineRect(AnimancerGUI.SpacingMode.Before);

            EditorGUI.LabelField(area, "OnEnd: " + Target.Events.OnEnd.Method);
        }

        /************************************************************************************************************************/
        #region Context Menu
        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override void PopulateContextMenu(GenericMenu menu)
        {
            AddContextMenuFunctions(menu);

            AnimancerEditorUtilities.AddMenuItem(menu, "Play",
                !Target.IsPlaying || Target.Weight != 1,
                () =>
                {
                    Target.Root.UnpauseGraph();
                    Target.Root.Play(Target);
                });

            AnimancerEditorUtilities.AddFadeFunction(menu, "Cross Fade (Ctrl + Click)",
                Target.Weight != 1,
                Target, (duration) =>
                {
                    Target.Root.UnpauseGraph();
                    Target.Root.Play(Target, duration);
                });

            AnimancerEditorUtilities.AddFadeFunction(menu, "Cross Fade Queued (Ctrl + Shift + Click)",
                Target.Weight != 1,
                Target, (duration) =>
                {
                    AnimationQueue.CrossFadeQueued(Target);
                });

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Destroy State"), false, () => Target.Destroy());

            menu.AddSeparator("");

            AnimancerPlayableDrawer.AddDisplayOptions(menu);

            AnimancerEditorUtilities.AddDocumentationLink(menu, "State Documentation", Strings.DocsURLs.States);
        }

        /************************************************************************************************************************/

        /// <summary>Adds the details of this state to the `menu`.</summary>
        protected virtual void AddContextMenuFunctions(GenericMenu menu)
        {
            menu.AddDisabledItem(new GUIContent($"{DetailsPrefix}{nameof(AnimancerState.Key)}: {Target.Key}"));

            var length = Target.Length;
            if (!float.IsNaN(length))
                menu.AddDisabledItem(new GUIContent($"{DetailsPrefix}{nameof(AnimancerState.Length)}: {length}"));

            menu.AddDisabledItem(new GUIContent($"{DetailsPrefix}Playable Path: {Target.GetPath()}"));

            var mainAsset = Target.MainObject;
            if (mainAsset != null)
            {
                var assetPath = AssetDatabase.GetAssetPath(mainAsset);
                if (assetPath != null)
                    menu.AddDisabledItem(new GUIContent($"{DetailsPrefix}Asset Path: {assetPath.Replace("/", "->")}"));
            }

            if (Target.HasEvents)
            {
                const string OnEndPrefix = "End Event/";

                var endEvent = Target.Events.endEvent;
                menu.AddDisabledItem(new GUIContent($"{OnEndPrefix}{nameof(AnimancerState.NormalizedTime)}: {endEvent.normalizedTime}"));

                if (endEvent.callback == null)
                {
                    menu.AddDisabledItem(new GUIContent(OnEndPrefix + "Callback: null"));
                }
                else
                {
                    var label = OnEndPrefix +
                        (endEvent.callback.Target != null ? ("Target: " + endEvent.callback.Target) : "Target: null");

                    var targetObject = endEvent.callback.Target as Object;
                    AnimancerEditorUtilities.AddMenuItem(menu, label,
                        targetObject != null,
                        () => Selection.activeObject = targetObject);

                    menu.AddDisabledItem(new GUIContent(
                        $"{OnEndPrefix}Declaring Type: {endEvent.callback.Method.DeclaringType.FullName}"));

                    menu.AddDisabledItem(new GUIContent(
                        $"{OnEndPrefix}Method: {endEvent.callback.Method}"));
                }

                AnimancerEditorUtilities.AddMenuItem(menu, OnEndPrefix + "Clear",
                    !float.IsNaN(endEvent.normalizedTime) || endEvent.callback != null,
                    () => Target.Events.endEvent = new AnimancerEvent(float.NaN, null));

                AnimancerEditorUtilities.AddMenuItem(menu, OnEndPrefix + "Invoke",
                    endEvent.callback != null,
                    () => Target.Events.endEvent.Invoke(Target));
            }
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

#endif

