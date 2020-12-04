// Animancer // https://kybernetik.com.au/animancer // Copyright 2020 Kybernetik //

#if UNITY_EDITOR

using System.Collections.Generic;

namespace Animancer.Editor
{
    /// <summary>[Internal] Manages the playing of animations in sequence.</summary>
    internal sealed class AnimationQueue
    {
        /************************************************************************************************************************/

        private static readonly Dictionary<AnimancerLayer, AnimationQueue>
            PlayableToQueue = new Dictionary<AnimancerLayer, AnimationQueue>();

        private readonly List<AnimancerState>
            Queue = new List<AnimancerState>();

        /************************************************************************************************************************/

        private AnimationQueue() { }

        public static void CrossFadeQueued(AnimancerState state)
        {
            CleanUp();

            var layer = state.Layer;

            // If the layer has no current state, just play the animation immediately.
            if (!layer.CurrentState.IsValid() || layer.CurrentState.Weight == 0)
            {
                var fadeDuration = state.CalculateEditorFadeDuration(AnimancerPlayable.DefaultFadeDuration);
                layer.Play(state, fadeDuration);
                return;
            }

            if (!PlayableToQueue.TryGetValue(layer, out var queue))
            {
                queue = new AnimationQueue();
                PlayableToQueue.Add(layer, queue);
            }

            queue.Queue.Add(state);

            layer.CurrentState.Events.OnEnd -= queue.PlayNext;
            layer.CurrentState.Events.OnEnd += queue.PlayNext;
        }

        /************************************************************************************************************************/

        public static void ClearQueue(AnimancerLayer layer)
        {
            PlayableToQueue.Remove(layer);
        }

        /************************************************************************************************************************/

        private static readonly List<AnimancerLayer>
            OldQueues = new List<AnimancerLayer>();

        /// <summary>
        /// Clear out any playables that have been destroyed.
        /// </summary>
        private static void CleanUp()
        {
            OldQueues.Clear();

            foreach (var layer in PlayableToQueue.Keys)
            {
                if (!layer.IsValid)
                    OldQueues.Add(layer);
            }

            for (int i = 0; i < OldQueues.Count; i++)
            {
                PlayableToQueue.Remove(OldQueues[i]);
            }
        }

        /************************************************************************************************************************/

        private void PlayNext()
        {
            if (Queue.Count == 0)
                return;

            var state = Queue[0];
            Queue.RemoveAt(0);
            if (!state.IsValid())
            {
                PlayNext();
                return;
            }

            var fadeDuration = state.CalculateEditorFadeDuration(AnimancerPlayable.DefaultFadeDuration);
            state.Layer.Play(state, fadeDuration);
            state.Events.OnEnd = PlayNext;
        }

        /************************************************************************************************************************/
    }
}

#endif

