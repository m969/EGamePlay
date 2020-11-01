using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat.Skill
{
    public class SkillEntity : Entity
    {
        public SkillConfigObject SkillConfigObject { get; set; }
        public SkillListen SkillListen { get; set; } = new SkillListen();
        public SkillRun SkillRun { get; set; } = new SkillRun();
        public bool Listening { get; set; }
        public bool Running { get; set; }


        public void Start()
        {
            SkillListen.Skill = this;
            SkillRun.Skill = this;
            SkillListen.Start();
            SkillRun.Start();
        }

        public void Update()
        {
            if (Listening)
            {
                SkillListen.Update();
            }
            if (Running)
            {
                SkillRun.Update();
            }
        }

        public void StartListen()
        {
            Debug.Log("StartListen");
            Listening = true;
            SkillListen.StartListen();
        }

        public void EndListen()
        {
            Listening = false;
            SkillListen.EndListen();
        }

        public void StartRun()
        {
            Debug.Log("StartRun");
            Running = true;
            SkillRun.StartRun();
        }

        public void EndRun()
        {
            Running = false;
            SkillRun.EndRun();
        }

        public void ApplyTargetSkillEffect(SkillEffectToggleGroup skillEffect)
        {

        }
    }
}