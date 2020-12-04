// Animancer // https://kybernetik.com.au/animancer // Copyright 2020 Kybernetik //

using System;
using UnityEngine;

namespace Animancer
{
    /// <summary>[Pro-Only] A <see cref="ControllerState"/> which manages three float parameters.</summary>
    /// <remarks>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/animator-controllers">Animator Controllers</see>
    /// </remarks>
    /// <seealso cref="Float1ControllerState"/>
    /// <seealso cref="Float2ControllerState"/>
    /// https://kybernetik.com.au/animancer/api/Animancer/Float3ControllerState
    /// 
    public sealed class Float3ControllerState : ControllerState
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

        private Parameter _ParameterZ;

        /// <summary>
        /// The name of the parameter which <see cref="ParameterZ"/> will get and set.
        /// This will be null if the <see cref="ParameterHashZ"/> was assigned directly.
        /// </summary>
        public string ParameterNameZ
        {
            get => _ParameterZ.Name;
            set
            {
                _ParameterZ.Name = value;
                _ParameterZ.ValidateHasParameter(Controller, AnimatorControllerParameterType.Float);
            }
        }

        /// <summary>
        /// The name hash of the parameter which <see cref="ParameterZ"/> will get and set.
        /// </summary>
        public int ParameterHashZ
        {
            get => _ParameterZ.Hash;
            set
            {
                _ParameterZ.Hash = value;
                _ParameterZ.ValidateHasParameter(Controller, AnimatorControllerParameterType.Float);
            }
        }

        /// <summary>
        /// Gets and sets a float parameter in the <see cref="ControllerState.Controller"/> using the
        /// <see cref="ParameterHashZ"/> as the id.
        /// </summary>
        public float ParameterZ
        {
            get => Playable.GetFloat(_ParameterZ);
            set => Playable.SetFloat(_ParameterZ, value);
        }

        /************************************************************************************************************************/

        /// <summary>
        /// Gets and sets <see cref="ParameterX"/>, <see cref="ParameterY"/>, and <see cref="ParameterZ"/>.
        /// </summary>
        public new Vector3 Parameter
        {
            get => new Vector3(ParameterX, ParameterY, ParameterZ);
            set
            {
                ParameterX = value.x;
                ParameterY = value.y;
                ParameterZ = value.z;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Constructs a new <see cref="Float3ControllerState"/> to play the `controller`.</summary>
        public Float3ControllerState(RuntimeAnimatorController controller,
            Parameter parameterX, Parameter parameterY, Parameter parameterZ, bool resetStatesOnStop = true)
            : base(controller, resetStatesOnStop)
        {
            _ParameterX = parameterX;
            _ParameterX.ValidateHasParameter(Controller, AnimatorControllerParameterType.Float);

            _ParameterY = parameterY;
            _ParameterY.ValidateHasParameter(Controller, AnimatorControllerParameterType.Float);

            _ParameterZ = parameterZ;
            _ParameterZ.ValidateHasParameter(Controller, AnimatorControllerParameterType.Float);
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override int ParameterCount => 3;

        /// <inheritdoc/>
        public override int GetParameterHash(int index)
        {
            switch (index)
            {
                case 0: return ParameterHashX;
                case 1: return ParameterHashY;
                case 2: return ParameterHashZ;
                default: throw new ArgumentOutOfRangeException(nameof(index));
            };
        }

        /************************************************************************************************************************/
        #region Transition
        /************************************************************************************************************************/

        /// <summary>
        /// A serializable <see cref="ITransition"/> which can create a <see cref="Float3ControllerState"/>
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
        public new class Transition : Transition<Float3ControllerState>
        {
            /************************************************************************************************************************/

            [SerializeField]
            private string _ParameterNameX;

            /// <summary>[<see cref="SerializeField"/>]
            /// The <see cref="Float3ControllerState.ParameterNameX"/> that will be used for the created state.
            /// </summary>
            public ref string ParameterNameX => ref _ParameterNameX;

            /************************************************************************************************************************/

            [SerializeField]
            private string _ParameterNameY;

            /// <summary>[<see cref="SerializeField"/>]
            /// The <see cref="Float3ControllerState.ParameterNameY"/> that will be used for the created state.
            /// </summary>
            public ref string ParameterNameY => ref _ParameterNameY;

            /************************************************************************************************************************/

            [SerializeField]
            private string _ParameterNameZ;

            /// <summary>[<see cref="SerializeField"/>]
            /// The <see cref="Float3ControllerState.ParameterNameZ"/> that will be used for the created state.
            /// </summary>
            public ref string ParameterNameZ => ref _ParameterNameZ;

            /************************************************************************************************************************/

            /// <summary>Constructs a new <see cref="Transition"/>.</summary>
            public Transition() { }

            /// <summary>Constructs a new <see cref="Transition"/> with the specified Animator Controller and parameters.</summary>
            public Transition(RuntimeAnimatorController controller,
                string parameterNameX, string parameterNameY, string parameterNameZ)
            {
                Controller = controller;
                _ParameterNameX = parameterNameX;
                _ParameterNameY = parameterNameY;
                _ParameterNameZ = parameterNameZ;
            }

            /************************************************************************************************************************/

            /// <summary>Creates and returns a new <see cref="Float3ControllerState"/>.</summary>
            /// <remarks>
            /// Note that using methods like <see cref="AnimancerPlayable.Play(ITransition)"/> will also call
            /// <see cref="ITransition.Apply"/>, so if you call this method manually you may want to call that method
            /// as well. Or you can just use <see cref="AnimancerUtilities.CreateStateAndApply"/>.
            /// <para></para>
            /// This method also assigns it as the <see cref="AnimancerState.Transition{TState}.State"/>.
            /// </remarks>
            public override Float3ControllerState CreateState()
                => State = new Float3ControllerState(Controller, _ParameterNameX, _ParameterNameY, _ParameterNameZ, KeepStateOnStop);

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
                public Drawer() : base(nameof(_ParameterNameX), nameof(_ParameterNameY), nameof(_ParameterNameZ)) { }

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

