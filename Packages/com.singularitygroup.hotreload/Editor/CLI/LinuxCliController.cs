using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace SingularityGroup.HotReload.Editor.Cli {

    class LinuxCliController : ICliController {
        Process process;

        public string BinaryFileName => "CodePatcherCLI";
        public string PlatformName => "linux-x64";
        public bool CanOpenInBackground => true;

        public Task Start(StartArgs args) {
            var startScript = Path.Combine(args.executableSourceDir, "hotreload-start-script.sh");
            if (!File.Exists(startScript)) {
                throw new FileNotFoundException(startScript);
            }
            File.WriteAllText(startScript, File.ReadAllText(startScript).Replace("\r\n", "\n"));
            CliUtils.Chmod(startScript);

            var title = CodePatcher.TAG + "Server " + new DirectoryInfo(args.unityProjDir).Name;
            title = title.Replace(" ", "-");
            title = title.Replace("'", "");

            var cliargsfile = Path.GetTempFileName();
            File.WriteAllText(cliargsfile,args.cliArguments);
            var codePatcherProc = Process.Start(new ProcessStartInfo {
                FileName = startScript,
                Arguments =
                    $"--title \"{title}\""
                    + $" --executables-source-dir \"{args.executableSourceDir}\" "
                    + $" --executable-taget-dir \"{args.executableTargetDir}\""
                    + $" --pidfile \"{CliUtils.GetPidFilePath(args.hotreloadTempDir)}\""
                    + $" --cli-arguments-file \"{cliargsfile}\""
                    + $" --method-patch-dir \"{args.cliTempDir}\""
                    + $" --create-no-window \"{args.createNoWindow}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
            if (codePatcherProc == null) {
                if (File.Exists(cliargsfile)) {
                    File.Delete(cliargsfile);
                }
                throw new Exception("Could not start code patcher process.");
            }
            codePatcherProc.BeginErrorReadLine();
            codePatcherProc.BeginOutputReadLine();
            codePatcherProc.OutputDataReceived += (_, a) => {
            };
            // error data can also mean we kill the proc beningly
            codePatcherProc.ErrorDataReceived += (_, a) => {
            };
            process = codePatcherProc;
            return Task.CompletedTask;
        }

        public async Task Stop() {
            await RequestHelper.KillServer();
            try {
                // process.CloseMainWindow throws if proc already exited.
                // also we just rely on the pid file it is fine
                CliUtils.KillLastKnownHotReloadProcess();
            } catch {
                //ignored
            }
            process = null;
        }
    }
}
