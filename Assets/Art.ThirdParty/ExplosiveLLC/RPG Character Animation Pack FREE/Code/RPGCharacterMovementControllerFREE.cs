using System.Collections;
using UnityEngine;

namespace RPGCharacterAnimsFREE
{
    public enum RPGCharacterState
    {
        Idle = 0,
        Move = 1,
        Jump = 2,
        DoubleJump = 3,
        Fall = 4,
        Block = 5,
        Roll = 6
    }

    public class RPGCharacterMovementControllerFREE:SuperStateMachine
    {
        //Components.
        [HideInInspector] public UnityEngine.AI.NavMeshAgent navMeshAgent;
        private SuperCharacterController superCharacterController;
        private RPGCharacterControllerFREE rpgCharacterController;
        private RPGCharacterInputControllerFREE rpgCharacterInputController;
        private Rigidbody rb;
        private Animator animator;
        private CapsuleCollider capCollider;
        public RPGCharacterState rpgCharacterState;

        [HideInInspector] public bool useMeshNav = false;
        [HideInInspector] public Vector3 lookDirection { get; private set; }
        [HideInInspector] public bool isKnockback;
        public float knockbackMultiplier = 1f;

        //Jumping.
        [HideInInspector] public bool canJump;
        [HideInInspector] public bool canDoubleJump = false;
        private bool doublejumped = false;
        public float gravity = 25.0f;
        public float jumpAcceleration = 5.0f;
        public float jumpHeight = 3.0f;
        public float doubleJumpHeight = 4f;

        //Movement.
        [HideInInspector] public Vector3 currentVelocity;
        [HideInInspector] public bool isMoving = false;
        [HideInInspector] public bool canMove = true;
        public float movementAcceleration = 90.0f;
        public float walkSpeed = 3.5f;
        public float runSpeed = 6f;
        private readonly float rotationSpeed = 40f;
        public float groundFriction = 50f;

        //Rolling.
        [HideInInspector] public bool isRolling = false;
        public float rollSpeed = 8;
        public float rollduration = 0.55f;
        private int rollNumber;

        //Air control.
        public float inAirSpeed = 6f;

        //Swimming.
        public float swimSpeed = 4f;
        public float waterFriction = 3f;

        private void Start()
        {
			//Get other RPG Character components.
			superCharacterController = GetComponent<SuperCharacterController>();
            rpgCharacterController = GetComponent<RPGCharacterControllerFREE>();
            rpgCharacterInputController = GetComponent<RPGCharacterInputControllerFREE>();
            navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

			//Check if Animator exists, otherwise pause script.
			animator = GetComponentInChildren<Animator>();
			if(animator == null)
			{
				Destroy(this);
				return;
			}

			//Setup Collider and Rigidbody for collisions.
			capCollider = GetComponent<CapsuleCollider>();
            rb = GetComponent<Rigidbody>();
            if(rb != null)
            {
                //Set restraints on startup if using Rigidbody.
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }

			SwitchCollisionOn();

            //Set currentState to idle on startup.
            currentState = RPGCharacterState.Idle;
            rpgCharacterState = RPGCharacterState.Idle;
        }

		#region Updates

		/*
		Update is normally run once on every frame update. We won't be using it in this case, since the SuperCharacterController component sends a callback Update called SuperUpdate. 
		SuperUpdate is recieved by the SuperStateMachine, and then fires further callbacks depending on the state
		void Update() 
		{
		}
		*/

		//Put any code in here you want to run BEFORE the state's update function. This is run regardless of what state you're in.
		protected override void EarlyGlobalSuperUpdate()
        {
        }

        //Put any code in here you want to run AFTER the state's update function.  This is run regardless of what state you're in.
        protected override void LateGlobalSuperUpdate()
        {
            //Move the player by our velocity every frame.
            transform.position += currentVelocity * superCharacterController.deltaTime;
            //If using Navmesh nagivation, update values.
            if(navMeshAgent != null)
            {
                if(useMeshNav)
                {
                    if(navMeshAgent.velocity.sqrMagnitude > 0)
                    {
                        animator.SetBool("Moving", true);
                        animator.SetFloat("Velocity Z", navMeshAgent.velocity.magnitude);
						return;
                    }
                    else
                    {
                        animator.SetFloat("Velocity Z", 0);
                    }
                }
            }
            //If alive and is moving, set animator.
            if(!useMeshNav && !rpgCharacterController.isDead && canMove)
            {
                if(currentVelocity.magnitude > 0 && rpgCharacterInputController.HasMoveInput())
                {
                    isMoving = true;
                    animator.SetBool("Moving", true);
                    animator.SetFloat("Velocity Z", currentVelocity.magnitude);
                }
                else
                {
                    isMoving = false;
                    animator.SetBool("Moving", false);
                }
            }
			//Rotate towards movement direction.
			if(!rpgCharacterController.isTargeting || rpgCharacterController.injured)
            {
                if(rpgCharacterInputController.HasMoveInput() && canMove)
                {
                    RotateTowardsMovementDir();
                }
            }
			//Otherwise rotate towards Target.(strafing)
            else
            {
                RotateTowardsTarget(rpgCharacterController.target.transform.position);
            }

			animator.SetFloat("Velocity X", transform.InverseTransformDirection(currentVelocity).x);
			animator.SetFloat("Velocity Z", transform.InverseTransformDirection(currentVelocity).z);
		}

