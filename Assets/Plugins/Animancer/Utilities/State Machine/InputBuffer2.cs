// Animancer // https://kybernetik.com.au/animancer // Copyright 2020 Kybernetik //

namespace Animancer.FSM
{
    public partial class StateMachine<TKey, TState>
    {
        /// <summary>
        /// A simple buffer that remembers any failed calls to
        /// <see cref="StateMachine{TKey, TState}.TrySetState(TKey, TState)"/> so that it can retry them each time you
        /// <see cref="Update"/> it until the <see cref="TimeOut"/> expires.
        /// </summary>
        public new class InputBuffer : StateMachine<TState>.InputBuffer
        {
            /************************************************************************************************************************/

            private StateMachine<TKey, TState> _StateMachine;

            /// <summary>The <see cref="StateMachine{TKey, TState}"/> this buffer is feeding input to.</summary>
            public new StateMachine<TKey, TState> StateMachine
            {
                get => _StateMachine;
                set
                {
                    _StateMachine = value;
                    base.StateMachine = value;
                }
            }

            /// <summary>The <typeparamref name="TKey"/> of the state this buffer is currently attempting to enter.</summary>
            public TKey BufferedKey { get; set; }

            /************************************************************************************************************************/

            /// <summary>
            /// Constructs a new <see cref="InputBuffer"/> targeting the specified `stateMachine`.
            /// </summary>
            public InputBuffer(StateMachine<TKey, TState> stateMachine)
                : base(stateMachine)
            {
                _StateMachine = stateMachine;
                BufferedKey = stateMachine.CurrentKey;
            }

            /************************************************************************************************************************/

            /// <summary>
            /// Attempts to enter the specified state and returns true if successful.
            /// Otherwise the state is remembered and attempted again every time
            /// <see cref="StateMachine{TState}.InputBuffer.Update"/> is called.
            /// </summary>
            public bool TrySetState(TKey key, TState state, float timeOut)
            {
                BufferedKey = key;
                return TrySetState(state, timeOut);
            }

            /// <summary>
            /// Attempts to enter the specified state and returns true if successful.
            /// Otherwise the state is remembered and attempted again every time
            /// <see cref="StateMachine{TState}.InputBuffer.Update"/> is called.
            /// </summary>
            public bool TrySetState(TKey key, float timeOut)
                => _StateMachine.TryGetValue(key, out var state) && TrySetState(key, state, timeOut);

            /************************************************************************************************************************/

            /// <summary>
            /// Attempts to enter the <see cref="BufferedState"/> and returns true if successful.
            /// </summary>
            protected override bool TryEnterBufferedState() => _StateMachine.TrySetState(BufferedKey, BufferedState);

            /************************************************************************************************************************/
        }
    }
}
