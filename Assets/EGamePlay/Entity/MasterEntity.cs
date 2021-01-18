using System;
using System.Collections.Generic;


namespace EGamePlay
{
    public sealed class MasterEntity : Entity
    {
        public Dictionary<Type, List<Entity>> Entities { get; private set; } = new Dictionary<Type, List<Entity>>();
        public List<Component> AllComponents { get; private set; } = new List<Component>();
        public List<Component> RemoveComponents { get; private set; } = new List<Component>();
        public List<Component> AddComponents { get; private set; } = new List<Component>();


        public void Update()
        {
            //while (AddComponents.Count > 0)
            //{
            //    AllComponents.Add(AddComponents[0]);
            //    AddComponents.RemoveAt(0);
            //}
            //foreach (var item in AllComponents)
            //{
            //    if (item.IsDisposed)
            //    {
            //        RemoveComponents.Add(item);
            //        continue;
            //    }
            //    item.Update();
            //}
            if (AllComponents.Count == 0)
            {
                return;
            }
            for (int i = AllComponents.Count - 1; i >= 0; i--)
            {
                var item = AllComponents[i];
                if (item.IsDisposed)
                {
                    AllComponents.RemoveAt(i);
                    continue;
                }
                if (item.Disable)
                {
                    continue;
                }
                item.Update();
            }
            //if (RemoveComponents.Count > 0)
            //{
            //    foreach (var item in RemoveComponents)
            //    {
            //        AllComponents.Remove(item);
            //    }
            //    RemoveComponents.Clear();
            //}
        }
    }
}