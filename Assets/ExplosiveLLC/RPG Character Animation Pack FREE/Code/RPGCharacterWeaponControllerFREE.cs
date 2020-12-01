using System.Collections;
using UnityEngine;

namespace RPGCharacterAnimsFREE
{
    public enum Weapon
    {
        UNARMED = 0,
        TWOHANDSWORD = 1,
    }

    public class RPGCharacterWeaponControllerFREE:MonoBehaviour
    {
        private RPGCharacterControllerFREE rpgCharacterController;
        private Animator animator;

        //Weapon Parameters.
        [HideInInspector] public int rightWeapon = 0;
        [HideInInspector] public int leftWeapon = 0;
        [HideInInspector] public bool isSwitchingFinished = true;
        [HideInInspector] public bool isWeaponSwitching = false;

        //Weapon Models.
        public GameObject twoHandSword;

        private void Start()
        {
            rpgCharacterController = GetComponent<RPGCharacterControllerFREE>();
            //Find the Animator component.
            animator = GetComponentInChildren<Animator>();
            StartCoroutine(_HideAllWeapons(false, false));
        }

        //0 = No side
        //1 = Left
        //2 = Right
        //3 = Dual
        //weaponNumber 0 = Unarmed
        //weaponNumber 1 = 2H Sword
        public IEnumerator _SwitchWeapon(int weaponNumber)
        {
            //If is Unarmed/Relax.
            if(IsNoWeapon(animator.GetInteger("Weapon")))
            {
                //Switch to Relax.
                if(weaponNumber == -1)
                {
                    StartCoroutine(_SheathWeapon(0, -1));
                }
                //Switch to Unarmed.
                else
                {
                    StartCoroutine(_UnSheathWeapon(weaponNumber));
                }
            }
            //Character has 2handed weapon.
            else if(Is2HandedWeapon(animator.GetInteger("Weapon")))
            {
                StartCoroutine(_SheathWeapon(leftWeapon, weaponNumber));
                yield return new WaitForSeconds(1.2f);
                //Switching to weapon.
                if(weaponNumber > 0)
                {
                    StartCoroutine(_UnSheathWeapon(weaponNumber));
                }
            }
            yield return null;
        }

        public IEnumerator _UnSheathWeapon(int weaponNumber)
        {
            isWeaponSwitching = true;
            //Switching to Unarmed from Relax.
            if(weaponNumber == 0)
            {
                DoWeaponSwitch(-1, -1, -1, 0, false);
                yield return new WaitForSeconds(0.75f);
                SetAnimator(0, -2, 0, 0, 0);
            }
            //Switching to 2handed weapon.
            else if(Is2HandedWeapon(weaponNumber))
            {
                //Switching from 2handed weapon.
                if(Is2HandedWeapon(animator.GetInteger("Weapon")))
                {
                    DoWeaponSwitch(0, weaponNumber, weaponNumber, -1, false);
                    yield return new WaitForSeconds(0.75f);
                    SetAnimator(weaponNumber, -2, animator.GetInteger("Weapon"), -1, -1);
                }
                else
                {
                    DoWeaponSwitch(animator.GetInteger("Weapon"), weaponNumber, weaponNumber, -1, false);
                    yield return new WaitForSeconds(0.75f);
                    SetAnimator(weaponNumber, -2, weaponNumber, -1, -1);
                }
            }
            //Switching to 1handed weapons.
            else
            {
                //If switching from Unarmed or Relax.
                if(IsNoWeapon(animator.GetInteger("Weapon")))
                {
                    animator.SetInteger("WeaponSwitch", 7);
                }
                //Left hand weapons.
                if(weaponNumber == 7 || weaponNumber == 8 || weaponNumber == 10 || weaponNumber == 12 || weaponNumber == 14 || weaponNumber == 16)
                {
                    //If not switching Shield.
                    if(weaponNumber == 7)
                    {
                        animator.SetBool("Shield", true);
                    }
                    DoWeaponSwitch(7, weaponNumber, animator.GetInteger("Weapon"), 1, false);
                    yield return new WaitForSeconds(0.5f);
                    SetAnimator(7, 7, weaponNumber, -1, 1);
                }
                //Right hand weapons.
                else if(weaponNumber == 9 || weaponNumber == 11 || weaponNumber == 13 || weaponNumber == 15 || weaponNumber == 17 || weaponNumber == 19)
                {
                    animator.SetBool("Shield", false);
                    DoWeaponSwitch(7, weaponNumber, animator.GetInteger("Weapon"), 2, false);
                    yield return new WaitForSeconds(0.5f);
                    if(leftWeapon == 7)
                    {
                        animator.SetBool("Shield", true);
                    }
                    SetAnimator(7, 7, -1, weaponNumber, 2);
                }
            }
            yield return null;
        }

