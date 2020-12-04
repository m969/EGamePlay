// Animancer // https://kybernetik.com.au/animancer // Copyright 2020 Kybernetik //

using UnityEngine.Playables;

namespace Animancer
{
    /// <summary>Interface for objects that manage a <see cref="UnityEngine.Playables.Playable"/>.</summary>
    /// https://kybernetik.com.au/animancer/api/Animancer/IPlayableWrapper
    /// 
    public interface IPlayableWrapper
    {
        /************************************************************************************************************************/

        /// <summary>The object which receives the output of the <see cref="Playable"/>.</summary>
        IPlayableWrapper Parent { get; }

        /// <summary>The <see cref="UnityEngine.Playables.Playable"/> managed by this object.</summary>
        Playable Playable { get; }

        /// <summary>The number of nodes using this object as their <see cref="Parent"/>.</summary>
        int ChildCount { get; }

        /// <summary>Returns the state connected to the specified `index` as a child of this object.</summary>
        AnimancerNode GetChild(int index);

        /// <summary>
        /// Indicates whether child playables should stay connected to the graph at all times.
        /// <para></para>
        /// If false, playables will be disconnected from the graph while they are at 0 weight to stop it from
        /// evaluating them every frame.
        /// </summary>
        /// <seealso cref="AnimancerPlayable.KeepChildrenConnected"/>
        bool KeepChildrenConnected { get; }

        /// <summary>How fast the <see cref="AnimancerState.Time"/> is advancing every frame.</summary>
        /// 
        /// <remarks>
        /// 1 is the normal speed.
        /// <para></para>
        /// A negative value will play the animation backwards.
        /// <para></para>
        /// <em>Animancer Lite does not allow this value to be changed in runtime builds.</em>
        /// </remarks>
        ///
        /// <example>
        /// <code>
        /// void PlayAnimation(AnimancerComponent animancer, AnimationClip clip)
        /// {
        ///     var state = animancer.Play(clip);
        ///
        ///     state.Speed = 1;// Normal speed.
        ///     state.Speed = 2;// Double speed.
        ///     state.Speed = 0.5f;// Half speed.
        ///     state.Speed = -1;// Normal speed playing backwards.
        /// }
        /// </code>
        /// </example>
        float Speed { get; set; }

        /************************************************************************************************************************/
    }
}

