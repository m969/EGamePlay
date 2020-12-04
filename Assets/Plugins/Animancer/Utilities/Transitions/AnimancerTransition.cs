// Animancer // https://kybernetik.com.au/animancer // Copyright 2020 Kybernetik //

using System.Collections.Generic;
using UnityEngine;

namespace Animancer
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> based <see cref="ITransition"/>s which can create a
    /// <see cref="ClipState"/> when passed into <see cref="AnimancerPlayable.Play(ITransition)"/>.
    /// </summary>
    /// <remarks>
    /// When adding a <see cref="CreateAssetMenuAttribute"/> to any derived classes, you can use
    /// <see cref="Strings.MenuPrefix"/> and <see cref="Strings.AssetMenuOrder"/>.
    /// <para></para>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/transitions/types#transition-assets">Transition Assets</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/AnimancerTransition
    /// 
    [HelpURL(Strings.DocsURLs.APIDocumentation + "/" + nameof(AnimancerTransition))]
    public abstract class AnimancerTransition : ScriptableObject, ITransition, IAnimationClipSource
    {
        /************************************************************************************************************************/

        /// <summary>Returns the <see cref="ITransition"/> wrapped by this <see cref="ScriptableObject"/>.</summary>
        public abstract ITransition GetTransition();

        /************************************************************************************************************************/

        /// <summary>Wraps <see cref="ITransition.FadeDuration"/>.</summary>
        public virtual float FadeDuration => GetTransition().FadeDuration;

        /// <summary>Wraps <see cref="IHasKey.Key"/>.</summary>
        public virtual object Key => GetTransition().Key;

        /// <summary>Wraps <see cref="ITransition.FadeMode"/>.</summary>
        public virtual FadeMode FadeMode => GetTransition().FadeMode;

        /// <summary>Wraps <see cref="ITransition.CreateState"/>.</summary>
        public virtual AnimancerState CreateState() => GetTransition().CreateState();

        /// <summary>Wraps <see cref="ITransition.Apply"/>.</summary>
        public virtual void Apply(AnimancerState state)
        {
            GetTransition().Apply(state);
            state.SetEditorName(name);
        }

        /************************************************************************************************************************/

        /// <summary>Wraps <see cref="AnimancerUtilities.GatherFromSource(ICollection{AnimationClip}, object)"/>.</summary>
        public virtual void GetAnimationClips(List<AnimationClip> clips) => clips.GatherFromSource(GetTransition());

        /************************************************************************************************************************/
    }

    /************************************************************************************************************************/

    /// <summary>An <see cref="AnimancerTransition"/> which uses a generic field for its <see cref="ITransition"/>.</summary>
    /// <remarks>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/transitions/types#transition-assets">Transition Assets</see>
    /// </remarks>
    /// https://kybernetik.com.au/animancer/api/Animancer/AnimancerTransition_1
    /// 
    [HelpURL(Strings.DocsURLs.APIDocumentation + "/" + nameof(AnimancerTransition) + "_1")]
    public class AnimancerTransition<T> : AnimancerTransition where T : ITransition
    {
        /************************************************************************************************************************/

        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("_Animation")]
        private T _Transition;

        /// <summary>[<see cref="SerializeField"/>]
        /// The <see cref="ITransition"/> wrapped by this <see cref="ScriptableObject"/>.
        /// <para></para>
        /// WARNING: the <see cref="AnimancerState.Transition{TState}.State"/> holds the post recently played state, so
        /// if you are sharing this transition between multiple objects it will only remember one of them.
        /// <para></para>
        /// You can use <see cref="AnimancerPlayable.StateDictionary.GetOrCreate(ITransition)"/> or
        /// <see cref="AnimancerLayer.GetOrCreateState(ITransition)"/> to get or create the state for a
        /// specific object.
        /// </summary>
        public ref T Transition => ref _Transition;

        public override ITransition GetTransition() => _Transition;

        /************************************************************************************************************************/
    }
}

/************************************************************************************************************************/

#if UNITY_EDITOR
namespace Animancer.Editor
{
    [UnityEditor.CustomEditor(typeof(AnimancerTransition), true)]
    internal class AnimancerTransitionEditor : ScriptableObjectEditor { }
}
#endif
