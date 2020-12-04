// Animancer // https://kybernetik.com.au/animancer // Copyright 2020 Kybernetik //

using System;
using UnityEngine;

namespace Animancer
{
    /// <summary>[Pro-Only] A <see cref="ControllerState"/> which manages two float parameters.</summary>
    /// <remarks>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/animator-controllers">Animator Controllers</see>
    /// </remarks>
    /// <seealso cref="Float1ControllerState"/>
    /// <seealso cref="Float3ControllerState"/>
    /// https://kybernetik.com.au/animancer/api/Animancer/Float2ControllerState
    /// 
    public sealed class Float2ControllerState : ControllerState
    {
        /************************************************************************************************************************/

        private Parameter _ParameterX;

        /// <summary>
        /// The name of the parameter which <see cref="ParameterX"/> will get and set.
        /// This will be null if the <see cref="ParameterHashX"/> was assigned directly.
        /// </summary>
        public string ParameterNameX
        {
            get => _ParameterX.Name;
            set
            {
                _ParameterX.Name = value;
                _ParameterX.ValidateHasParameter(Controller, AnimatorControllerParameterType.Float);
            }
        }

        /// <summary>
        /// The name hash of the parameter which <see cref="ParameterX"/> will get and set.
        /// </summary>
        public int ParameterHashX
        {
            get => _ParameterX.Hash;
            set
            {
                _ParameterX.Hash = value;
                _ParameterX.ValidateHasParameter(Controller, AnimatorControllerParameterType.Float);
            }
        }

        /// <summary>
        /// Gets and sets a float parameter in the <see cref="ControllerState.Controller"/> using the
        /// <see cref="ParameterHashX"/> as the id.
        /// </summary>
        public float ParameterX
        {
            get => Playable.GetFloat(_ParameterX);
            set => Playable.SetFloat(_ParameterX, value);
        }

        /************************************************************************************************************************/

        private Parameter _ParameterY;

        /// <summary>
        /// The name of the parameter which <see cref="ParameterY"/> will get and set.
        /// This will be null if the <see cref="ParameterHashY"/> was assigned directly.
        /// </summary>
        public string ParameterNameY
        {
            get => _ParameterY.Name;
            set
            {
                _ParameterY.Name = value;
                _ParameterY.ValidateHasParameter(Controller, AnimatorControllerParameterType.Float);
            }
        }

        /// <summary>
        /// The name hash of the parameter which <see cref="ParameterY"/> will get and set.
        /// </summary>
        public int ParameterHashY
        {
            get => _ParameterY.Hash;
            set
            {
                _ParameterY.Hash = value;
                _ParameterY.ValidateHasParameter(Controller, AnimatorControllerParameterType.Float);
            }
        }

