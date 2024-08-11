namespace ET
{
	[Config]
	public partial class AbilityConfigCategory : ACategory<AbilityConfig>
	{
		public static AbilityConfigCategory Instance;
		public AbilityConfigCategory()
		{
			Instance = this;
		}
	}

	public partial class AbilityConfig: IConfig
	{
		public int Id { get; set; }
		public string KeyName;
		public string Name;
		public string Type;
		public string TargetGroup;
		public string TargetSelect;
		public float Cooldown;
		public string Description;
		public string BuffType;
		public string StatusSlot;
		public string CanStack;
	}
}
