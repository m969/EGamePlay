namespace ET
{
	[Config]
	public partial class SkillDamageEffectConfigCategory : ACategory<SkillDamageEffectConfig>
	{
		public static SkillDamageEffectConfigCategory Instance;
		public SkillDamageEffectConfigCategory()
		{
			Instance = this;
		}
	}

	public partial class SkillDamageEffectConfig: IConfig
	{
		public int Id { get; set; }
		public string Name;
		public string Target;
		public string Probability;
		public string Type;
		public string ValueFormula;
	}
}
