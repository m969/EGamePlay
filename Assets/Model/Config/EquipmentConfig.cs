using MongoDB.Bson.Serialization.Attributes;

namespace ET
{
	[Config]
	public partial class EquipmentConfigCategory : ACategory<EquipmentConfig>
	{
		public static EquipmentConfigCategory Instance;
		public EquipmentConfigCategory()
		{
			Instance = this;
		}
	}

	public partial class EquipmentConfig: IConfig
	{
		[BsonId]
		public int Id { get; set; }
		public string Name;
		public string Attribute;
		public float Value;
	}
}
