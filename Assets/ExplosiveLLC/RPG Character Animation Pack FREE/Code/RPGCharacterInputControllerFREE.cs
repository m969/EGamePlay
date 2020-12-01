using UnityEngine;
using UnityEngine.InputSystem;

namespace RPGCharacterAnimsFREE
{
    public class RPGCharacterInputControllerFREE:MonoBehaviour
    {
		//InputSystem
		public @RPGFREEInputs rpgFREEInputs;

		//Inputs.
		[HideInInspector] public Vector2 inputMovement;
		[HideInInspector] public Vector2 inputMouseFacing;
		[HideInInspector] public bool inputJump;
        [HideInInspector] public bool inputLightHit;
        [HideInInspector] public bool inputDeath;
        [HideInInspector] public bool inputAttackL;
        [HideInInspector] public bool inputAttackR;
		[HideInInspector] public bool inputKickL;
		[HideInInspector] public bool inputKickR;
		[HideInInspector] public float inputBlock = 0;
		[HideInInspector] public bool inputTarget;
		[HideInInspector] public bool inputFace;
		[HideInInspector] public bool inputRoll;
		[HideInInspector] public bool inputSwitchWeapon;

		//Variables.
		[HideInInspector] public bool allowedInput = true;
        [HideInInspector] public Vector3 moveInput;
        [HideInInspector] public Vector2 aimInput;

        private void Awake()
        {
            allowedInput = true;
			rpgFREEInputs = new @RPGFREEInputs();
		}

		private void OnEnable()
		{
			rpgFREEInputs.Enable();
		}

		private void OnDisable()
		{
			rpgFREEInputs.Disable();
		}

		private void Update()
        {
            Inputs();
			moveInput = CameraRelativeInput(inputMovement.x, inputMovement.y);
			HasJoystickConnected();
        }

		/// <summary>
		/// Input abstraction for easier asset updates using outside control schemes.
		/// </summary>
		private void Inputs()
        {
			try
			{
				inputMouseFacing = Mouse.current.position.ReadValue();
				inputMovement = rpgFREEInputs.RPGCharacter.Move.ReadValue<Vector2>();
				inputJump = rpgFREEInputs.RPGCharacter.Jump.WasPressedThisFrame();
				inputLightHit = rpgFREEInputs.RPGCharacter.LightHit.WasPressedThisFrame();
				inputDeath = rpgFREEInputs.RPGCharacter.Death.WasPressedThisFrame();
				inputAttackL = rpgFREEInputs.RPGCharacter.AttackL.WasPressedThisFrame();
				inputAttackR = rpgFREEInputs.RPGCharacter.AttackR.WasPressedThisFrame();
				inputKickL = rpgFREEInputs.RPGCharacter.KickL.WasPressedThisFrame();
				inputKickR = rpgFREEInputs.RPGCharacter.KickR.WasPressedThisFrame();
				inputBlock = rpgFREEInputs.RPGCharacter.Block.ReadValue<float>();
				inputTarget = rpgFREEInputs.RPGCharacter.Targeting.IsPressed();
				inputFace = rpgFREEInputs.RPGCharacter.Face.IsPressed();
				inputRoll = rpgFREEInputs.RPGCharacter.Roll.WasPressedThisFrame();

			}
			catch(System.Exception)
			{
				Debug.LogWarning("INPUTS NOT FOUND! PLEASE SEE THE INCLUDED README FILE!");
			}
		}


        /// <summary>
        /// Movement based off camera facing.
        /// </summary>
        private Vector3 CameraRelativeInput(float inputX, float inputZ)
        {
			//Forward vector relative to the camera along the x-z plane.
			Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
			forward.y = 0;
			forward = forward.normalized;
			//Right vector relative to the camera always orthogonal to the forward vector.
			Vector3 right = new Vector3(forward.z, 0, -forward.x);
			Vector3 relativeVelocity = inputMovement.x * right + inputMovement.y * forward;
			//Reduce input for diagonal movement.
			if(relativeVelocity.magnitude > 1)
			{
				relativeVelocity.Normalize();
			}
			return relativeVelocity; ;
        }

		public bool HasAnyInput()
		{
			if(allowedInput && HasMoveInput() && inputJump != false)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool HasMoveInput()
		{
			if(allowedInput && inputMovement != Vector2.zero)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool HasJoystickConnected()
		{
			//No joysticks.
			if(Input.GetJoystickNames().Length == 0)
			{
				//Debug.Log("No Joystick Connected");
				return false;
			}
			else
			{
				//Debug.Log("Joystick Connected");
				//If joystick is plugged in.
				for(int i = 0; i < Input.GetJoystickNames().Length; i++)
				{
					//Debug.Log(Input.GetJoystickNames()[i].ToString());
				}
				return true;
			}
		}

		public bool HasBlockInput()
		{
			if(inputBlock != 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	//Extension Method to allow checking InputSystem without Action Callbacks.
	public static class InputActionExtensions
	{
		public static bool IsPressed(this InputAction inputAction)
		{
			return inputAction.ReadValue<float>() > 0f;
		}

		public static bool WasPressedThisFrame(this InputAction inputAction)
		{
			return inputAction.triggered && inputAction.ReadValue<float>() > 0f;
		}

		public static bool WasReleasedThisFrame(this InputAction inputAction)
		{
			return inputAction.triggered && inputAction.ReadValue<float>() == 0f;
		}
	}
}