// Animancer // https://kybernetik.com.au/animancer // Copyright 2020 Kybernetik //

using System;
using UnityEngine;

namespace Animancer
{
    /// <summary>[Pro-Only] A <see cref="ControllerState"/> which manages one float parameter.</summary>
    /// <remarks>
    /// Documentation: <see href="https://kybernetik.com.au/animancer/docs/manual/animator-controllers">Animator Controllers</see>
    /// </remarks>
    /// <seealso cref="Float2ControllerState"/>
    /// <seealso cref="Float3ControllerState"/>
    /// https://kybernetik.com.au/animancer/api/Animancer/Float1ControllerState
    /// 
    public sealed class Float1ControllerState : ControllerState
    {
        /************************************************************************************************************************/

        private Parameter _Parameter;

        /// <summary>
        /// The name of the parameter which <see cref="Parameter"/> will get and set.
        /// This will be null if the <see cref="ParameterHash"/> was assigned directly.
        /// </summary>
        public string ParameterName
        {
            get => _Parameter.Name;
            set
            {
                _Parameter.Name = value;
                _Parameter.ValidateHasParameter(Controller, AnimatorControllerParameterType.Float);
            }
        }

        /// <summary>
        /// The name hash of the parameter which <see cref="Parameter"/> will get and set.
        /// </summary>
        public int ParameterHash
        {
            get => _Parameter.Hash;
            set
            {
                _Parameter.Hash = value;
                _Parameter.ValidateHasParameter(Controller, AnimatorControllerParameterType.Float);
            }
        }

        /// <summary>
        /// Gets and sets a float parameter in the <see cref="ControllerState.Controller"/> using the
        /// <see cref="ParameterHash"/> as the id.
        /// </summary>
        public new float Parameter
        {
            get => Playable.GetFloat(_Parameter);
            set => Playable.SetFloat(_Parameter, value);
        }

        /************************************************************************************************************************/

        /// <summary>Constructs a new <see cref="Float1ControllerState"/> to play the `controller`.</summary>
        public Float1ControllerState(RuntimeAnimatorController controller, Parameter parameter,
            bool resetStatesOnStop = true)
            : base(controller, resetStatesOnStop)
        {
            _Parameter = parameter;
            _Parameter.ValidateHasParameter(controller, AnimatorControllerParameterType.Float);
        }

        /************************************************************************************************************************/

        /// <inheritdoc/>
        public override int ParameterCount => 1;

        /// <inheritdoc/>
        public override int GetParameterHash(int index) => ParameterHash;

        /************************************************************************************************************************/
        #region Transition
        /************************************************************************************************************************/

        /// <summary>
        /// A serializable <see cref="ITransition"/> which can create a <see cref="Float1ControllerState"/>
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
        public new class Transition : Transition<Float1ControllerState>
        {
            /************************************************************************************************************************/

            [SerializeField]
            private string _ParameterName;

            /// <summary>[<see cref="SerializeField"/>]
            /// The <see cref="Float1ControllerState.ParameterName"/> that will be used for the created state.
            /// </summary>
            public ref string ParameterName => ref _ParameterName;

            /************************************************************************************************************************/

            /// <summary>Constructs a new <see cref="Transition"/>.</summary>
            public Transition() { }

            /// <summary>Constructs a new <see cref="Transition"/> with the specified Animator Controller and parameter.</summary>
            public Transition(RuntimeAnimatorController controller, string parameterName)
            {
                Controller = controller;
                _ParameterName = parameterName;
            }

            /************************************************************************************************************************/

            /// <summary>Creates and returns a new <see cref="Float1ControllerState"/>.</summary>
            /// <remarks>
            /// Note that using methods like <see cref="AnimancerPlayable.Play(ITransition)"/> will also call
            /// <see cref="ITransition.Apply"/>, so if you call this method manually you may want to call that method
            /// as well. Or you can just use <see cref="AnimancerUtilities.CreateStateAndApply"/>.
            /// <para></para>
            /// This method also assigns it as the <see cref="AnimancerState.Transition{TState}.State"/>.
            /// </remarks>
            public override Float1ControllerState CreateState()
                => State = new Float1ControllerState(Controller, _ParameterName, KeepStateOnStop);

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
                public Drawer() : base(nameof(_ParameterName)) { }

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

