// Animancer // https://kybernetik.com.au/animancer // Copyright 2020 Kybernetik //

using UnityEngine;

namespace Animancer.FSM
{
    /// <summary>
    /// A state that can be used in a <see cref="StateMachine{TState}"/>.
    /// </summary>
    public interface IState<TState> where TState : class, IState<TState>
    {
        /// <summary>Determines whether this state can be entered.</summary>
        bool CanEnterState(TState previousState);

        /// <summary>Determines whether this state can be exited.</summary>
        bool CanExitState(TState nextState);

        /// <summary>Called when this state is entered.</summary>
        void OnEnterState();

        /// <summary>Called when this state is exited.</summary>
        void OnExitState();
    }

    /************************************************************************************************************************/

    /// <summary>
    /// A type of <see cref="IState{TState}"/> that knows which <see cref="StateMachine{TState}"/> it is used in so it
    /// can be used with various extension methods in <see cref="StateExtensions"/>.
    /// </summary>
    public interface IOwnedState<TState> : IState<TState> where TState : class, IState<TState>
    {
        /// <summary>The <see cref="StateMachine{TState}"/> that this state is used in.</summary>
        StateMachine<TState> OwnerStateMachine { get; }
    }

    /************************************************************************************************************************/

    /// <summary>Various extension methods for <see cref="IOwnedState{TState}"/>.</summary>
    /// 
    /// <example><code>
    /// public class Creature : MonoBehaviour
    /// {
    ///     public StateMachine&lt;CreatureState&gt; StateMachine { get; private set; }
    /// }
    /// 
    /// public class CreatureState : StateBehaviour, IOwnedState&lt;CreatureState&gt;
    /// {
    ///     [SerializeField]
    ///     private Creature _Creature;
    ///     public Creature Creature =&gt; _Creature;
    ///     
    ///     public StateMachine&lt;CreatureState&gt; OwnerStateMachine =&gt; _Creature.StateMachine;
    /// }
    /// 
    /// public class CreatureBrain : MonoBehaviour
    /// {
    ///     [SerializeField] private Creature _Creature;
    ///     [SerializeField] private CreatureState _Jump;
    ///     
    ///     private void Update()
    ///     {
    ///         if (Input.GetKeyDown(KeyCode.Space))
    ///         {
    ///             // Normally you would need to refer to both the state machine and the state:
    ///             _Creature.StateMachine.TrySetState(_Jump);
    ///             
    ///             // But since CreatureState implements IOwnedState you can use these extension methods:
    ///             _Jump.TryEnterState();
    ///         }
    ///     }
    /// }
    /// </code>
    /// <h2>Inherited Types</h2>
    /// Unfortunately, if the field type is not the same as the <c>T</c> in the <c>IOwnedState&lt;T&gt;</c>
    /// implementation then attempting to use these extension methods without specifying the generic argument will
    /// give the following error:
    /// <para></para>
    /// <em>The type 'StateType' cannot be used as type parameter 'TState' in the generic type or method
    /// 'StateExtensions.TryEnterState&lt;TState&gt;(TState)'. There is no implicit reference conversion from
    /// 'StateType' to 'Animancer.FSM.IOwnedState&lt;StateType&gt;'.</em>
    /// <para></para>
    /// For example, you might want to access members of a derived state class like this <c>SetTarget</c> method:
    /// <para></para><code>
    /// public class AttackState : CreatureState
    /// {
    ///     public void SetTarget(Transform target) { }
    /// }
    /// 
    /// public class CreatureBrain : MonoBehaviour
    /// {
    ///     [SerializeField] private AttackState _Attack;
    ///     
    ///     private void Update()
    ///     {
    ///         if (Input.GetMouseButtonDown(0))
    ///         {
    ///             _Attack.SetTarget(...)
    ///             // Can't do _Attack.TryEnterState();
    ///             _Attack.TryEnterState&lt;CreatureState&gt;();
    ///         }
    ///     }
    /// }
    /// </code>
    /// Unlike the <c>_Jump</c> example, the <c>_Attack</c> field is an <c>AttackState</c> rather than the base
    /// <c>CreatureState</c> so we can call <c>_Attack.SetTarget(...)</c> but that causes problems with these extension
    /// methods.
    /// <para></para>
    /// Calling the method without specifying its generic argument automatically uses the variable's type as the
    /// argument so both of the following calls do the same thing:
    /// <para></para><code>
    /// _Attack.TryEnterState();
    /// _Attack.TryEnterState&lt;AttackState&gt;();
    /// </code>
    /// <para></para>
    /// The problem is that <c>AttackState</c> inherits the implementation of <c>IOwnedState</c> from the base
    /// <c>CreatureState</c> class. But since that implementation is <c>IOwnedState&lt;CreatureState&gt;</c>, rather
    /// than <c>IOwnedState&lt;AttackState&gt;</c> that means <c>TryEnterState&lt;AttackState&gt;</c> does not satisfy
    /// that method's generic constraints: <c>where TState : class, IOwnedState&lt;TState&gt;</c>
    /// <para></para>
    /// That is why you simply need to specify the base class which implements <c>IOwnedState</c> as the generic
    /// argument to prevent it from inferring the wrong type:
    /// <para></para><code>
    /// _Attack.TryEnterState&lt;CreatureState&gt;();
    /// </code></example>
    /// 
    /// https://kybernetik.com.au/animancer/api/Animancer.FSM/StateExtensions
    /// 
    [HelpURL(APIDocumentationURL + nameof(StateExtensions))]
    public static class StateExtensions
    {
        /************************************************************************************************************************/

