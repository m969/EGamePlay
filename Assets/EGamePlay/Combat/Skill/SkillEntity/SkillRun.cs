using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat.Skill
{
    public class SkillRun
    {
        public SkillEntity Skill { get; set; }
        public SkillConfigObject SkillConfigObject => Skill.SkillConfigObject;
        public GameTimer GameTimer { get; set; } = new GameTimer(0.05f);
        public GameObject SkillColliderObj { get; set; }
        public GameObject SkillEffectObj { get; set; }


        public void Start()
        {

        }

        public void Update()
        {
            if (!GameTimer.IsFinished)
            {
                GameTimer.UpdateAsFinish(Time.deltaTime);
            }
        }

        /// <summary>
        /// 开始技能运行表现
        /// </summary>
        public void StartRun()
        {
            SkillColliderObj = GameObject.Instantiate(SkillConfigObject.AreaCollider.gameObject);
            SkillEffectObj = GameObject.Instantiate(SkillConfigObject.SkillEffectObject.SkillParticleEffect);
            SkillColliderObj.transform.position = Skill.SkillListen.SkillGuideTrm.position;
            SkillEffectObj.transform.position = Skill.SkillListen.SkillGuideTrm.position;
            GameTimer.OnFinish(() =>
            {
                Skill.EndRun();
            });
            GameTimer.Reset();
        }

        /// <summary>
        /// 结束技能运行表现
        /// </summary>
        public void EndRun()
        {
            GameObject.Destroy(SkillColliderObj);
            GameObject.Destroy(SkillEffectObj);
        }
    }
}