        private bool AcquiringGround()
        {
            return superCharacterController.currentGround.IsGrounded(false, 0.01f);
        }

        public bool MaintainingGround()
        {
            return superCharacterController.currentGround.IsGrounded(true, 0.5f);
        }

        public void RotateGravity(Vector3 up)
        {
            lookDirection = Quaternion.FromToRotation(transform.up, up) * lookDirection;
        }

        // Calculate the initial velocity of a jump based off gravity and desired maximum height attained
        private float CalculateJumpSpeed(float jumpHeight, float gravity)
        {
            return Mathf.Sqrt(2 * jumpHeight * gravity);
        }

        //Below are the state functions. Each one is called based on the name of the state, so when currentState = Idle, we call Idle_EnterState. If currentState = Jump, we call Jump_SuperUpdate()
        private void Idle_EnterState()
        {
            superCharacterController.EnableSlopeLimit();
            superCharacterController.EnableClamping();
            canJump = true;
            doublejumped = false;
            canDoubleJump = false;
            animator.SetInteger("Jumping", 0);
        }

        //Run every frame we are in the idle state.
        private void Idle_SuperUpdate()
        {
            //If Jump.
            if(rpgCharacterInputController.allowedInput && rpgCharacterInputController.inputJump)
            {
                currentState = RPGCharacterState.Jump;
                rpgCharacterState = RPGCharacterState.Jump;
                return;
            }
            if(!MaintainingGround())
            {
                currentState = RPGCharacterState.Fall;
                rpgCharacterState = RPGCharacterState.Fall;
                return;
            }
            if(rpgCharacterInputController.HasMoveInput() && canMove)
            {
                currentState = RPGCharacterState.Move;
                rpgCharacterState = RPGCharacterState.Move;
                return;
            }
            //Apply friction to slow to a halt.
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, groundFriction * superCharacterController.deltaTime);
        }

        //Run once when character exits the idle state.
        private void Idle_ExitState()
        {
        }

        private void Move_SuperUpdate()
        {
            //If Jump.
            if(rpgCharacterInputController.allowedInput && rpgCharacterInputController.inputJump)
            {
                currentState = RPGCharacterState.Jump;
                rpgCharacterState = RPGCharacterState.Jump;
                return;
            }
			//Falling.
            if(!MaintainingGround())
            {
                currentState = RPGCharacterState.Fall;
                rpgCharacterState = RPGCharacterState.Fall;
                return;
            }
            //Set speed determined by movement type.
            if(rpgCharacterInputController.HasMoveInput() && canMove)
            {
                //Keep strafing animations from playing.
                animator.SetFloat("Velocity X", 0F);
                //Injured limp.
                if(rpgCharacterController.injured)
                {
                    currentVelocity = Vector3.MoveTowards(currentVelocity, rpgCharacterInputController.moveInput * 1.35f, movementAcceleration * superCharacterController.deltaTime);
                    return;
                }
                //Strafing or Walking.
                if(rpgCharacterController.isTargeting)
                {
                    currentVelocity = Vector3.MoveTowards(currentVelocity, rpgCharacterInputController.moveInput * walkSpeed, movementAcceleration * superCharacterController.deltaTime);
                    RotateTowardsTarget(rpgCharacterController.target.transform.position);
                    return;
                }
                //Run.
                currentVelocity = Vector3.MoveTowards(currentVelocity, rpgCharacterInputController.moveInput * runSpeed, movementAcceleration * superCharacterController.deltaTime);
            }
            else
            {
                currentState = RPGCharacterState.Idle;
                rpgCharacterState = RPGCharacterState.Idle;
                return;
            }
        }

        private void Jump_EnterState()
        {
            superCharacterController.DisableClamping();
            superCharacterController.DisableSlopeLimit();
            currentVelocity += superCharacterController.up * CalculateJumpSpeed(jumpHeight, gravity);
            canJump = false;
            animator.SetInteger("Jumping", 1);
            animator.SetTrigger("JumpTrigger");
        }