        /// <summary>The URL of the API documentation for the <see cref="FSM"/> system.</summary>
        public const string APIDocumentationURL = "https://kybernetik.com.au/animancer/api/Animancer.FSM/";

        /************************************************************************************************************************/

        /// <summary>
        /// Checks if the specified `state` is the <see cref="StateMachine{TState}.CurrentState"/> in its
        /// <see cref="IOwnedState{TState}.OwnerStateMachine"/>.
        /// </summary>
        public static bool IsCurrentState<TState>(this TState state)
            where TState : class, IOwnedState<TState>
            => state.OwnerStateMachine.CurrentState == state;

        /************************************************************************************************************************/

        /// <summary>
        /// Checks if it is currently possible to enter the specified `state`. This requires
        /// <see cref="IState{TState}.CanExitState"/> on the <see cref="StateMachine{TState}.CurrentState"/> and
        /// <see cref="IState{TState}.CanEnterState"/> on the specified `state` to both return true.
        /// </summary>
        public static bool CanEnterState<TState>(this TState state)
            where TState : class, IOwnedState<TState>
            => state.OwnerStateMachine.CanSetState(state);

        /************************************************************************************************************************/

        /// <summary>
        /// Attempts to enter the specified `state` and returns true if successful.
        /// <para></para>
        /// This method returns true immediately if the specified `state` is already the
        /// <see cref="StateMachine{TState}.CurrentState"/>. To allow directly re-entering the same state, use
        /// <see cref="TryReEnterState"/> instead.
        /// </summary>
        public static bool TryEnterState<TState>(this TState state)
            where TState : class, IOwnedState<TState>
            => state.OwnerStateMachine.TrySetState(state);

        /************************************************************************************************************************/

        /// <summary>
        /// Attempts to enter the specified `state` and returns true if successful.
        /// <para></para>
        /// This method does not check if the `state` is already the <see cref="StateMachine{TState}.CurrentState"/>.
        /// To do so, use <see cref="TryEnterState"/> instead.
        /// </summary>
        public static bool TryReEnterState<TState>(this TState state)
            where TState : class, IOwnedState<TState>
            => state.OwnerStateMachine.TryResetState(state);

        /************************************************************************************************************************/

        /// <summary>
        /// Calls <see cref="IState{TState}.OnExitState"/> on the <see cref="StateMachine{TState}.CurrentState"/> then
        /// changes to the specified `state` and calls <see cref="IState{TState}.OnEnterState"/> on it.
        /// <para></para>
        /// This method does not check <see cref="IState{TState}.CanExitState"/> or
        /// <see cref="IState{TState}.CanEnterState"/>. To do that, you should use <see cref="TrySetState"/> instead.
        /// </summary>
        public static void ForceEnterState<TState>(this TState state)
            where TState : class, IOwnedState<TState>
            => state.OwnerStateMachine.ForceSetState(state);

        /************************************************************************************************************************/
    }
}
