using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
	[Config]
	public partial class UnitConfigCategory : ACategory<UnitConfig>
	{
		public static UnitConfigCategory Instance;
		public UnitConfigCategory()
		{
			Instance = this;
		}
	}

	public partial class UnitConfig: IConfig
	{
		[BsonId]
		public int Id { get; set; }
		public string Name;
	}
}