        public IEnumerator _SheathWeapon(int weaponNumber, int weaponTo)
        {
            //Debug.Log("Sheath Weapon: " + weaponNumber + " - Weapon To: " + weaponTo);
            //Reset for animation events.
            isWeaponSwitching = true;
            //Set LeftRight hand for 1handed switching.
            if(IsLeftWeapon(weaponNumber))
            {
                animator.SetInteger("LeftRight", 1);
            }
            else if(IsRightWeapon(weaponNumber))
            {
                animator.SetInteger("LeftRight", 2);
            }
            //Switching to Unarmed or Relaxed.
            if(weaponTo < 1)
            {
                //Have at least 1 weapon.
                if(rightWeapon != 0 || leftWeapon != 0)
                {
                    //Sheath 1handed weapon.
                    if(Is1HandedWeapon(weaponNumber))
                    {
                        //If sheathing both weapons, go to Armed first.
                        if(rightWeapon != 0 && leftWeapon != 0)
                        {
                            DoWeaponSwitch(7, weaponNumber, 7, -1, true);
                        }
                        else
                        {
                            DoWeaponSwitch(weaponTo, weaponNumber, 7, -1, true);
                        }
                        yield return new WaitForSeconds(0.5f);
                        if(IsLeftWeapon(weaponNumber))
                        {
                            animator.SetInteger("LeftWeapon", 0);
                            SetAnimator(weaponTo, -2, 0, -1, -1);
                        }
                        else if(IsRightWeapon(weaponNumber))
                        {
                            animator.SetInteger("RightWeapon", 0);
                            SetAnimator(weaponTo, -2, -1, 0, -1);
                        }
                        animator.SetBool("Shield", false);
                    }
                    //Sheath 2handed weapon.
                    else if(Is2HandedWeapon(weaponNumber))
                    {
                        DoWeaponSwitch(weaponTo, weaponNumber, animator.GetInteger("Weapon"), -1, true);
                        yield return new WaitForSeconds(0.5f);
                        SetAnimator(weaponTo, -2, 0, 0, -1);
                    }
                }
                //Unarmed, switching to Relax.
                else if(rightWeapon == 0 && leftWeapon == 0)
                {
                    DoWeaponSwitch(weaponTo, weaponNumber, animator.GetInteger("Weapon"), 0, true);
                    yield return new WaitForSeconds(0.5f);
                    SetAnimator(weaponTo, -2, 0, 0, -1);
                }
            }
            //Switching to 2handed weapon.
            else if(Is2HandedWeapon(weaponTo))
            {
                //Switching from 1handed weapons.
                if(animator.GetInteger("Weapon") == 7)
                {
                    //Dual weilding, switch to Armed if first switch.
                    if(leftWeapon != 0 && rightWeapon != 0)
                    {
                        DoWeaponSwitch(7, weaponNumber, 7, -1, true);
                        if(IsLeftWeapon(weaponNumber))
                        {
                            SetAnimator(7, -2, 0, -1, -1);
                        }
                        else if(IsRightWeapon(weaponNumber))
                        {
                            SetAnimator(7, -2, -1, 0, -1);
                        }
                    }
                    else
                    {
                        DoWeaponSwitch(0, weaponNumber, 7, -1, true);
                        yield return new WaitForSeconds(0.5f);
                        SetAnimator(0, -2, 0, 0, -1);
                    }
                }
                //Switching from 2handed weapons.
                else
                {
                    DoWeaponSwitch(0, weaponNumber, animator.GetInteger("Weapon"), -1, true);
                    yield return new WaitForSeconds(0.5f);
                    SetAnimator(weaponNumber, -2, weaponNumber, 0, -1);
                }
            }
            //Switching to 1handed weapons.
            else
            {
                //Switching from 2handed weapons, go to Unarmed before next switch.
                if(Is2HandedWeapon(animator.GetInteger("Weapon")))
                {
                    DoWeaponSwitch(0, weaponNumber, animator.GetInteger("Weapon"), 0, true);
                    yield return new WaitForSeconds(0.5f);
                    SetAnimator(0, -2, 0, 0, 0);
                }
                //Switching from 1handed weapon(s), go to Armed before next switch.
                else if(Is1HandedWeapon(animator.GetInteger("Weapon")))
                {
                    if(IsRightWeapon(weaponNumber))
                    {
                        animator.SetBool("Shield", false);
                    }
                    DoWeaponSwitch(7, weaponNumber, 7, -1, true);
                    yield return new WaitForSeconds(0.1f);
                    if(weaponNumber == 7)
                    {
                        animator.SetBool("Shield", false);
                    }
                    if(IsLeftWeapon(weaponNumber))
                    {
                        SetAnimator(7, 7, 0, -1, 0);
                    }
                    else
                    {
                        SetAnimator(7, 7, -1, 0, 0);
                    }
                }
            }
            rpgCharacterController.SetWeaponState(weaponTo);
            yield return null;
        }

