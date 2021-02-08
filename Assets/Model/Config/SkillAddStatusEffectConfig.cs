using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
	[Config]
	public partial class SkillAddStatusEffectConfigCategory : ACategory<SkillAddStatusEffectConfig>
	{
		public static SkillAddStatusEffectConfigCategory Instance;
		public SkillAddStatusEffectConfigCategory()
		{
			Instance = this;
		}
	}

	public partial class SkillAddStatusEffectConfig: IConfig
	{
		[BsonId]
		public int Id { get; set; }
		public string Type;
		public float Cooldown;
		public string Effects;
	}
}
