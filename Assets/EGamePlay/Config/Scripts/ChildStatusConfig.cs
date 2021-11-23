namespace ET
{
	[Config]
	public partial class ChildStatusConfigCategory : ACategory<ChildStatusConfig>
	{
		public static ChildStatusConfigCategory Instance;
		public ChildStatusConfigCategory()
		{
			Instance = this;
		}
	}

	public partial class ChildStatusConfig: IConfig
	{
		public int Id { get; set; }
		public string ID;
		public string ChildStatus1;
		public string Status1KV1;
		public string Status1KV2;
		public string ChildStatus2;
		public string Status2KV1;
		public string Status2KV2;
	}
}
