using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
	[Config]
	public partial class StatusDamageEffectConfigCategory : ACategory<StatusDamageEffectConfig>
	{
		public static StatusDamageEffectConfigCategory Instance;
		public StatusDamageEffectConfigCategory()
		{
			Instance = this;
		}
	}

	public partial class StatusDamageEffectConfig: IConfig
	{
		[BsonId]
		public int Id { get; set; }
		public string TriggerType;
		public float Probability;
		public string Type;
		public string ValueFormula;
	}
}
