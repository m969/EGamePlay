using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EGamePlay.Combat.Skill
{
    public class SkillListen
    {
        public SkillEntity SkillEntity { get; set; }
        public SkillConfigObject SkillConfigObject => SkillEntity.SkillConfigObject;
        public Transform SkillGuideTrm { get; set; }


        public void Start()
        {

        }

        public void Update()
        {
            if (SkillEntity.SkillConfigObject.TargetSelectType == SkillTargetSelectType.AreaSelect)
            {
                if (CastMapPoint(out var point))
                {
                    SkillGuideTrm.position = point;
                }
                if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
                {
                    //SkillEntity.EndListen();
                    var operation = CombatActionManager.CreateAction<SpellSkillAction>(SkillEntity.SpellCaster);
                    operation.SpellSkill();
                }
            }
        }

        ///// <summary>
        ///// 开始技能监听
        ///// </summary>
        //public void StartListen()
        //{
        //    SkillGuideTrm = GameObject.Instantiate(SkillEntity.SkillConfigObject.AreaGuideObj).transform;
        //    if (CastMapPoint(out var point))
        //    {
        //        SkillGuideTrm.position = point;
        //    }
        //}

        ///// <summary>
        ///// 结束技能监听
        ///// </summary>
        //public void EndListen()
        //{
        //    GameObject.Destroy(SkillGuideTrm.gameObject);
        //}

        public bool CastMapPoint(out Vector3 hitPoint)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 500, 1 << LayerMask.NameToLayer("Map")))
            {
                hitPoint = hit.point;
                return true;
            }
            hitPoint = Vector3.zero;
            return false;
        }
    }
}