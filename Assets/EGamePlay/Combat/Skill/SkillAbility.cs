using EGamePlay.Combat.Ability;
using System;

namespace EGamePlay.Combat.Skill
{
    public abstract class SkillAbility : AbilityEntity
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