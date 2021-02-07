using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
	[Config]
	public partial class AddStatusEffectConfigCategory : ACategory<AddStatusEffectConfig>
	{
		public static AddStatusEffectConfigCategory Instance;
		public AddStatusEffectConfigCategory()
		{
			Instance = this;
		}
	}

	public partial class AddStatusEffectConfig: IConfig
	{
		[BsonId]
		public int Id { get; set; }
		public string Name;
		public string Type;
		public float Cooldown;
		public string Effects;
	}
}
