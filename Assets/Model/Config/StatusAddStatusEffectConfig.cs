using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
	[Config]
	public partial class StatusAddStatusEffectConfigCategory : ACategory<StatusAddStatusEffectConfig>
	{
		public static StatusAddStatusEffectConfigCategory Instance;
		public StatusAddStatusEffectConfigCategory()
		{
			Instance = this;
		}
	}

	public partial class StatusAddStatusEffectConfig: IConfig
	{
		[BsonId]
		public int Id { get; set; }
		public string Type;
		public float Cooldown;
		public string Effects;
	}
}
