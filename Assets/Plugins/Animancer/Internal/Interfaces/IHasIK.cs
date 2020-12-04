// Animancer // https://kybernetik.com.au/animancer // Copyright 2020 Kybernetik //

using UnityEngine;

namespace Animancer
{
    /// <summary>A node which has an Inverse Kinematics system that can be enabled and disabled.</summary>
    /// <remarks>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/ik">Inverse Kinematics</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/IHasIK
    /// 
    public interface IHasIK
    {
        /************************************************************************************************************************/

        /// <summary>The <see cref="AnimancerPlayable"/> at the root of the graph.</summary>
        AnimancerPlayable Root { get; }

        /************************************************************************************************************************/

        /// <summary>
        /// Determines whether <c>OnAnimatorIK(int layerIndex)</c> will be called on the animated object while this
        /// node is active. The initial value is determined by <see cref="AnimancerPlayable.DefaultApplyAnimatorIK"/>
        /// when a new state is created and setting this value will also set the default.
        /// <para></para>
        /// This is equivalent to the "IK Pass" toggle in Animator Controller layers, except that due to limitations in
        /// the Playables API the <c>layerIndex</c> will always be zero.
        /// </summary>
        bool ApplyAnimatorIK { get; set; }

        /************************************************************************************************************************/

        /// <summary>
        /// Determines whether this node or any of its children should apply IK to the character's feet.
        /// The initial value is determined by <see cref="AnimancerPlayable.DefaultApplyFootIK"/> when a new state is
        /// created.
        /// <para></para>
        /// This is equivalent to the "Foot IK" toggle in Animator Controller states.
        /// </summary>
        bool ApplyFootIK { get; set; }

        /************************************************************************************************************************/
    }
}

/************************************************************************************************************************/
#if UNITY_EDITOR
/************************************************************************************************************************/

namespace Animancer.Editor
{
    partial class AnimancerEditorUtilities
    {
        /************************************************************************************************************************/

        /// <summary>Adds functions relevant to the `ik`.</summary>
        public static void AddContextMenuIK(UnityEditor.GenericMenu menu, IHasIK ik)
        {
            menu.AddItem(new GUIContent("Inverse Kinematics/Apply Animator IK ?"),
                ik.ApplyAnimatorIK,
                () => ik.ApplyAnimatorIK = !ik.ApplyAnimatorIK);
            menu.AddItem(new GUIContent("Inverse Kinematics/Default Apply Animator IK ?"),
                ik.Root.DefaultApplyAnimatorIK,
                () => ik.Root.DefaultApplyAnimatorIK = !ik.Root.DefaultApplyAnimatorIK);
            menu.AddItem(new GUIContent("Inverse Kinematics/Apply Foot IK ?"),
                ik.ApplyFootIK,
                () => ik.ApplyFootIK = !ik.ApplyFootIK);
            menu.AddItem(new GUIContent("Inverse Kinematics/Default Apply Foot IK ?"),
                ik.Root.DefaultApplyFootIK,
                () => ik.Root.DefaultApplyFootIK = !ik.Root.DefaultApplyFootIK);
        }

        /************************************************************************************************************************/
    }
}

/************************************************************************************************************************/
#endif
/************************************************************************************************************************/

