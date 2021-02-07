using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
	[Config]
	public partial class SkillConfigCategory : ACategory<SkillConfig>
	{
		public static SkillConfigCategory Instance;
		public SkillConfigCategory()
		{
			Instance = this;
		}
	}

	public partial class SkillConfig: IConfig
	{
		[BsonId]
		public int Id { get; set; }
		public string Name;
		public string Type;
		public float Cooldown;
		public string Effects;
	}
}
