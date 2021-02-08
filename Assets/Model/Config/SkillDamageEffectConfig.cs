using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
	[Config]
	public partial class SkillDamageEffectConfigCategory : ACategory<SkillDamageEffectConfig>
	{
		public static SkillDamageEffectConfigCategory Instance;
		public SkillDamageEffectConfigCategory()
		{
			Instance = this;
		}
	}

	public partial class SkillDamageEffectConfig: IConfig
	{
		[BsonId]
		public int Id { get; set; }
		public string Name;
		public string Target;
		public float Probability;
		public string Type;
		public string ValueFormula;
	}
}
