
using System;
using System.ComponentModel;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace SingularityGroup.HotReload.Editor {
    internal static class GitUtil {
        /// <remarks>
        /// Fallback is PatchServerInfo.UnknownCommitHash
        /// </remarks>
        public static string GetShortCommitHashOrFallback(int timeoutAfterMillis = 5000) {
            var shortCommitHash = PatchServerInfo.UnknownCommitHash;
            
            var commitHash = GetShortCommitHashSafe(timeoutAfterMillis);
            // On MacOS GetShortCommitHash() returns 7 characters, on Windows it returns 8 characters.
            // When git command produced an unexpected result, use a fallback string
            if (commitHash != null && commitHash.Length >= 6) {
                shortCommitHash = commitHash.Length < 8 ? commitHash : commitHash.Substring(0, 8);
            }

            return shortCommitHash;
        }
        
        // only log exception once per domain reload, to prevent spamming the console
        private static bool loggedExceptionInGetShortCommitHashSafe = false;

        /// <summary>
        /// Get the git commit hash, returning null if it takes too long.
        /// </summary>
        /// <param name="timeoutAfterMillis"></param>
        /// <returns></returns>
        /// <remarks>
        /// This method is 'better safe than sorry' because we must not break the user's build.<br/>
        /// It is better to not know the commit hash than to fail the build.
        /// </remarks>
        private static string GetShortCommitHashSafe(int timeoutAfterMillis) {
            Process process = null;
            // Note: don't use ReadToEndAsync because waiting on that task blocks forever.
            try {
                process = StartGitCommand("log", " -n 1 --pretty=format:%h");
                var stdout = process.StandardOutput;
                if (process.WaitForExit(timeoutAfterMillis)) {
                    return stdout.ReadToEnd();
                } else {
                    // In a git repo with git lfs, git log can be blocked by waiting for switch branches / download lfs objects
                    // For that reason I disabled this warning log until a better solution is implemented (e.g. cache the commit and use cached if timeout).
                    // Log.Warning(
                    //     $"[{CodePatcher.TAG}] Timed out trying to get the git commit hash, HotReload will not warn you about" +
                    //     " a build connecting to a server running on a different commit (which is not supported)");
                    return null;
                }
            } catch (Win32Exception ex) {
                if (ex.NativeErrorCode == 2) {
                    // git not found, ignore because user doesn't use git for version control
                    return null;
                } else if (!loggedExceptionInGetShortCommitHashSafe) {
                    loggedExceptionInGetShortCommitHashSafe = true;
                    Debug.LogException(ex);
                } 
            } catch (Exception ex) {
                if (!loggedExceptionInGetShortCommitHashSafe) {
                    loggedExceptionInGetShortCommitHashSafe = true;
                    Log.Exception(ex);
                }
            } finally {
                if (process != null) {
                    process.Dispose();
                }
            }
            return null;
        }

        static Process StartGitCommand(string command, string arguments, Action<ProcessStartInfo> modifySettings = null) {
            var startInfo = new ProcessStartInfo("git", command + " " + arguments) {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            if (modifySettings != null) {
                modifySettings(startInfo);
            }
            return Process.Start(startInfo);
        }
    }
}