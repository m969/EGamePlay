// Animancer // https://kybernetik.com.au/animancer // Copyright 2020 Kybernetik //

using System;

namespace Animancer.FSM
{
    /// <summary>
    /// A state that uses delegates to define its behaviour in the <see cref="StateMachine{TState}"/>.
    /// </summary>
    public class DelegateState<TState> : IState<TState> where TState : class, IState<TState>
    {
        /************************************************************************************************************************/

        /// <summary>Determines whether this state can be entered. Null is treated as returning true.</summary>
        public Func<TState, bool> canEnter;

        /// <summary>Determines whether this state can be exited. Null is treated as returning true.</summary>
        public Func<TState, bool> canExit;

        /// <summary>Called when this state is entered.</summary>
        public Action onEnter;

        /// <summary>Called when this state is exited.</summary>
        public Action onExit;

        /************************************************************************************************************************/

        /// <summary>
        /// Constructs a new <see cref="DelegateState{TState}"/> with the provided delegates.
        /// </summary>
        /// <param name="canEnter">Determines whether this state can be entered. Null is treated as returning true.</param>
        /// <param name="canExit">Determines whether this state can be exited. Null is treated as returning true.</param>
        /// <param name="onEnter">Called when this state is entered.</param>
        /// <param name="onExit">Called when this state is exited.</param>
        public DelegateState(
            Func<TState, bool> canEnter = null,
            Func<TState, bool> canExit = null,
            Action onEnter = null,
            Action onExit = null)
        {
            this.canEnter = canEnter;
            this.canExit = canExit;
            this.onEnter = onEnter;
            this.onExit = onExit;
        }

        /************************************************************************************************************************/

        /// <summary>Calls <see cref="canEnter"/> to determine whether this state can be entered.</summary>
        bool IState<TState>.CanEnterState(TState previousState) => canEnter == null || canEnter(previousState);

        /************************************************************************************************************************/

        /// <summary>Calls <see cref="canExit"/> to determine whether this state can be exited.</summary>
        bool IState<TState>.CanExitState(TState nextState) => canExit == null || canExit(nextState);

        /************************************************************************************************************************/

        /// <summary>Calls <see cref="onEnter"/> when this state is entered.</summary>
        void IState<TState>.OnEnterState() => onEnter?.Invoke();

        /************************************************************************************************************************/

        /// <summary>Calls <see cref="onExit"/> when this state is exited.</summary>
        void IState<TState>.OnExitState() => onExit?.Invoke();

        /************************************************************************************************************************/
    }
}
