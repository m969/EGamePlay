#if ENABLE_MONO && (DEVELOPMENT_BUILD || UNITY_EDITOR)
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SingularityGroup.HotReload {
    internal class ServerHandshake {
        public static readonly ServerHandshake I = new ServerHandshake();

        /// <summary>
        /// Not verified as compatible yet - need to do handshake
        /// </summary>
        private PatchServerInfo pendingServer;

        /// <summary>
        /// Handshake is complete. Player can connect to this server.
        /// </summary>
        private PatchServerInfo verifiedServer;

        private Task handshakeCheck;

        private CancellationTokenSource cts = new CancellationTokenSource();
        /// Track first handshake request after calling SetServerInfo.
        /// Sometimes and it can take 10-30 seconds and succeed.
        private TaskCompletionSource<Result> firstHandshake = new TaskCompletionSource<Result>();

        /// <remarks>Server info should be well known or a strong guess, not just a random ip address.</remarks>
        public Task<Result> SetServerInfo(PatchServerInfo serverInfo) {
            if (verifiedServer != null && serverInfo == verifiedServer) {
                return Task.FromResult(Result.Verified);
            }
            pendingServer = serverInfo;
            if (serverInfo != null) {
                Prompts.SetConnectionState(ConnectionSummary.Handshaking);
            }

            // disconnect
            verifiedServer = null;
            
            // cancel any ongoing RequestHandshake task
            firstHandshake.TrySetCanceled(cts.Token);
            firstHandshake = new TaskCompletionSource<Result>();
            cts.Cancel();
            cts = new CancellationTokenSource();
            if (serverInfo == null) return Task.FromResult(Result.None);
            return firstHandshake.Task;
        }

        /// Ensures a handshake request is running.
        public void CheckHandshake() {
            var serverToCheck = pendingServer;
            if (verifiedServer == null && serverToCheck != null) {
                if (handshakeCheck == null || handshakeCheck.IsCompleted) {
                    handshakeCheck = Task.Run(async () => {
                        try {
                            Log.Debug("Run RequestHandshake");
                            var results = await RequestHandshake(serverToCheck);
                            await ThreadUtility.SwitchToMainThread();
                            var decisionIsFinal = await VerifyResults(results, serverToCheck);
                            firstHandshake.TrySetResult(results); // VerifyResults() can also set it, this is the default fallback
                            if (decisionIsFinal) {
                                pendingServer = null;
                            }
                        } catch (Exception ex) {
                            Log.Exception(ex);
                        } finally {
                            // set as failed if wasnt set as true by above code
                            firstHandshake.TrySetResult(Result.None);
                        }
                    }, cts.Token);
                }
            }
        }

        /// <summary>
        /// Verify results of the handshake.
        /// </summary>
        /// <param name="results"></param>
        /// <param name="server"></param>
        /// <returns>True if the conclusion is final, otherwise false</returns>
        /// <remarks>
        /// Must be called on main thread because it uses Unity UI methods.
        /// </remarks>
        async Task<bool> VerifyResults(Result results, PatchServerInfo server) {
            if (results.HasFlag(Result.QuietWarning)) {
                // can handle here if needed later
            }
            if (results.HasFlag(Result.Verified)) {
                if (!firstHandshake.Task.IsCompleted) {
                    Prompts.SetConnectionState(ConnectionSummary.Connecting);
                }
                OnVerified(server);
                return true;
            }

            // handle objections in order of obviousness, most obvious goes first
            if (results.HasFlag(Result.DifferentProject)) {
                await Prompts.ShowQuestionDialog(new QuestionDialog.Config {
                    summary = "Hot Reload was started from a different project",
                    suggestion = "Please run Hot Reload from the matching Unity project",
                    continueButtonText = "OK",
                    cancelButtonText = null,
                });
                // they need to provide a new server info
                Prompts.SetConnectionState(ConnectionSummary.Cancelled);
                return true;
            }
            if (results.HasFlag(Result.DifferentCommit)) {
                Prompts.SetConnectionState(ConnectionSummary.DifferencesFound);
                bool yes = await Prompts.ShowQuestionDialog(new QuestionDialog.Config {
                    summary = "Editor and current build are on different commits",
                    suggestion = "This can cause errors when the build was made on an old commit.",
                    continueButtonText = "Connect",
                });
                if (yes) {
                    results |= Result.Verified;
                    Prompts.SetConnectionState(ConnectionSummary.Connecting);
                    firstHandshake.TrySetResult(results);
                    OnVerified(server);
                } else {
                    Prompts.SetConnectionState(ConnectionSummary.Cancelled);
                }
                // cancel -> tell them to provide a new server
                return true;
            }

            if (results.HasFlag(Result.TempError)) {
                // retry might work, its not over yet
                return false;
            }
            // at time of writing, code should never reach here. Adding new HandshakeResult flags should be handled above.
            Log.Debug("UNEXPECTED: VerifyResults continued into untested code: {0}", results);
            return true;
        }

        void OnVerified(PatchServerInfo serverToCheck) {
            verifiedServer = serverToCheck;
        }

        public bool TryGetVerifiedServer(out PatchServerInfo serverInfo) {
            // take verifiedServer
            var server = Interlocked.Exchange(ref verifiedServer, null);
            serverInfo = server;
            return serverInfo != null;
        }

        /// <summary>
        /// Result of a handshake with the remote Hot Reload instance.
        /// </summary>
        [Flags]
        public enum Result {
            None = 0,
            DifferentCommit  = 1 << 0,
            DifferentProject = 1 << 1,
            
            /// <summary>
            /// A temporary error occurred, retrying might work.
            /// </summary>
            TempError        = 1 << 2,
            
            /// <summary>
            /// Hot Reload is compiling, so we should wait a bit before trying again.
            /// </summary>
            WaitForCompiling = 1 << 3,
            
            [Obsolete("Not needed so far", true)]
            Placeholder2     = 1 << 4,

            // use when a warning is logged, but we're allowing Hot Reload to connect bcus it probably works.
            QuietWarning     = 1 << 5,
            Verified         = 1 << 6,
        }
        
        static async Task<Result> RequestHandshake(PatchServerInfo info) {
            var buildInfo = PlayerEntrypoint.PlayerBuildInfo;
            var results = Result.None;
            var verified = true;
            Log.Debug($"Comparing commits {buildInfo.commitHash} and {info.commitHash}");
            if (buildInfo.IsDifferentCommit(info.commitHash)) {
                results |= Result.DifferentCommit;
                verified = false;
            }
            // Check for health before sending handshake request
            // If health check fails UI updates faster
            var healthy = await ServerHealthCheck.CheckHealthAsync(info);
            if (!healthy) {
                Log.Debug("Won't send handshake request because server is not healhy");
                return results;
            }
            Log.Info("Request handshake to Hot Reload server with hostname: {0}", info.hostName);
            //Log.Debug("Handshake with projectOmissionRegex: \"{0}\"", buildInfo.projectOmissionRegex);
            var response = await RequestHelper.RequestHandshake(info, buildInfo.DefineSymbolsAsHashSet,
                buildInfo.projectOmissionRegex);
            if (response.error != null) {
                verified = false;
                Log.Debug($"RequestHandshake errored: {response.error}");
                if (response.error == Result.WaitForCompiling.ToString()) {
                    // WaitForCompiling is a temp error
                    results |= Result.WaitForCompiling;
                    results |= Result.TempError;
                } else {
                    results |= Result.TempError;
                }
            }

            if (response.data == null) {
                // need response data to continue
                verified = false;
                return results;
            }

            // handshake response is what we post to /files which is BuildInfo
            var remoteBuildTarget = response.data[nameof(BuildInfo.activeBuildTarget)] as string;
            var remoteCommitHash = response.data[nameof(BuildInfo.commitHash)] as string;
            var remoteProjectIdentifier = response.data[nameof(BuildInfo.projectIdentifier)] as string;
            if (buildInfo.IsDifferentCommit(remoteCommitHash)) {
                Log.Debug($"RequestHandshake server is on different commit {response.error}");
                results |= Result.DifferentCommit;
                verified = false;
            }

            if (remoteProjectIdentifier != buildInfo.projectIdentifier) {
                Log.Debug("RequestHandshake remote is using a different project identifier");
                results |= Result.DifferentProject;
                verified = false;
            }

            if (remoteBuildTarget == null) {
                // Should never happen. Server responsed with an error when no BuildInfo at all.
                Log.Warning("Server did not declare its current Unity activeBuildTarget in the handshake response. Will assume it is {0}.", buildInfo.activeBuildTarget);
                results |= Result.QuietWarning;
            } else if (remoteBuildTarget != buildInfo.activeBuildTarget) {
                Log.Warning("Your Unity project is running on {0}. You may need to switch it to {1} for Hot Reload to work.", remoteBuildTarget, buildInfo.activeBuildTarget);
                results |= Result.QuietWarning;
            }

            if (verified) {
                results |= Result.Verified;
            }
            return results;
        }
    }
}
#endif
