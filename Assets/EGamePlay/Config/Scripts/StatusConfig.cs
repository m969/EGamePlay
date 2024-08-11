namespace ET
{
	[Config]
	public partial class StatusConfigCategory : ACategory<StatusConfig>
	{
		public static StatusConfigCategory Instance;
		public StatusConfigCategory()
		{
			Instance = this;
		}
	}

	public partial class StatusConfig: IConfig
	{
		public int Id { get; set; }
		public string ID;
		public string Name;
		public string Type;
		public string StatusSlot;
		public string CanStack;
		public string Description;
	}
}
