using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ET
{
	public abstract class ACategory : ISupportInitialize
	{
		public abstract Type ConfigType { get; }

        public virtual void BeginInit()
        {
        }

        public virtual void EndInit()
        {
        }
    }

	/// <summary>
	/// 管理该所有的配置
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ACategory<T> : ACategory where T : IConfig
	{
		protected Dictionary<int, T> dict;

		public override void BeginInit()
		{
			var typeName = typeof(T).Name;
			
			string configStr = EGamePlayInit.Instance.TypeConfigTexts[typeName].text;

			try
			{
                this.dict = JsonHelper.FromJson<Dictionary<int, T>>(configStr);
            }
			catch (Exception e)
			{
				throw new Exception($"parser json fail: {configStr}", e);
			}
		}

		public override Type ConfigType
		{
			get
			{
				return typeof(T);
			}
		}

		public override void EndInit()
		{
		}

		public T Get(int id)
		{
			T t;
			if (!this.dict.TryGetValue(id, out t))
			{
				throw new Exception($"not found config: {typeof(T)} id: {id}");
			}
			return t;
		}

		public Dictionary<int, T> GetAll()
		{
			return this.dict;
		}

		public T GetOne()
		{
			return this.dict.Values.First();
		}
	}
}