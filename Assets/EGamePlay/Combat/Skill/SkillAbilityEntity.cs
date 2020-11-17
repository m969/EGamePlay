using EGamePlay.Combat.Ability;

namespace EGamePlay.Combat.Skill
{
    public abstract class SkillAbilityEntity : AbilityEntity
    {
        public SkillConfigObject SkillConfigObject { get; set; }


        public override void Awake(object paramObject)
        {
            base.Awake(paramObject);

            SkillConfigObject = paramObject as SkillConfigObject;
            if (SkillConfigObject.SkillSpellType == SkillSpellType.Passive)
            {
                TryActivateAbility();
            }
        }
    }
}