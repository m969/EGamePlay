namespace ET
{
	[Config]
	public partial class SkillAddStatusEffectConfigCategory : ACategory<SkillAddStatusEffectConfig>
	{
		public static SkillAddStatusEffectConfigCategory Instance;
		public SkillAddStatusEffectConfigCategory()
		{
			Instance = this;
		}
	}

	public partial class SkillAddStatusEffectConfig: IConfig
	{
		public int Id { get; set; }
		public string Name;
		public string Target;
		public string Probability;
		public string StatusID;
		public string Duration;
		public string Param1;
		public string Param2;
	}
}
