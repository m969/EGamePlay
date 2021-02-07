using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
	[Config]
	public partial class DamageEffectConfigCategory : ACategory<DamageEffectConfig>
	{
		public static DamageEffectConfigCategory Instance;
		public DamageEffectConfigCategory()
		{
			Instance = this;
		}
	}

	public partial class DamageEffectConfig: IConfig
	{
		[BsonId]
		public int Id { get; set; }
		public int Skill;
		public string Target;
		public float Probability;
		public string Type;
		public string ValueFormula;
	}
}
