#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SingularityGroup.HotReload {
    internal static class TaskExtensions {
        public static async void Forget(this Task task, CancellationToken token = new CancellationToken()) {
            try {
                await task;
                if(task.IsFaulted) {
                    throw task.Exception ?? new Exception("unknown exception " + task);
                }
                token.ThrowIfCancellationRequested();
            } 
            catch(OperationCanceledException) {
                // ignore
            } catch(Exception ex) {
                if(!token.IsCancellationRequested) {
                    Log.Exception(ex);
                }
            }
        }

        /// <summary>
        /// Blocks until condition is true or timeout occurs.
        /// </summary>
        /// <param name="condition">The break condition.</param>
        /// <param name="pollInterval">The frequency at which the condition will be checked.</param>
        /// <param name="timeoutMs">The timeout in milliseconds.</param>
        /// <returns>True on condition became true, False if timeouted</returns>
        // credit: https://stackoverflow.com/a/52357854/5921285
        public static async Task<bool> WaitUntil(Func<bool> condition, int timeoutMs = -1, int pollInterval = 33) {
            var waitTask = Task.Run(async () => {
                while (!condition()) await Task.Delay(pollInterval);
            });

            if (waitTask != await Task.WhenAny(waitTask,
                    Task.Delay(timeoutMs))) {
                // timed out
                return false;
            }
            return true;
        }
    }
}
#endif
