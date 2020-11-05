using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGamePlay.Combat.Skill
{
    public abstract class SkillEntity : Entity
    {
        public CombatEntity SpellCaster { get; set; }
        public CombatEntity SkillTarget { get; set; }
        public SkillConfigObject SkillConfigObject { get; set; }
        public SkillListen SkillListen { get; set; } = new SkillListen();
        public SkillRun SkillRun { get; set; } = new SkillRun();
        public bool Listening { get; set; }
        public bool Running { get; set; }


        public void Start()
        {
            SkillListen.SkillEntity = this;
            SkillRun.SkillEntity = this;
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

        public void AssignSkillEffect()
        {
            foreach (var item in SkillConfigObject.EffectGroupList)
            {
                if (item.SkillEffectType == SkillEffectType.CauseDamage)
                {
                    var operation = CombatOperationManager.CreateOperation<DamageOperation>(this.SpellCaster);
                    operation.Target = SkillTarget;
                    operation.ApplyDamage();
                }
                else if (item.SkillEffectType == SkillEffectType.CureHero)
                {
                    var operation = CombatOperationManager.CreateOperation<CureOperation>(this.SpellCaster);
                    operation.Target = SkillTarget;
                    operation.ApplyCure();
                }
                else
                {
                    var operation = CombatOperationManager.CreateOperation<AssignEffectOperation>(this.SpellCaster);
                    operation.Target = SkillTarget;
                    operation.ApplyAssignOperation();
                }
            }
        }
    }
}