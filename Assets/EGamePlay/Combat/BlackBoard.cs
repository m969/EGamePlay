namespace EGamePlay.Combat
{
    using System.Collections.Generic;

    public class BlackBoard
    {
        class BlackboardItem
        {
            private object _value;

            public void SetValue(object v)
            {
                _value = v;
            }

            public T GetValue<T>()
            {
                return (T)_value;
            }
        }

        private Dictionary<string, BlackboardItem> _items;

        public BlackBoard()
        {
            _items = new Dictionary<string, BlackboardItem>();
        }

        public void SetValue(string key, object v)
        {
            BlackboardItem item;
            if (_items.ContainsKey(key) == false)
            {
                item = new BlackboardItem();
                _items.Add(key, item);
            }
            else
            {
                item = _items[key];
            }
            item.SetValue(v);
        }

        public T GetValue<T>(string key, T defaultValue)
        {
            if (_items.ContainsKey(key) == false)
            {
                return defaultValue;
            }
            return _items[key].GetValue<T>();
        }
    }
}