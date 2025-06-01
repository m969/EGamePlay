using System.Collections;
using UnityEngine;

namespace RPGCharacterAnimsFREE
{
    public class RPGCharacterControllerFREE:MonoBehaviour
    {
        //Components.
        [HideInInspector] public RPGCharacterMovementControllerFREE rpgCharacterMovementControllerFREE;
        [HideInInspector] public RPGCharacterWeaponControllerFREE rpgCharacterWeaponControllerFREE;
        [HideInInspector] public RPGCharacterInputControllerFREE rpgCharacterInputControllerFREE;
        [HideInInspector] public Animator animator;
        [HideInInspector] public IKHandsFREE ikHandsFREE;
        public Weapon weapon = Weapon.UNARMED;
        public GameObject target;

        //Strafing/action.
        [HideInInspector] public bool isDead = false;
		[HideInInspector] public bool canBlock = true;
		[HideInInspector] public bool isBlocking = false;
        [HideInInspector] public bool canAction = true;
        [HideInInspector] public bool isTargeting = false;
		[HideInInspector] public bool isFacing = false;
		[HideInInspector] public bool injured;
        private float idleTimer;
        private float idleTrigger = 0f;

        public float animationSpeed = 1;

        #region Initialization

        private void Awake()
        {
			//Initialize other RPG scripts.
			rpgCharacterMovementControllerFREE = GetComponent<RPGCharacterMovementControllerFREE>();
            rpgCharacterWeaponControllerFREE = GetComponent<RPGCharacterWeaponControllerFREE>();
            rpgCharacterInputControllerFREE = gameObject.AddComponent<RPGCharacterInputControllerFREE>();

			//Setup Animator, add AnimationEvents script.
			animator = GetComponentInChildren<Animator>();
			if(animator == null)
			{
				Debug.LogError("ERROR: THERE IS NO ANIMATOR COMPONENT ON CHILD OF CHARACTER.");
				Time.timeScale = 0f;
				Destroy(this);
				return;
			}
			else if(!animator.isHuman)
			{
				Debug.LogError("ERROR: CHARACTER AVATAR RIG IS NOT HUMANOID.");
				Time.timeScale = 0f;
				Destroy(this);
				return;
			}
			else
			{
				animator.gameObject.AddComponent<RPGCharacterAnimatorEventsFREE>();
				animator.GetComponent<RPGCharacterAnimatorEventsFREE>().rpgCharacterController = this;
				animator.gameObject.AddComponent<AnimatorParentMoveFREE>();
				animator.GetComponent<AnimatorParentMoveFREE>().anim = animator;
				animator.GetComponent<AnimatorParentMoveFREE>().rpgCharacterMovementController = rpgCharacterMovementControllerFREE;
				//animator.updateMode = AnimatorUpdateMode.Fixed;
				animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
			}

            ikHandsFREE = GetComponentInChildren<IKHandsFREE>();

            //Set for starting Unarmed state.
            weapon = Weapon.UNARMED;
            animator.SetInteger("Weapon", 0);
            animator.SetInteger("WeaponSwitch", -1);
            StartCoroutine(_ResetIdleTimer());
		}

        #endregion

        #region Updates

