// Animancer // https://kybernetik.com.au/animancer // Copyright 2020 Kybernetik //

using UnityEngine;

namespace Animancer
{
    /// <summary>An object that can create an <see cref="AnimancerState"/> and set its details.</summary>
    /// <remarks>
    /// Transitions are generally used as arguments for <see cref="AnimancerPlayable.Play(ITransition)"/>.
    /// <para></para>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/transitions">Transitions</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/ITransition
    /// 
    public interface ITransition : IHasKey
    {
        /************************************************************************************************************************/

        /// <summary>
        /// Creates and returns a new <see cref="AnimancerState"/>.
        /// <para></para>
        /// Note that using methods like <see cref="AnimancerPlayable.Play(ITransition)"/> will also call
        /// <see cref="Apply"/>, so if you call this method manually you may want to call that method as well. Or you
        /// can just use <see cref="AnimancerUtilities.CreateStateAndApply"/>.
        /// </summary>
        /// <remarks>
        /// The first time a transition is used on an object, this method is called to create the state and register it
        /// in the internal dictionary using the <see cref="IHasKey.Key"/> so that it can be reused later on.
        /// </remarks>
        AnimancerState CreateState();

        /// <summary>
        /// When a transition is passed into <see cref="AnimancerPlayable.Play(ITransition)"/>, this property
        /// determines which <see cref="Animancer.FadeMode"/> will be used.
        /// </summary>
        FadeMode FadeMode { get; }

        /// <summary>The amount of time the transition should take (in seconds).</summary>
        float FadeDuration { get; }

        /// <summary>
        /// Called by <see cref="AnimancerPlayable.Play(ITransition)"/> to apply any modifications to the `state`.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="CreateState"/>, this method is called every time the transition is used so it can do
        /// things like set the <see cref="AnimancerState.Events"/> or <see cref="AnimancerState.Time"/>.
        /// </remarks>
        void Apply(AnimancerState state);

        /************************************************************************************************************************/
    }

    /// <summary>
    /// An <see cref="ITransition"/> with some additional details for the Unity Editor GUI.
    /// </summary>
    public interface ITransitionDetailed : ITransition
    {
        /************************************************************************************************************************/

        /// <summary>Indicates what the value of <see cref="AnimancerState.IsLooping"/> will be for the created state.</summary>
        bool IsLooping { get; }

        /// <summary>Determines what <see cref="AnimancerState.NormalizedTime"/> to start the animation at.</summary>
        float NormalizedStartTime { get; set; }

        /// <summary>Determines how fast the animation plays (1x = normal speed).</summary>
        float Speed { get; set; }

        /// <summary>The maximum amount of time the animation is expected to take (in seconds).</summary>
        float MaximumDuration { get; }

        /// <summary>Indicates whether this transition can create a valid <see cref="AnimancerState"/>.</summary>
        bool IsValid { get; }

        /// <summary>The <see cref="AnimancerState.MainObject"/> that the created state will have.</summary>
        Object MainObject { get; }

#if UNITY_EDITOR
        /// <summary>[Editor-Only] Adds context menu functions for this transition.</summary>
        void AddItemsToContextMenu(UnityEditor.GenericMenu menu, UnityEditor.SerializedProperty property,
            Editor.Serialization.PropertyAccessor accessor);
#endif

        /************************************************************************************************************************/
    }
}

