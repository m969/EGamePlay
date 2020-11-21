using System;
using System.Linq;
using System.Collections.Generic;

namespace EGamePlay
{
    public static class IdFactory
    {
        public static long BaseRevertTicks { get; set; }


        public static long NewInstanceId()
        {
            if (BaseRevertTicks == 0)
            {
                var now = DateTime.UtcNow.Ticks;
                var str = now.ToString().Reverse();
                BaseRevertTicks = long.Parse(string.Concat(str));
            }
            BaseRevertTicks++;
            return BaseRevertTicks;
        }
    }
}