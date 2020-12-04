// Animancer // https://kybernetik.com.au/animancer // Copyright 2020 Kybernetik //

using System;
using UnityEngine;

namespace Animancer.FSM
{
    /// <summary>
    /// A simple keyless Finite State Machine system.
    /// <para></para>
    /// This class doesn't keep track of any states other than the currently active one.
    /// See <see cref="StateMachine{TKey, TState}"/> for a system that allows states to be pre-registered and accessed
    /// using a separate key.
    /// </summary>
    [HelpURL(StateExtensions.APIDocumentationURL + nameof(StateMachine<TState>) + "_1")]
    public partial class StateMachine<TState> where TState : class, IState<TState>
    {
        /************************************************************************************************************************/

        /// <summary>The current state.</summary>
        public TState CurrentState { get; private set; }

        /************************************************************************************************************************/

        /// <summary>
        /// Constructs a new <see cref="StateMachine{TState}"/>.
        /// </summary>
        public StateMachine() { }

        /// <summary>
        /// Constructs a new <see cref="StateMachine{TState}"/> and immediately enters the `defaultState`.
        /// </summary>
        public StateMachine(TState defaultState)
        {
            CurrentState = defaultState;
            defaultState.OnEnterState();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Checks if it is currently possible to enter the specified `state`. This requires
        /// <see cref="IState{TState}.CanExitState"/> on the <see cref="CurrentState"/> and
        /// <see cref="IState{TState}.CanEnterState"/> on the specified `state` to both return true.
        /// </summary>
        public bool CanSetState(TState state)
        {
            if (CurrentState != null && !CurrentState.CanExitState(state))
                return false;

            if (state != null && !state.CanEnterState(CurrentState))
                return false;

            return true;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Attempts to enter the specified `state` and returns true if successful.
        /// <para></para>
        /// This method returns true immediately if the specified `state` is already the <see cref="CurrentState"/>.
        /// To allow directly re-entering the same state, use <see cref="TryResetState"/> instead.
        /// </summary>
        public bool TrySetState(TState state)
        {
            if (CurrentState == state)
                return true;
            else
                return TryResetState(state);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Attempts to enter the specified `state` and returns true if successful.
        /// <para></para>
        /// This method does not check if the `state` is already the <see cref="CurrentState"/>. To do so, use
        /// <see cref="TrySetState"/> instead.
        /// </summary>
        public bool TryResetState(TState state)
        {
            if (!CanSetState(state))
                return false;

            ForceSetState(state);
            return true;
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Calls <see cref="IState{TState}.OnExitState"/> on the <see cref="CurrentState"/> then changes to the
        /// specified `state` and calls <see cref="IState{TState}.OnEnterState"/> on it.
        /// <para></para>
        /// This method does not check <see cref="IState{TState}.CanExitState"/> or
        /// <see cref="IState{TState}.CanEnterState"/>. To do that, you should use <see cref="TrySetState"/> instead.
        /// </summary>
        public void ForceSetState(TState state)
        {
#if UNITY_EDITOR
            var owned = state as IOwnedState<TState>;
            if (owned != null && owned.OwnerStateMachine != this)
            {
                throw new InvalidOperationException(
                    "You are attempting to use a state in a machine that is not its owner." +
                    "\n    State: " + state +
                    "\n    Machine: " + ToString());
            }
#endif

            if (CurrentState != null)
                CurrentState.OnExitState();

            CurrentState = state;

            if (CurrentState != null)
                state.OnEnterState();
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Returns a string describing the type of this state machine and its <see cref="CurrentState"/>.
        /// </summary>
        public override string ToString() => GetType().Name + " -> " + CurrentState;

        /************************************************************************************************************************/
    }
}
