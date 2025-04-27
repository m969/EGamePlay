using System;

namespace SingularityGroup.HotReload.Editor {
    static class ExponentialBackoff {
        
        public static TimeSpan GetTimeout(int attempt, int minBackoff = 250, int maxBackoff = 60000, int deltaBackoff = 400) {
            attempt = Math.Min(25, attempt); // safety to avoid overflow below

            var delta = (uint)(
                (Math.Pow(2.0, attempt) - 1.0)
                * deltaBackoff 
            );

            var interval = Math.Min(checked(minBackoff + delta), maxBackoff);
            return TimeSpan.FromMilliseconds(interval);
        }
    }
}