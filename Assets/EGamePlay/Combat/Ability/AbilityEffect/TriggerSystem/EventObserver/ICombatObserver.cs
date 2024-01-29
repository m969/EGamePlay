using Sirenix.OdinInspector;
using System;

namespace EGamePlay.Combat
{
    public interface ICombatObserver
    {
        void OnTrigger(Entity source);
    }
}