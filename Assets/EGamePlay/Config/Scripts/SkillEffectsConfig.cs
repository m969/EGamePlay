namespace ET
{
	[Config]
	public partial class SkillEffectsConfigCategory : ACategory<SkillEffectsConfig>
	{
		public static SkillEffectsConfigCategory Instance;
		public SkillEffectsConfigCategory()
		{
			Instance = this;
		}
	}

	public partial class SkillEffectsConfig: IConfig
	{
		public int Id { get; set; }
		public string EffectType;
		public string OwnerAbility;
		public string Target;
		public string TriggerType;
		public string TriggerParam;
		public string Probability;
		public string KV1;
		public string KV2;
		public string KV3;
		public string Param1;
		public string Param2;
	}
}
