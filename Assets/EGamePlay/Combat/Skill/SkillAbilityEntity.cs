using EGamePlay.Combat.Ability;
using System;

namespace EGamePlay.Combat.Skill
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
    public sealed class ExecutionSkillTypeAttribute : System.Attribute
    {
        readonly Type skill;

        public ExecutionSkillTypeAttribute(Type skill)
        {
            this.skill = skill;
        }

        public Type Skill
        {
            get { return skill; }
        }
    }

    public abstract class SkillAbilityEntity : AbilityEntity
    {
        public SkillConfigObject SkillConfigObject { get; set; }


        public override void Awake(object initData)
        {
            base.Awake(initData);

            SkillConfigObject = initData as SkillConfigObject;
            if (SkillConfigObject.SkillSpellType == SkillSpellType.Passive)
            {
                TryActivateAbility();
            }
        }
    }
}