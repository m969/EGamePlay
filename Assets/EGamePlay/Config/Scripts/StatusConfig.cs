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
		public string ActionControl;
		public string AttributeType;
		public string AttributeParams;
		public string Effect1;
		public string Effect2;
		public string Effect3;
	}
}
