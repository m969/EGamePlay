// Animancer // https://kybernetik.com.au/animancer // Copyright 2020 Kybernetik //

using UnityEngine;

namespace Animancer.FSM
{
    /// <summary>
    /// Base class for <see cref="MonoBehaviour"/> states to be used in a <see cref="StateMachine{TState}"/>.
    /// </summary>
    [HelpURL(StateExtensions.APIDocumentationURL + nameof(StateBehaviour<TState>) + "_1")]
    public abstract class StateBehaviour<TState> : MonoBehaviour, IState<TState>
        where TState : StateBehaviour<TState>
    {
        /************************************************************************************************************************/

        /// <summary>
        /// Determines whether the <see cref="StateMachine{TState}"/> can enter this state.
        /// Always returns true unless overridden.
        /// </summary>
        public virtual bool CanEnterState(TState previousState) => true;

        /// <summary>
        /// Determines whether the <see cref="StateMachine{TState}"/> can exit this state.
        /// Always returns true unless overridden.
        /// </summary>
        public virtual bool CanExitState(TState nextState) => true;

        /************************************************************************************************************************/

        /// <summary>
        /// Asserts that this component is not already enabled, then enables it.
        /// </summary>
        public virtual void OnEnterState()
        {
#if UNITY_ASSERTIONS
            if (enabled)
                Debug.LogError($"{this} was already enabled when entering its state", this);
#endif
            enabled = true;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Asserts that this component is not already disabled, then disables it.
        /// </summary>
        public virtual void OnExitState()
        {
            if (this == null)
                return;

#if UNITY_ASSERTIONS
            if (!enabled)
                Debug.LogError($"{this} was already disabled when exiting its state", this);
#endif

            enabled = false;
        }

        /************************************************************************************************************************/

#if UNITY_EDITOR
        /// <summary>[Editor-Only]
        /// Called by the Unity Editor in Edit Mode whenever an instance of this script is loaded or a value is changed
        /// in the Inspector.
        /// <para></para>
        /// States start disabled and only the current state gets enabled at runtime.
        /// </summary>
        protected virtual void OnValidate()
        {
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            enabled = false;
        }
#endif

        /************************************************************************************************************************/
    }
}
