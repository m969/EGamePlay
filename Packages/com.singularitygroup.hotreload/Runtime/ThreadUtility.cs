#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace SingularityGroup.HotReload {
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    static class ThreadUtility {
        /// <summary>
        /// Run code on Unity's main thread
        /// </summary>
        /// <remarks>
        /// This field is set early in [InitializeOnLoadMethod] in the editor and [RuntimeInitializeOnLoad] in playmode / for player builds, so your code assume it is already set.
        /// </remarks>
#if UNITY_EDITOR
        static SynchronizationContext _cachedMainContext;
        public static SynchronizationContext MainContext
        {
            get {
                if(_cachedMainContext != null) {
                    return _cachedMainContext;
                }
                return EditorFallbackContext.I;
            }
            private set {
                _cachedMainContext = value;
            }
        }
        
        class EditorFallbackContext : SynchronizationContext {
            public static readonly EditorFallbackContext I = new EditorFallbackContext();
            EditorFallbackContext() { }
            
            public override void Send(SendOrPostCallback d, object state) {
                UnityEditor.EditorApplication.delayCall += () => d(state);
            }
            public override void Post(SendOrPostCallback d, object state) {
                UnityEditor.EditorApplication.delayCall += () => d(state);
            }
        }
#else
        public static SynchronizationContext MainContext {get; private set;}
#endif

        public static int mainThreadId {get; private set;}

#if UNITY_EDITOR
        static ThreadUtility() {
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitMainThread() {
#endif
            MainContext = SynchronizationContext.Current;
            mainThreadId = Thread.CurrentThread.ManagedThreadId;
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void InitEditor() {
            //trigger static constructor
        }
        
        public static bool ShouldLogException(Exception ex) {
            AggregateException agg;
            while((agg = ex as AggregateException) != null) {
                ex = agg.InnerException;
            }
            if(ex is ThreadAbortException) {
                return false;
            }
            return true;
        }
        
        public static void LogException(Exception ex, CancellationToken token = default(CancellationToken)) {
            if(ShouldLogException(ex) && !token.IsCancellationRequested) {
                Log.Exception(ex);
            }
        }
        
        public static void RunOnMainThread(Action action, CancellationToken token = default(CancellationToken)) {
            if(Thread.CurrentThread.ManagedThreadId == mainThreadId) {
                action();
            } else {
                MainContext.Post(_ => {
                    if(!token.IsCancellationRequested) {
                        action();
                    }
                }, null);
            }
        }
        
        public static SwitchToMainThreadAwaitable SwitchToMainThread() {
            return new SwitchToMainThreadAwaitable();
        }

        public static CancellableSwitchToMainThreadAwaitable SwitchToMainThread(CancellationToken token) {
            return new CancellableSwitchToMainThreadAwaitable(token);
        }
        
        public static SwitchToThreadPoolAwaitable SwitchToThreadPool() {
            return new SwitchToThreadPoolAwaitable();
        }

        public static CancellableSwitchToThreadPoolAwaitable SwitchToThreadPool(CancellationToken token) {
            return new CancellableSwitchToThreadPoolAwaitable(token);
        }
    }
    
    struct SwitchToMainThreadAwaitable {
        public Awaiter GetAwaiter() => new Awaiter();

        public struct Awaiter : ICriticalNotifyCompletion {
            static readonly SendOrPostCallback switchToCallback = Callback;
            
            public bool IsCompleted => Thread.CurrentThread.ManagedThreadId == ThreadUtility.mainThreadId;

            public void GetResult() { }

            public void OnCompleted(Action continuation) {
                ThreadUtility.MainContext.Post(switchToCallback, continuation);
            }

            public void UnsafeOnCompleted(Action continuation) {
                ThreadUtility.MainContext.Post(switchToCallback, continuation);
            }
            
            static void Callback(object state) {
                var continuation = (Action)state;
                continuation();
            }
        }
    }


    struct CancellableSwitchToMainThreadAwaitable {
        readonly CancellationToken token;
        public CancellableSwitchToMainThreadAwaitable(CancellationToken token) {
            this.token = token;
        }
        
        public Awaiter GetAwaiter() => new Awaiter(token);

        public struct Awaiter : ICriticalNotifyCompletion {
            readonly CancellationToken token;
            public Awaiter(CancellationToken token) {
                this.token = token;
            }
            
            public bool IsCompleted => Thread.CurrentThread.ManagedThreadId == ThreadUtility.mainThreadId;
            
            public void GetResult() { }

            public void OnCompleted(Action continuation) {
                UnsafeOnCompleted(continuation);
            }

            public void UnsafeOnCompleted(Action continuation) {
                var tokenCopy = this.token;
                ThreadUtility.MainContext.Post(o => {
                    if(!tokenCopy.IsCancellationRequested) {
                        continuation();
                    }
                }, null);
            }
        }
    }
    
    struct CancellableSwitchToThreadPoolAwaitable {
        readonly CancellationToken token;
        public CancellableSwitchToThreadPoolAwaitable(CancellationToken token) {
            this.token = token;
        }
        
        public Awaiter GetAwaiter() => new Awaiter(token);

        public struct Awaiter : ICriticalNotifyCompletion {
            readonly CancellationToken token;
            public Awaiter(CancellationToken token) {
                this.token = token;
            }
            public bool IsCompleted => false;
            public void GetResult() { }

            public void OnCompleted(Action continuation) {
                ThreadPool.UnsafeQueueUserWorkItem(Callback, continuation);
            }

            public void UnsafeOnCompleted(Action continuation) {
                ThreadPool.UnsafeQueueUserWorkItem(Callback, continuation);
            }

            void Callback(object state) {
                token.ThrowIfCancellationRequested();
                var continuation = (Action)state;
                continuation();
            }
        }
    }
     
    struct SwitchToThreadPoolAwaitable {
        public Awaiter GetAwaiter() => new Awaiter();

        public struct Awaiter : ICriticalNotifyCompletion {
            static readonly WaitCallback switchToCallback = Callback;

            public bool IsCompleted => false;
            public void GetResult() { }

            public void OnCompleted(Action continuation) {
                ThreadPool.UnsafeQueueUserWorkItem(switchToCallback, continuation);
            }

            public void UnsafeOnCompleted(Action continuation) {
                ThreadPool.UnsafeQueueUserWorkItem(switchToCallback, continuation);
            }

            static void Callback(object state) {
                var continuation = (Action)state;
                continuation();
            }
        }
    }
}
#endif