        private void DoWeaponSwitch(int weaponSwitch, int weaponVisibility, int weaponNumber, int leftRight, bool sheath)
        {
            //Go to Null state and wait for animator.
            animator.SetInteger("Weapon", -2);
            while(animator.isActiveAndEnabled && animator.GetInteger("Weapon") != -2)
            {
            }
            //Lock character for switch unless has moving sheath/unsheath anims.
            if(weaponSwitch < 1)
            {
                if(Is2HandedWeapon(weaponNumber))
                {
                    rpgCharacterController.Lock(true, true, true, 0f, 1f);
                }
            }
            else if(Is1HandedWeapon(weaponSwitch))
            {
                rpgCharacterController.Lock(true, true, true, 0f, 1f);
            }
            //Set weaponSwitch if applicable.
            if(weaponSwitch != -2)
            {
                animator.SetInteger("WeaponSwitch", weaponSwitch);
            }
            animator.SetInteger("Weapon", weaponNumber);
            //Set leftRight if applicable.
            if(leftRight != -1)
            {
                animator.SetInteger("LeftRight", leftRight);
            }
            //Set animator trigger.
            if(sheath)
            {
                animator.SetTrigger("WeaponSheathTrigger");
                StartCoroutine(_WeaponVisibility(weaponVisibility, false, false));
                //If using IKHands, trigger IK blend.
                if(rpgCharacterController.ikHandsFREE != null)
                {
                    StartCoroutine(rpgCharacterController.ikHandsFREE._BlendIK(false, 0f, 0.2f, weaponVisibility));
                }
            }
            else
            {
                animator.SetTrigger("WeaponUnsheathTrigger");
                StartCoroutine(_WeaponVisibility(weaponVisibility, true, false));
                //If using IKHands, trigger IK blend.
                if(rpgCharacterController.ikHandsFREE != null)
                {
                    StartCoroutine(rpgCharacterController.ikHandsFREE._BlendIK(true, 0.5f, 1, weaponVisibility));
                }
            }
        }

        /// <summary>
        /// Controller weapon switching.
        /// </summary>
        public void SwitchWeaponTwoHand(int upDown)
        {
            isSwitchingFinished = false;
            int weaponSwitch = (int)rpgCharacterController.weapon;
            if(upDown == 0)
            {
                weaponSwitch--;
                if(weaponSwitch < 1 || weaponSwitch == 18 || weaponSwitch == 20)
                {
                    StartCoroutine(_SwitchWeapon(6));
                }
                else
                {
                    StartCoroutine(_SwitchWeapon(weaponSwitch));
                }
            }
            if(upDown == 1)
            {
                weaponSwitch++;
                if(weaponSwitch > 6 && weaponSwitch < 18)
                {
                    StartCoroutine(_SwitchWeapon(1));
                }
                else
                {
                    StartCoroutine(_SwitchWeapon(weaponSwitch));
                }
            }
        }

