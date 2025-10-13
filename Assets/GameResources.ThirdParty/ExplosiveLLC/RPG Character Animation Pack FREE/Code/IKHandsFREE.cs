using System.Collections;
using UnityEngine;

namespace RPGCharacterAnimsFREE
{
    public class IKHandsFREE:MonoBehaviour
    {
        private Animator animator;
        private RPGCharacterWeaponControllerFREE rpgCharacterWeaponController;
        public Transform leftHandObj;
        public Transform attachLeft;
		public bool canBeUsed;
        [Range(0, 1)] public float leftHandPositionWeight;
        [Range(0, 1)] public float leftHandRotationWeight;
        private Transform blendToTransform;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            rpgCharacterWeaponController = GetComponentInParent<RPGCharacterWeaponControllerFREE>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if(leftHandObj != null)
            {
				if(attachLeft != null)
				{
					animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandPositionWeight);
					animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandRotationWeight);
					animator.SetIKPosition(AvatarIKGoal.LeftHand, attachLeft.position);
					animator.SetIKRotation(AvatarIKGoal.LeftHand, attachLeft.rotation);
				}
            }
        }

        public IEnumerator _BlendIK(bool blendOn, float delay, float timeToBlend, int weapon)
        {
			if(canBeUsed)
			{
				//If not using 2 handed weapon, quit function. (or 2hand axe/bow)
				if((weapon > 0 && weapon < 3) || weapon == 18 || weapon == 5 || weapon == 20)
				{
					GetCurrentWeaponAttachPoint(weapon);
				}
				else
				{
					yield break;
				}
				yield return new WaitForSeconds(delay);
				float t = 0f;
				float blendTo = 0;
				float blendFrom = 0;
				if(blendOn)
				{
					blendTo = 1;
				}
				else
				{
					blendFrom = 1;
				}
				while(t < 1)
				{
					t += Time.deltaTime / timeToBlend;
					attachLeft = blendToTransform;
					leftHandPositionWeight = Mathf.Lerp(blendFrom, blendTo, t);
					leftHandRotationWeight = Mathf.Lerp(blendFrom, blendTo, t);
					yield return null;
				}
				yield break;
			}
        }

        private void GetCurrentWeaponAttachPoint(int weapon)
        {
            if(weapon == 1)
            {
                blendToTransform = rpgCharacterWeaponController.twoHandSword.transform.GetChild(0).transform;
            }
        }
    }
}