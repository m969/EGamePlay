using System;
using System.Collections.Generic;


namespace EGamePlay
{
    public sealed class MasterEntity : Entity
    {
        public Dictionary<Type, List<Entity>> Entities { get; private set; } = new Dictionary<Type, List<Entity>>();
        public List<Component> AllComponents { get; private set; } = new List<Component>();
        public List<UpdateComponent> UpdateComponents { get; private set; } = new List<UpdateComponent>();
        public static MasterEntity Instance { get; private set; }


        private MasterEntity()
        {
            
        }

        public static MasterEntity Create()
        {
            if (Instance == null)
            {
                Instance = new MasterEntity();
#if !NOT_UNITY
                Instance.AddComponent<GameObjectComponent>();
#endif
            }
            return Instance;
        }

        public static void Destroy()
        {
            Instance = null;
        }

        public override void Update()
        {
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
        }
    }
}