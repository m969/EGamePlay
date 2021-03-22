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
		public int Id { get; set; }
		public string Name;
		public string Type;
		public string TargetSelect;
		public float Cooldown;
		public string Description;
		public string Effect1;
		public string Effect2;
		public string Effect3;
	}
}