        /// <summary>
        /// Controller weapon switching.
        /// </summary>
        public void SwitchWeaponLeftRight(int leftRight)
        {
            int weaponSwitch = 0;
            isSwitchingFinished = false;
            if(leftRight == 0)
            {
                weaponSwitch = leftWeapon;
                if(weaponSwitch < 16 && weaponSwitch != 0 && leftWeapon != 7)
                {
                    weaponSwitch += 2;
                }
                else
                {
                    weaponSwitch = 8;
                }
            }
            if(leftRight == 1)
            {
                weaponSwitch = rightWeapon;
                if(weaponSwitch < 17 && weaponSwitch != 0)
                {
                    weaponSwitch += 2;
                }
                else
                {
                    weaponSwitch = 9;
                }
            }
            StartCoroutine(_SwitchWeapon(weaponSwitch));
        }

        public void WeaponSwitch()
        {
            if(isWeaponSwitching)
            {
                isWeaponSwitching = false;
            }
        }

        public void SetSheathLocation(int location)
        {
            animator.SetInteger("SheathLocation", location);
        }

        public IEnumerator _HideAllWeapons(bool timed, bool resetToUnarmed)
        {
            if(timed)
            {
                while(!isWeaponSwitching)
                {
                    yield return null;
                }
            }
            //Reset to Unarmed.
            if(resetToUnarmed)
            {
                animator.SetInteger("Weapon", 0);
                rpgCharacterController.weapon = Weapon.UNARMED;
                StartCoroutine(rpgCharacterController.rpgCharacterWeaponControllerFREE._WeaponVisibility(rpgCharacterController.rpgCharacterWeaponControllerFREE.leftWeapon, false, true));
                animator.SetInteger("RightWeapon", 0);
                animator.SetInteger("LeftWeapon", 0);
                animator.SetInteger("LeftRight", 0);
            }
            if(twoHandSword != null)
            {
                twoHandSword.SetActive(false);
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
                leftWeapon = Lweapon;
                animator.SetInteger("LeftWeapon", Lweapon);
            }
            //Set right weapon if applicable.
            if(Rweapon != -1)
            {
                rightWeapon = Rweapon;
                animator.SetInteger("RightWeapon", Rweapon);
            }
            //Set weapon side if applicable.
            if(weaponSide != -1)
            {
                animator.SetInteger("LeftRight", weaponSide);
            }
            rpgCharacterController.SetWeaponState(weapon);
        }

        public IEnumerator _WeaponVisibility(int weaponNumber, bool visible, bool dual)
        {
            //Debug.Log("WeaponVisiblity: " + weaponNumber + "  Visible: " + visible + "  dual: " + dual);
            while(isWeaponSwitching)
            {
                yield return null;
            }
            if(weaponNumber == 1)
            {
                twoHandSword.SetActive(visible);
            }
            yield return null;
        }

        public bool IsNoWeapon(int weaponNumber)
        {
            if(weaponNumber < 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsLeftWeapon(int weaponNumber)
        {
            if((weaponNumber == 7 || weaponNumber == 8 || weaponNumber == 10 || weaponNumber == 12 || weaponNumber == 14 || weaponNumber == 16))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HasLeftWeapon()
        {
            if(IsLeftWeapon(animator.GetInteger("LeftWeapon")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsRightWeapon(int weaponNumber)
        {
            if((weaponNumber == 9 || weaponNumber == 11 || weaponNumber == 13 || weaponNumber == 15 || weaponNumber == 17 || weaponNumber == 19))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HasRightWeapon()
        {
            if(IsRightWeapon(animator.GetInteger("RightWeapon")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HasDualWeapons()
        {
            if(HasRightWeapon() && HasLeftWeapon())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HasTwoHandedWeapon()
        {
            if((rpgCharacterController.weapon > 0 && (int)rpgCharacterController.weapon < 7) || (int)rpgCharacterController.weapon == 18 || (int)rpgCharacterController.weapon == 20)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Is2HandedWeapon(int weaponNumber)
        {
            if((weaponNumber > 0 && weaponNumber < 7) || weaponNumber == 18 || weaponNumber == 20)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Is1HandedWeapon(int weaponNumber)
        {
            if((weaponNumber > 6 && weaponNumber < 18) || weaponNumber == 19)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
	}
}