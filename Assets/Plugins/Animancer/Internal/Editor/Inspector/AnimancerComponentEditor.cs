// Animancer // https://kybernetik.com.au/animancer // Copyright 2020 Kybernetik //

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only] A custom Inspector for <see cref="AnimancerComponent"/>s.</summary>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/AnimancerComponentEditor
    /// 
    [CustomEditor(typeof(AnimancerComponent), true), CanEditMultipleObjects]
    public class AnimancerComponentEditor : BaseAnimancerComponentEditor
    {
        /************************************************************************************************************************/

        private bool _ShowResetOnDisableWarning;

        protected override bool DoOverridePropertyGUI(string path, SerializedProperty property, GUIContent label)
        {
            if (path == Targets[0].ActionOnDisableFieldName)
            {
                EditorGUILayout.PropertyField(property, label, true);
                if (property.enumValueIndex == (int)AnimancerComponent.DisableAction.Reset)
                {
                    // Since getting all the components creates garbage, only do it during layout events.
                    if (Event.current.type == EventType.Layout)
                    {
                        _ShowResetOnDisableWarning = !AreAllResettingTargetsAboveTheirAnimator();
                    }

                    if (_ShowResetOnDisableWarning)
                    {
                        EditorGUILayout.HelpBox("Reset only works if this component is above the Animator" +
                            " so OnDisable can perform the Reset before the Animator actually gets disabled." +
                            " Click here to fix." +
                            "\n\nOtherwise you can use Stop and call Animator.Rebind before disabling this GameObject.",
                            MessageType.Error);

                        if (AnimancerGUI.TryUseClickEventInLastRect())
                            MoveResettingTargetsAboveTheirAnimator();
                    }
                }
                return true;
            }

            return base.DoOverridePropertyGUI(path, property, label);
        }

        /************************************************************************************************************************/

        private bool AreAllResettingTargetsAboveTheirAnimator()
        {
            for (int i = 0; i < Targets.Length; i++)
            {
                var target = Targets[i];
                if (!target.ResetOnDisable)
                    continue;

                var animator = target.Animator;
                if (animator == null ||
                    target.gameObject != animator.gameObject)
                    continue;

                var targetObject = (Object)target;
                var components = target.gameObject.GetComponents<Component>();
                for (int j = 0; j < components.Length; j++)
                {
                    var component = components[j];
                    if (component == targetObject)
                        break;
                    else if (component == animator)
                        return false;
                }
            }

            return true;
        }

        /************************************************************************************************************************/

        private void MoveResettingTargetsAboveTheirAnimator()
        {
            for (int i = 0; i < Targets.Length; i++)
            {
                var target = Targets[i];
                if (!target.ResetOnDisable)
                    continue;

                var animator = target.Animator;
                if (animator == null ||
                    target.gameObject != animator.gameObject)
                    continue;

                int animatorIndex = -1;

                var targetObject = (Object)target;
                var components = target.gameObject.GetComponents<Component>();
                for (int j = 0; j < components.Length; j++)
                {
                    var component = components[j];
                    if (component == targetObject)
                    {
                        if (animatorIndex >= 0)
                        {
                            var count = j - animatorIndex;
                            while (count-- > 0)
                                UnityEditorInternal.ComponentUtility.MoveComponentUp((Component)target);
                        }
                        break;
                    }
                    else if (component == animator)
                    {
                        animatorIndex = j;
                    }
                }
            }
        }

        /************************************************************************************************************************/
    }
}

#endif