		private void Jump_SuperUpdate()
		{
			Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(superCharacterController.up, currentVelocity);
			Vector3 verticalMoveDirection = currentVelocity - planarMoveDirection;
			//Falling.
			if(currentVelocity.y < 0)
			{
				currentVelocity = planarMoveDirection;
				currentState = RPGCharacterState.Fall;
				rpgCharacterState = RPGCharacterState.Fall;
				return;
			}
			planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, rpgCharacterInputController.moveInput * inAirSpeed, jumpAcceleration * superCharacterController.deltaTime);
			verticalMoveDirection -= superCharacterController.up * gravity * superCharacterController.deltaTime;
			currentVelocity = planarMoveDirection + verticalMoveDirection;
		}

		private void DoubleJump_EnterState()
        {
            currentVelocity += superCharacterController.up * CalculateJumpSpeed(doubleJumpHeight, gravity);
            canDoubleJump = false;
            doublejumped = true;
            animator.SetInteger("Jumping", 3);
            animator.SetTrigger("JumpTrigger");
        }

        private void DoubleJump_SuperUpdate()
        {
            Jump_SuperUpdate();
        }

        private void Fall_EnterState()
        {
            if(!doublejumped)
            {
                canDoubleJump = true;
            }
            superCharacterController.DisableClamping();
            superCharacterController.DisableSlopeLimit();
            canJump = false;
            animator.SetInteger("Jumping", 2);
            animator.SetTrigger("JumpTrigger");
        }

        private void Fall_SuperUpdate()
        {
            if(AcquiringGround())
            {
                currentVelocity = Math3d.ProjectVectorOnPlane(superCharacterController.up, currentVelocity);
                currentState = RPGCharacterState.Idle;
                rpgCharacterState = RPGCharacterState.Idle;
                return;
            }
            DoubleJump();
            currentVelocity -= superCharacterController.up * gravity * superCharacterController.deltaTime;
        }

		#endregion

		private void DoubleJump()
		{
			if(!doublejumped)
			{
				canDoubleJump = true;
			}
			if(rpgCharacterInputController.inputJump && canDoubleJump && !doublejumped)
			{
				currentState = RPGCharacterState.DoubleJump;
				rpgCharacterState = RPGCharacterState.DoubleJump;
			}
		}

		public void DirectionalRoll()
        {
            //Check which way the dash is pressed relative to the character facing.
            float angle = Vector3.Angle(rpgCharacterInputController.moveInput, -transform.forward);
            float sign = Mathf.Sign(Vector3.Dot(transform.up, Vector3.Cross(rpgCharacterInputController.aimInput, transform.forward)));
            //Angle in [-179,180].
            float signed_angle = angle * sign;
            //Angle in 0-360.
            float angle360 = (signed_angle + 180) % 360;
            //Deternime the animation to play based on the angle.
            if(angle360 > 315 || angle360 < 45)
            {
                StartCoroutine(_Roll(1));
            }
            if(angle360 > 45 && angle360 < 135)
            {
                StartCoroutine(_Roll(2));
            }
            if(angle360 > 135 && angle360 < 225)
            {
                StartCoroutine(_Roll(3));
            }
            if(angle360 > 225 && angle360 < 315)
            {
                StartCoroutine(_Roll(4));
            }
        }

        /// <summary>
        /// Character Roll.
        /// </summary>
        /// <param name="1">Forward.</param>
        /// <param name="2">Right.</param>
        /// <param name="3">Backward.</param>
        /// <param name="4">Left.</param>
        public IEnumerator _Roll(int roll)
        {
            rollNumber = roll;
            currentState = RPGCharacterState.Roll;
            rpgCharacterState = RPGCharacterState.Roll;
            animator.SetInteger("Action", rollNumber);
            animator.SetTrigger("RollTrigger");
            isRolling = true;
			rpgCharacterController.Lock(true, true, true, 0, rollduration);
			yield return new WaitForSeconds(rollduration);
            isRolling = false;
            currentState = RPGCharacterState.Idle;
            rpgCharacterState = RPGCharacterState.Idle;
        }

        public void SwitchCollisionOff()
        {
            canMove = false;
            superCharacterController.enabled = false;
            animator.applyRootMotion = true;
            if(rb != null)
            {
                rb.isKinematic = false;
            }
        }

        public void SwitchCollisionOn()
        {
            canMove = true;
            superCharacterController.enabled = true;
            animator.applyRootMotion = false;
            if(rb != null)
            {
                rb.isKinematic = true;
            }
        }

        private void RotateTowardsMovementDir()
        {
            if(rpgCharacterInputController.moveInput != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rpgCharacterInputController.moveInput), Time.deltaTime * rotationSpeed);
            }
        }

        private void RotateTowardsTarget(Vector3 targetPosition)
        {
			Quaternion targetRotation = Quaternion.LookRotation(targetPosition - new Vector3(transform.position.x, 0, transform.position.z));
            transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, (rotationSpeed * Time.deltaTime) * rotationSpeed);
        }

        public IEnumerator _Knockback(Vector3 knockDirection, int knockBackAmount, int variableAmount)
        {
            isKnockback = true;
            StartCoroutine(_KnockbackForce(knockDirection, knockBackAmount, variableAmount));
            yield return new WaitForSeconds(.1f);
            isKnockback = false;
        }

        private IEnumerator _KnockbackForce(Vector3 knockDirection, int knockBackAmount, int variableAmount)
        {
            while(isKnockback)
            {
                rb.AddForce(knockDirection * ((knockBackAmount + Random.Range(-variableAmount, variableAmount)) * (knockbackMultiplier * 10)), ForceMode.Impulse);
                yield return null;
            }
        }

        //Keep character from moving.
        public void LockMovement()
        {
			canMove = false;
            animator.SetBool("Moving", false);
            animator.applyRootMotion = true;
            currentVelocity = new Vector3(0, 0, 0);
        }

		//Allow character movement.
        public void UnlockMovement()
        {
            canMove = true;
            animator.applyRootMotion = false;
        }
    }
}