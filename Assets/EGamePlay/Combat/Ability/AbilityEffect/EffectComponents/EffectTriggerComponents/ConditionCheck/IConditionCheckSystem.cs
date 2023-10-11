using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGamePlay.Combat
{
    public interface IConditionCheckSystem
    {
        bool IsInvert { get; }
        bool CheckCondition(Entity target);
    }
}
