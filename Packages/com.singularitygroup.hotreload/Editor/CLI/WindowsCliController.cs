using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SingularityGroup.HotReload.Editor.Cli {
    class WindowsCliController : ICliController {
        Process process;

        public string BinaryFileName => "CodePatcherCLI.exe";
        public string PlatformName => "win-x64";
        public bool CanOpenInBackground => true;

        public Task Start(StartArgs args) {
            process = Process.Start(new ProcessStartInfo {
                FileName = Path.GetFullPath(Path.Combine(args.executableTargetDir, "CodePatcherCLI.exe")),
                Arguments = args.cliArguments,
                UseShellExecute = !args.createNoWindow,
                CreateNoWindow = args.createNoWindow,
            });
            return Task.CompletedTask;
        }

        public async Task Stop() {
            await RequestHelper.KillServer();
            try {
                process?.CloseMainWindow();
            } catch {
                //ignored
            }  
            process = null;
        }
    }
}