        private void Update()
        {
            if(rpgCharacterMovementControllerFREE.MaintainingGround())
            {
                //Revive.
                if(rpgCharacterInputControllerFREE.inputDeath)
                {
                    if(isDead)
                    {
                        Revive();
                    }
                }
                if(canAction)
                {
                    Blocking();
                    if(!isBlocking)
                    {
                        Targeting();
                        RandomIdle();
                        Rolling();
                        //Hit.
                        if(rpgCharacterInputControllerFREE.inputLightHit)
                        {
                            GetHit();
                        }
                        //Death.
                        if(rpgCharacterInputControllerFREE.inputDeath)
                        {
                            if(!isDead)
                            {
                                Death();
                            }
                            else
                            {
                                Revive();
                            }
                        }
                        //Attacks.
                        if(rpgCharacterInputControllerFREE.inputAttackL)
                        {
                            Attack(1);
                        }
                        if(rpgCharacterInputControllerFREE.inputAttackR)
                        {
                            Attack(2);
                        }
						if(rpgCharacterInputControllerFREE.inputKickL)
						{
							AttackKick(1);
						}
						if(rpgCharacterInputControllerFREE.inputKickR)
						{
							AttackKick(2);
						}
						if(rpgCharacterInputControllerFREE.inputLightHit)
                        {
                            GetHit();
                        }
						//Navmesh.
						if(UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
						{
                            if(rpgCharacterMovementControllerFREE.useMeshNav)
                            {
                                RaycastHit hit;
								if(Physics.Raycast(Camera.main.ScreenPointToRay(rpgCharacterInputControllerFREE.inputMouseFacing), out hit, 100))
								{
                                    rpgCharacterMovementControllerFREE.navMeshAgent.destination = hit.point;
                                }
                            }
                        }
                    }
                }
            }
			//Injury toggle.
			if(UnityEngine.InputSystem.Keyboard.current.iKey.wasPressedThisFrame)
			{
                if(injured == false)
                {
                    injured = true;
                    animator.SetBool("Injured", true);
                }
                else
                {
                    injured = false;
                    animator.SetBool("Injured", false);
                }
            }
			//Slow time toggle.
			if(UnityEngine.InputSystem.Keyboard.current.tKey.wasPressedThisFrame)
			{
                if(Time.timeScale != 1)
                {
                    Time.timeScale = 1;
                }
                else
                {
                    Time.timeScale = 0.25f;
                }
            }
			//Pause toggle.
			if(UnityEngine.InputSystem.Keyboard.current.pKey.wasPressedThisFrame)
			{
                if(Time.timeScale != 1)
                {
                    Time.timeScale = 1;
                }
                else
                {
                    Time.timeScale = 0f;
                }
            }

			//Update animation play speed if adjusted.
            animator.SetFloat("AnimationSpeed", animationSpeed);
        }

        #endregion

        #region Combat

        /// <summary>
        /// Dodge the specified direction.
        /// </summary>
        /// <param name="1">Left</param>
        /// <param name="2">Right</param>
        public IEnumerator _Dodge(int direction)
        {
            animator.SetInteger("Action", direction);
            animator.SetTrigger("DodgeTrigger");
            Lock(true, true, true, 0, 0.55f);
            yield return null;
        }

        //0 = No side
        //1 = Left
        //2 = Right
        //3 = Dual
        //weaponNumber 0 = Unarmed
        //weaponNumber 1 = 2H Sword
        public void Attack(int attackSide)
        {
            int attackNumber = 0;
            //Unarmed.
            if(weapon == Weapon.UNARMED)
            {
                int maxAttacks = 3;
                //Left attacks.
                if(attackSide == 1)
                {
					if(rpgCharacterWeaponControllerFREE != null)
					{
						attackNumber = Random.Range(1, maxAttacks + 1);
					}
                }
                //Right attacks.
                else if(attackSide == 2)
                {
					if(rpgCharacterWeaponControllerFREE != null)
					{
						attackNumber = Random.Range(4, maxAttacks + 4);
					}
                }
                //Set the Locks.
                if(attackSide != 3)
                {
					if(rpgCharacterWeaponControllerFREE != null)
					{
						if(rpgCharacterWeaponControllerFREE.leftWeapon == 8 || rpgCharacterWeaponControllerFREE.leftWeapon == 10 || rpgCharacterWeaponControllerFREE.leftWeapon == 16
							|| rpgCharacterWeaponControllerFREE.rightWeapon == 9 || rpgCharacterWeaponControllerFREE.rightWeapon == 11 || rpgCharacterWeaponControllerFREE.rightWeapon == 17)
						{
							Lock(true, true, true, 0, 0.75f);
						}
						else
						{
							//Dagger and Item has longer attack time.
							Lock(true, true, true, 0, 1f);
						}
					}
                }
            }
            else if(weapon == Weapon.TWOHANDSWORD)
            {
                int maxAttacks = 11;
                attackNumber = Random.Range(1, maxAttacks);
                Lock(true, true, true, 0, 1.1f);
            }
            else
            {
                int maxAttacks = 6;
                attackNumber = Random.Range(1, maxAttacks);
                if(weapon == Weapon.TWOHANDSWORD)
                {
                    Lock(true, true, true, 0, 0.85f);
                }
                else
                {
                    Lock(true, true, true, 0, 0.75f);
                }
            }
            //Trigger the animation.
            animator.SetInteger("Action", attackNumber);
            if(attackSide == 3)
            {
                animator.SetTrigger("AttackDualTrigger");
            }
            else
            {
                animator.SetTrigger("AttackTrigger");
            }
        }

        public void AttackKick(int kickSide)
        {
            animator.SetInteger("Action", kickSide);
            animator.SetTrigger("AttackKickTrigger");
            Lock(true, true, true, 0, 0.9f);
        }

        public void Blocking()
        {
			if(canBlock)
			{
				if(!isBlocking)
				{
					if(rpgCharacterInputControllerFREE.HasBlockInput() )
					{
						isBlocking = true;
						animator.SetBool("Blocking", true);
						rpgCharacterMovementControllerFREE.canMove = false;
						animator.SetTrigger("BlockTrigger");
					}
				}
				else
				{
					if(!rpgCharacterInputControllerFREE.HasBlockInput())
					{
						isBlocking = false;
						animator.SetBool("Blocking", false);
						rpgCharacterMovementControllerFREE.canMove = true;
					}
				}
			}
		}

        private void Targeting()
        {
            if(rpgCharacterInputControllerFREE.inputTarget)
            {
                animator.SetBool("Targeting", true);
                isTargeting = true;
            }
            else
            {
                isTargeting = false;
                animator.SetBool("Targeting", false);
            }
        }

        private void Rolling()
        {
            if(!rpgCharacterMovementControllerFREE.isRolling)
            {
                if(rpgCharacterInputControllerFREE.inputRoll)
                {
                    rpgCharacterMovementControllerFREE.DirectionalRoll();
                }
            }
        }

        public void GetHit()
        {
            int hits = 5;
            if(isBlocking)
            {
                hits = 2;
            }
            int hitNumber = Random.Range(1, hits + 1);
            animator.SetInteger("Action", hitNumber);
            animator.SetTrigger("GetHitTrigger");
            Lock(true, true, true, 0.1f, 0.4f);
            if(isBlocking)
            {
                StartCoroutine(rpgCharacterMovementControllerFREE._Knockback(-transform.forward, 3, 3));
                return;
            }
            //Apply directional knockback force.
            if(hitNumber <= 1)
            {
                StartCoroutine(rpgCharacterMovementControllerFREE._Knockback(-transform.forward, 8, 4));
            }
            else if(hitNumber == 2)
            {
                StartCoroutine(rpgCharacterMovementControllerFREE._Knockback(transform.forward, 8, 4));
            }
            else if(hitNumber == 3)
            {
                StartCoroutine(rpgCharacterMovementControllerFREE._Knockback(transform.right, 8, 4));
            }
            else if(hitNumber == 4)
            {
                StartCoroutine(rpgCharacterMovementControllerFREE._Knockback(-transform.right, 8, 4));
            }
        }

        public void Death()
        {
            animator.SetTrigger("DeathTrigger");
            Lock(true, true, false, 0.1f, 0f);
            isDead = true;
        }

        public void Revive()
        {
            animator.SetTrigger("ReviveTrigger");
            Lock(true, true, true, 0f, 1f);
            isDead = false;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Keep character from doing actions.
        /// </summary>
        private void LockAction()
        {
            canAction = false;
        }

        /// <summary>
        /// Let character move and act again.
        /// </summary>
        private void UnLock(bool movement, bool actions)
        {
            StartCoroutine(_ResetIdleTimer());
            if(movement)
            {
                rpgCharacterMovementControllerFREE.UnlockMovement();
            }
            if(actions)
            {
                canAction = true;
            }
        }

		#endregion

		#region Misc

		/// <summary>
		/// Plays random idle animation. Currently only Alert1 animation.
		/// </summary>
		private void RandomIdle()
        {
			if(rpgCharacterWeaponControllerFREE != null)
			{
				if(!rpgCharacterMovementControllerFREE.isMoving && !rpgCharacterWeaponControllerFREE.isWeaponSwitching && rpgCharacterMovementControllerFREE.canMove)
				{
					idleTimer += 0.01f;
					if(idleTimer > idleTrigger)
					{
						//Turn off IK Hands.
						if(ikHandsFREE != null)
						{
							ikHandsFREE.canBeUsed = false;
						}
						animator.SetInteger("Action", 1);
						animator.SetTrigger("IdleTrigger");
						StartCoroutine(_ResetIdleTimer());
						Lock(true, true, true, 0, 1.25f);
						if(ikHandsFREE != null)
						{
							ikHandsFREE.canBeUsed = true;
						}
					}
				}
			}
        }

        private IEnumerator _ResetIdleTimer()
        {
            idleTrigger = Random.Range(5f, 15f);
            idleTimer = 0;
            yield return new WaitForSeconds(1f);
            animator.ResetTrigger("IdleTrigger");
        }

        private IEnumerator _GetCurrentAnimationLength()
        {
            yield return new WaitForEndOfFrame();
            float f = animator.GetCurrentAnimatorClipInfo(0).Length;
            Debug.Log(f);
        }

        /// <summary>
        /// Lock character movement and/or action, on a delay for a set time.
        /// </summary>
        /// <param name="lockMovement">If set to <c>true</c> lock movement.</param>
        /// <param name="lockAction">If set to <c>true</c> lock action.</param>
        /// <param name="timed">If set to <c>true</c> timed.</param>
        /// <param name="delayTime">Delay time.</param>
        /// <param name="lockTime">Lock time.</param>
        public void Lock(bool lockMovement, bool lockAction, bool timed, float delayTime, float lockTime)
        {
            StopCoroutine("_Lock");
            StartCoroutine(_Lock(lockMovement, lockAction, timed, delayTime, lockTime));
        }

        //Timed -1 = infinite, 0 = no, 1 = yes.
        public IEnumerator _Lock(bool lockMovement, bool lockAction, bool timed, float delayTime, float lockTime)
        {
            if(delayTime > 0)
            {
                yield return new WaitForSeconds(delayTime);
            }
            if(lockMovement)
            {
                rpgCharacterMovementControllerFREE.LockMovement();
            }
            if(lockAction)
            {
                LockAction();
            }
            if(timed)
            {
                if(lockTime > 0)
                {
                    yield return new WaitForSeconds(lockTime);
                }
                UnLock(lockMovement, lockAction);
            }
        }

        /// <summary>
        /// Sets the animator state.
        /// </summary>
        /// <param name="weapon">Weapon.</param>
        /// <param name="weaponSwitch">Weapon switch.</param>
        /// <param name="Lweapon">Lweapon.</param>
        /// <param name="Rweapon">Rweapon.</param>
        /// <param name="weaponSide">Weapon side.</param>
        private void SetAnimator(int weapon, int weaponSwitch, int Lweapon, int Rweapon, int weaponSide)
        {
            Debug.Log("SETANIMATOR: Weapon:" + weapon + " Weaponswitch:" + weaponSwitch + " Lweapon:" + Lweapon + " Rweapon:" + Rweapon + " Weaponside:" + weaponSide);
            //Set Weapon if applicable.
            if(weapon != -2)
            {
                animator.SetInteger("Weapon", weapon);
            }
            //Set WeaponSwitch if applicable.
            if(weaponSwitch != -2)
            {
                animator.SetInteger("WeaponSwitch", weaponSwitch);
            }
            //Set left weapon if applicable.
            if(Lweapon != -1)
            {
				if(rpgCharacterWeaponControllerFREE != null)
				{
					rpgCharacterWeaponControllerFREE.leftWeapon = Lweapon;
				}
                animator.SetInteger("LeftWeapon", Lweapon);
            }
            //Set right weapon if applicable.
            if(Rweapon != -1)
            {
				if(rpgCharacterWeaponControllerFREE != null)
				{
					rpgCharacterWeaponControllerFREE.rightWeapon = Rweapon;
				}
                animator.SetInteger("RightWeapon", Rweapon);
            }
            //Set weapon side if applicable.
            if(weaponSide != -1)
            {
                animator.SetInteger("LeftRight", weaponSide);
            }
            SetWeaponState(weapon);
        }

        public void SetWeaponState(int weaponNumber)
        {
			if(rpgCharacterWeaponControllerFREE != null)
			{
				if(weaponNumber == 0)
				{
					weapon = Weapon.UNARMED;
				}
				else if(weaponNumber == 1)
				{
					weapon = Weapon.TWOHANDSWORD;
				}
			}
        }

        public IEnumerator _BlockBreak()
        {
            animator.SetTrigger("BlockBreakTrigger");
			Lock(true, true, true, 0, 1f);
			yield return null;
		}

        public void AnimatorDebug()
        {
            Debug.Log("ANIMATOR SETTINGS---------------------------");
            Debug.Log("Moving: " + animator.GetBool("Moving"));
            Debug.Log("Strafing: " + animator.GetBool("Strafing"));
            Debug.Log("Blocking: " + animator.GetBool("Blocking"));
            Debug.Log("Injured: " + animator.GetBool("Injured"));
            Debug.Log("Weapon: " + animator.GetInteger("Weapon"));
            Debug.Log("WeaponSwitch: " + animator.GetInteger("WeaponSwitch"));
            Debug.Log("LeftRight: " + animator.GetInteger("LeftRight"));
            Debug.Log("LeftWeapon: " + animator.GetInteger("LeftWeapon"));
            Debug.Log("RightWeapon: " + animator.GetInteger("RightWeapon"));
            Debug.Log("AttackSide: " + animator.GetInteger("AttackSide"));
            Debug.Log("Jumping: " + animator.GetInteger("Jumping"));
            Debug.Log("Action: " + animator.GetInteger("Action"));
            Debug.Log("Velocity X: " + animator.GetFloat("Velocity X"));
            Debug.Log("Velocity Z: " + animator.GetFloat("Velocity Z"));
        }

        #endregion

    }
}