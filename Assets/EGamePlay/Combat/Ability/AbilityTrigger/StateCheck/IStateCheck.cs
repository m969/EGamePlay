using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGamePlay.Combat
{
    public interface IStateCheck
    {
        bool IsInvert { get; }
        bool CheckWith(Entity target);
    }
}
