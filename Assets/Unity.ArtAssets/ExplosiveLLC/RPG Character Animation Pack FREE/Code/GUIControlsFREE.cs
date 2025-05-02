using UnityEngine;

namespace RPGCharacterAnimsFREE
{
    public class GUIControlsFREE:MonoBehaviour
    {
        private RPGCharacterControllerFREE rpgCharacterController;
        private RPGCharacterMovementControllerFREE rpgCharacterMovementController;
        private RPGCharacterWeaponControllerFREE rpgCharacterWeaponController;
        [HideInInspector] public bool blockGui;
        private bool blockToggle;
		private bool navAgentToggle;
        public bool useNavAgent;

        private void Start()
        {
            rpgCharacterController = GetComponent<RPGCharacterControllerFREE>();
            rpgCharacterMovementController = GetComponent<RPGCharacterMovementControllerFREE>();
            rpgCharacterWeaponController = GetComponent<RPGCharacterWeaponControllerFREE>();

			//Check if Animator exists, otherwise pause script.
			if(rpgCharacterController.animator == null)
			{
				Destroy(this);
				return;
			}
		}

        private void OnGUI()
        {
			//If not dead.
			if(!rpgCharacterController.isDead)
			{
				//Actions.
				if(rpgCharacterController.canAction)
				{
					//Character is on the ground.
					if(rpgCharacterMovementController.MaintainingGround())
					{
						if(!navAgentToggle)
						{
							//Blocking.
							blockGui = GUI.Toggle(new Rect(25, 215, 100, 30), blockGui, "Block");
							if(blockGui)
							{
								if(!blockToggle)
								{
									blockToggle = true;
									rpgCharacterController.canBlock = false;
									rpgCharacterController.isBlocking = true;
									rpgCharacterController.animator.SetBool("Blocking", true);
									rpgCharacterMovementController.canMove = false;
									rpgCharacterController.animator.SetTrigger("BlockTrigger");
								}
							}
							if(!blockGui)
							{
								if(blockToggle)
								{
									rpgCharacterController.isBlocking = false;
									rpgCharacterController.animator.SetBool("Blocking", false);
									rpgCharacterMovementController.canMove = true;
									blockToggle = false;
									rpgCharacterController.canBlock = true;
								}
							}
							//Blocking.
							if(blockGui)
							{
								if(GUI.Button(new Rect(30, 240, 100, 30), "Get Hit"))
								{
									rpgCharacterController.GetHit();
								}
								if(GUI.Button(new Rect(30, 270, 100, 30), "Block Break"))
								{
									StartCoroutine(rpgCharacterController._BlockBreak());
								}
							}
							//Not Blocking.
							else if(!rpgCharacterController.isBlocking)
							{
								//Rolling.
								if(GUI.Button(new Rect(25, 15, 100, 30), "Roll Forward"))
								{
									StartCoroutine(rpgCharacterMovementController._Roll(1));
								}
								if(GUI.Button(new Rect(130, 15, 100, 30), "Roll Backward"))
								{
									StartCoroutine(rpgCharacterMovementController._Roll(3));
								}
								if(GUI.Button(new Rect(25, 45, 100, 30), "Roll Left"))
								{
									StartCoroutine(rpgCharacterMovementController._Roll(4));
								}
								if(GUI.Button(new Rect(130, 45, 100, 30), "Roll Right"))
								{
									StartCoroutine(rpgCharacterMovementController._Roll(2));
								}
								//Dodging.
								if(GUI.Button(new Rect(235, 15, 100, 30), "Dodge Left"))
								{
									StartCoroutine(rpgCharacterController._Dodge(1));
								}
								if(GUI.Button(new Rect(235, 45, 100, 30), "Dodge Right"))
								{
									StartCoroutine(rpgCharacterController._Dodge(2));
								}
								//ATTACK LEFT.
								if(rpgCharacterWeaponController.leftWeapon != 0 && rpgCharacterWeaponController.leftWeapon != 7)
								{
									if(GUI.Button(new Rect(25, 85, 100, 30), "Attack L"))
									{
										rpgCharacterController.Attack(1);
									}
								}
								//ATTACK RIGHT.
								if(rpgCharacterController.animator.GetInteger("RightWeapon") != 0)
								{
									if(GUI.Button(new Rect(130, 85, 100, 30), "Attack R"))
									{
										rpgCharacterController.Attack(2);
									}
								}
								//Kicking.
								if(GUI.Button(new Rect(25, 115, 100, 30), "Left Kick"))
								{
									rpgCharacterController.AttackKick(1);
								}
								if(GUI.Button(new Rect(130, 115, 100, 30), "Right Kick"))
								{
									rpgCharacterController.AttackKick(2);
								}
								if(GUI.Button(new Rect(30, 240, 100, 30), "Get Hit"))
								{
									rpgCharacterController.GetHit();
								}
								//Weapon Switching.
								if(!rpgCharacterMovementController.isMoving)
								{
									if(rpgCharacterController.weapon != Weapon.UNARMED)
									{
										if(GUI.Button(new Rect(1115, 310, 100, 30), "Unarmed"))
										{
											StartCoroutine(rpgCharacterWeaponController._SwitchWeapon(0));
											rpgCharacterController.canAction = true;
										}
									}
									if(rpgCharacterController.weapon != Weapon.TWOHANDSWORD)
									{
										if(GUI.Button(new Rect(1115, 340, 100, 30), "2 Hand Sword"))
										{
											StartCoroutine(rpgCharacterWeaponController._SwitchWeapon(1));
										}
									}
								}
							}
						}
					}
					//Jump / Double Jump.
					if((rpgCharacterMovementController.canJump || rpgCharacterMovementController.canDoubleJump) && !blockGui && rpgCharacterController.canAction && !navAgentToggle)
					{
						if(rpgCharacterMovementController.MaintainingGround())
						{
							if(GUI.Button(new Rect(25, 175, 100, 30), "Jump"))
							{
								if(rpgCharacterMovementController.canJump)
								{
									rpgCharacterMovementController.currentState = RPGCharacterState.Jump;
									rpgCharacterMovementController.rpgCharacterState = RPGCharacterState.Jump;
								}
							}
						}
						if(rpgCharacterMovementController.canDoubleJump)
						{
							if(GUI.Button(new Rect(25, 175, 100, 30), "Jump Flip"))
							{
								rpgCharacterMovementController.currentState = RPGCharacterState.DoubleJump;
								rpgCharacterMovementController.rpgCharacterState = RPGCharacterState.DoubleJump;
							}
						}
					}
					//NavMesh
					if(!blockGui && rpgCharacterMovementController.MaintainingGround())
					{
						if(rpgCharacterMovementController.navMeshAgent != null)
						{
							useNavAgent = GUI.Toggle(new Rect(500, 15, 100, 30), useNavAgent, "Use NavAgent");
							if(useNavAgent)
							{
								navAgentToggle = true;
								rpgCharacterMovementController.useMeshNav = true;
								rpgCharacterMovementController.navMeshAgent.enabled = true;
								rpgCharacterController.rpgCharacterInputControllerFREE.allowedInput = false;
								GUI.Label(new Rect(500, 45, 220, 50), "Click to move Character.");
							}
							else
							{
								navAgentToggle = false;
								rpgCharacterMovementController.useMeshNav = false;
								rpgCharacterMovementController.navMeshAgent.enabled = false;
								rpgCharacterController.rpgCharacterInputControllerFREE.allowedInput = true;
							}
						}
						else
						{
							rpgCharacterMovementController.useMeshNav = false;
							rpgCharacterController.rpgCharacterInputControllerFREE.allowedInput = true;
						}
					}
				}
				//Death Pickup Activate.
				if(!blockGui && !rpgCharacterController.isBlocking && rpgCharacterMovementController.MaintainingGround() && rpgCharacterController.canAction && !navAgentToggle)
				{
					if(GUI.Button(new Rect(30, 270, 100, 30), "Death"))
					{
						rpgCharacterController.Death();
					}
				}
			}

			//Revive.
			if(rpgCharacterController.isDead)
			{
				if(GUI.Button(new Rect(30, 270, 100, 30), "Revive"))
				{
					rpgCharacterController.Revive();
				}
			}
        }
    }
}