        /// <summary>
        /// Gets and sets a float parameter in the <see cref="ControllerState.Controller"/> using the
        /// <see cref="ParameterHashY"/> as the id.
        /// </summary>
        public float ParameterY
        {
            get => Playable.GetFloat(_ParameterY);
            set => Playable.SetFloat(_ParameterY, value);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Gets and sets <see cref="ParameterX"/> and <see cref="ParameterY"/>.
        /// </summary>
        public new Vector2 Parameter
        {
            get => new Vector2(ParameterX, ParameterY);
            set
            {
                ParameterX = value.x;
                ParameterY = value.y;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Constructs a new <see cref="Float2ControllerState"/> to play the `controller`.</summary>
        public Float2ControllerState(RuntimeAnimatorController controller,
            Parameter parameterX, Parameter parameterY, bool resetStatesOnStop = true)
            : base(controller, resetStatesOnStop)
        {
            _ParameterX = parameterX;
            _ParameterX.ValidateHasParameter(Controller, AnimatorControllerParameterType.Float);

            _ParameterY = parameterY;
            _ParameterY.ValidateHasParameter(Controller, AnimatorControllerParameterType.Float);
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override int ParameterCount => 2;

        /// <inheritdoc/>
        public override int GetParameterHash(int index)
        {
            switch (index)
            {
                case 0: return ParameterHashX;
                case 1: return ParameterHashY;
                default: throw new ArgumentOutOfRangeException(nameof(index));
            };
        }

        /************************************************************************************************************************/
        #region Transition
        /************************************************************************************************************************/

        /// <summary>
        /// A serializable <see cref="ITransition"/> which can create a <see cref="Float2ControllerState"/>
        /// when passed into <see cref="AnimancerPlayable.Play(ITransition)"/>.
        /// </summary>
        /// <remarks>
        /// Unfortunately the tool used to generate this documentation does not currently support nested types with
        /// identical names, so only one <c>Transition</c> class will actually have a documentation page.
        /// <para></para>
        /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/transitions">Transitions</see>
        /// </remarks>
        /// https://kybernetik.com.au/animancer/api/Animancer/Transition
        /// 
        [Serializable]
        public new class Transition : Transition<Float2ControllerState>
        {
            /************************************************************************************************************************/

            [SerializeField]
            private string _ParameterNameX;

            /// <summary>[<see cref="SerializeField"/>]
            /// The <see cref="Float2ControllerState.ParameterNameX"/> that will be used for the created state.
            /// </summary>
            public ref string ParameterNameX => ref _ParameterNameX;

            /************************************************************************************************************************/

            [SerializeField]
            private string _ParameterNameY;

            /// <summary>[<see cref="SerializeField"/>]
            /// The <see cref="Float2ControllerState.ParameterNameY"/> that will be used for the created state.
            /// </summary>
            public ref string ParameterNameY => ref _ParameterNameY;

            /************************************************************************************************************************/

            /// <summary>Constructs a new <see cref="Transition"/>.</summary>
            public Transition() { }

            /// <summary>Constructs a new <see cref="Transition"/> with the specified Animator Controller and parameters.</summary>
            public Transition(RuntimeAnimatorController controller, string parameterNameX, string parameterNameY)
            {
                Controller = controller;
                _ParameterNameX = parameterNameX;
                _ParameterNameY = parameterNameY;
            }

            /************************************************************************************************************************/

            /// <summary>Creates and returns a new <see cref="Float2ControllerState"/>.</summary>
            /// <remarks>
            /// Note that using methods like <see cref="AnimancerPlayable.Play(ITransition)"/> will also call
            /// <see cref="ITransition.Apply"/>, so if you call this method manually you may want to call that method
            /// as well. Or you can just use <see cref="AnimancerUtilities.CreateStateAndApply"/>.
            /// <para></para>
            /// This method also assigns it as the <see cref="AnimancerState.Transition{TState}.State"/>.
            /// </remarks>
            public override Float2ControllerState CreateState()
                => State = new Float2ControllerState(Controller, _ParameterNameX, _ParameterNameY, KeepStateOnStop);

            /************************************************************************************************************************/
            #region Drawer
#if UNITY_EDITOR
            /************************************************************************************************************************/

            /// <summary>[Editor-Only] Draws the Inspector GUI for a <see cref="Transition"/>.</summary>
            /// <remarks>
            /// Unfortunately the tool used to generate this documentation does not currently support nested types with
            /// identical names, so only one <c>Drawer</c> class will actually have a documentation page.
            /// <para></para>
            /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/transitions">Transitions</see>
            /// </remarks>
            [UnityEditor.CustomPropertyDrawer(typeof(Transition), true)]
            public class Drawer : ControllerState.Transition.Drawer
            {
                /************************************************************************************************************************/

                /// <summary>
                /// Constructs a new <see cref="Drawer"/> and sets the
                /// <see cref="ControllerState.Transition.Drawer.Parameters"/>.
                /// </summary>
                public Drawer() : base(nameof(_ParameterNameX), nameof(_ParameterNameY)) { }

                /************************************************************************************************************************/
            }

            /************************************************************************************************************************/
#endif
            #endregion
            /************************************************************************************************************************/
        }

        /************************************************************************************************************************/
        #endregion
        /************************************************************************************************************************/
    }
}

