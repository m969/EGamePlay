using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ET
{
	public abstract class ACategory : ISupportInitialize
	{
		public abstract Type ConfigType { get; }
		public string ConfigText { get; set; }


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
	public abstract class ACategory<T> : ACategory where T : class, IConfig
	{
		protected Dictionary<int, T> dict = new Dictionary<int, T>();
		protected Dictionary<string, T> nameDict;


		public override void BeginInit()
		{
			var typeName = typeof(T).Name;
			
			string configStr = ConfigText;

			try
			{
                var dit = JsonHelper.FromJson<Dictionary<string, T>>(configStr);
				dict.Clear();
				foreach (var item in dit)
                {
					dict.Add(int.Parse(item.Key), item.Value);
				}
            }
			catch (Exception e)
			{
				Log.Error(e);
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
				//throw new Exception($"not found config: {typeof(T)} id: {id}");
				return null;
			}
			return t;
		}

		public T GetByName(string name)
		{
			T t;
			if (!this.nameDict.TryGetValue(name, out t))
			{
				//throw new Exception($"not found config: {typeof(T)} name: {name}");
				return null;
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