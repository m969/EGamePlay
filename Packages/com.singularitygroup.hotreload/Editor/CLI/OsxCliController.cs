using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using SingularityGroup.HotReload.Editor.Semver;
using Debug = UnityEngine.Debug;

namespace SingularityGroup.HotReload.Editor.Cli {
    class OsxCliController : ICliController {
        Process process;

        public string BinaryFileName => "HotReload.app.zip";
        public string PlatformName => "osx-x64";
        public bool CanOpenInBackground => false;

        /// In MacOS 13 Ventura, our app cannot launch a terminal window.
        /// We use a custom app that launches HotReload server and shows it's output (just like a terminal would). 
        //  Including MacOS 12 Monterey as well so I can dogfood it -Troy
        private static bool UseCustomConsoleApp() => MacOSVersion.Value.Major >= 12;

        // dont use static because null comparison on SemVersion is broken
        private static readonly Lazy<SemVersion> MacOSVersion = new Lazy<SemVersion>(() => {
            //UnityHelper.OperatingSystem; // in Unity 2018 it returns 10.16 on monterey (no idea why)
            //Environment.OSVersion returns unix version like 21.x
            var startinfo = new ProcessStartInfo {
                FileName = "/usr/bin/sw_vers",
                Arguments = "-productVersion",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };
            var process = Process.Start(startinfo);

            string osVersion = process.StandardOutput.ReadToEnd().Trim();

            SemVersion macosVersion;
            if (SemVersion.TryParse(osVersion, out macosVersion)) {
                return macosVersion;
            }
            // should never happen
            Log.Warning("Failed to detect MacOS version, if Hot Reload fails to start, please contact support.");
            return SemVersion.None;
        });

        public async Task Start(StartArgs args) {
            // Unzip the .app.zip to temp folder .app
            var appExecutablePath = $"{args.executableTargetDir}/HotReload.app/Contents/MacOS/HotReload";
            var cliExecutablePath = $"{args.executableTargetDir}/HotReload.app/Contents/Resources/CodePatcherCLI";
            
            // ensure running on threadpool
            await ThreadUtility.SwitchToThreadPool();

            // executableTargetDir is versioned, so only need to extract once.
            if (!File.Exists(appExecutablePath)) {
                try {
                    // delete only the extracted app folder (must not delete downloaded zip which is in same folder)
                    Directory.Delete(args.executableTargetDir + "/HotReload.app", true);
                } catch (IOException) {
                    // ignore directory not found
                }
                Directory.CreateDirectory(args.executableTargetDir);
                UnzipMacOsPackage($"{args.executableTargetDir}/{BinaryFileName}", args.executableTargetDir + "/");
            }

            try {
                // Always stop first because rarely it has happened that the server process was still running after custom console closed.
                // Note: this will also stop Hot Reload started by other Unity projects.
                await Stop();
            } catch {
                // ignored
            }

            if (UseCustomConsoleApp()) {
                await StartCustomConsole(args, appExecutablePath);
            } else {
                await StartTerminal(args, cliExecutablePath);
            }
        }

        public Task StartCustomConsole(StartArgs args, string executablePath) {
            process = Process.Start(new ProcessStartInfo {
                // Path to the HotReload.app
                FileName = executablePath,
                Arguments = args.cliArguments,
                UseShellExecute = false,
            });
            return Task.CompletedTask;
        }

        public Task StartTerminal(StartArgs args, string executablePath) {
            var pidFilePath = CliUtils.GetPidFilePath(args.hotreloadTempDir);
            // To run in a Terminal window (so you can see compiler logs), we must put the arguments into a script file
            // and run the script in Terminal. Terminal.app does not forward the arguments passed to it via `open --args`.
            // *.command files are opened with the user's default terminal app.
            var executableScriptPath = Path.Combine(Path.GetTempPath(), "Start_HotReloadServer.command");
            // You don't need to copy the cli executable on mac
            // omit hashbang line, let shell use the default interpreter (easier than detecting your default shell beforehand)
            File.WriteAllText(executableScriptPath, $"echo $$ > \"{pidFilePath}\"" +
                                                    $"\ncd \"{Environment.CurrentDirectory}\"" + // set cwd because 'open' launches script with $HOME as cwd.
                                                    $"\n\"{executablePath}\" {args.cliArguments} || read");

            CliUtils.Chmod(executableScriptPath); // make it executable
            CliUtils.Chmod(executablePath); // make it executable

            Directory.CreateDirectory(args.hotreloadTempDir);
            Directory.CreateDirectory(args.executableTargetDir);
            Directory.CreateDirectory(args.cliTempDir);
            
            process = Process.Start(new ProcessStartInfo {
                FileName = "open",
                Arguments = $"{(args.createNoWindow ? "-gj" : "")} '{executableScriptPath}'",
                UseShellExecute = true,
            });

            if (process.WaitForExit(1000)) {
                if (process.ExitCode != 0) {
                    Log.Warning("Failed to the run the start server command. ExitCode={0}\nFilepath: {1}", process.ExitCode, executableScriptPath);
                }
            }
            else {
                process.EnableRaisingEvents = true;
                process.Exited += (_, __) => {
                    if (process.ExitCode != 0) {
                        Log.Warning("Failed to the run the start server command. ExitCode={0}\nFilepath: {1}", process.ExitCode, executableScriptPath);
                    }
                };
            }
            return Task.CompletedTask;
        }

        public async Task Stop() {
            // kill HotReload server process (on mac it has different pid to the window which started it)
            await RequestHelper.KillServer();

            // process.CloseMainWindow throws if proc already exited.
            // We rely on the pid file for killing the trampoline script (in-case script is just starting and HotReload server not running yet)
            process = null;
            CliUtils.KillLastKnownHotReloadProcess();
        }

        static void UnzipMacOsPackage(string zipPath, string unzippedFolderPath) {
            //Log.Info("UnzipMacOsPackage called with {0}\n workingDirectory = {1}", zipPath, unzippedFolderPath);
            if (!zipPath.EndsWith(".zip")) {
                throw new ArgumentException($"Expected to end with .zip, but it was: {zipPath}", nameof(zipPath));
            }

            if (!File.Exists(zipPath)) {
                throw new ArgumentException($"zip file not found {zipPath}", nameof(zipPath));
            }
            var processStartInfo = new ProcessStartInfo {
                FileName = "unzip",
                Arguments = $"-o \"{zipPath}\"",
                WorkingDirectory = unzippedFolderPath, // unzip extracts to working directory by default
                UseShellExecute = true,
                CreateNoWindow = true
            };

            Process process = Process.Start(processStartInfo);
            process.WaitForExit();
            if (process.ExitCode != 0) {
                throw new Exception($"unzip failed with ExitCode {process.ExitCode}");
            }
            //Log.Info($"did unzip to {unzippedFolderPath}");
            // Move the .app folder to unzippedFolderPath
            
            // find the .app directory which is now inside unzippedFolderPath directory
            var foundDirs = Directory.GetDirectories(unzippedFolderPath, "*.app", SearchOption.AllDirectories);
            var done = false;
            var destDir = unzippedFolderPath + "HotReload.app";
            foreach (var dir in foundDirs) {
                if (dir.EndsWith(".app")) {
                    done = true;
                    if (dir == destDir) {
                        // already in the right place
                        break;
                    }
                    Directory.Move(dir, destDir);
                    //Log.Info("Moved to " + destDir);
                    break;
                }
            }

            if (!done) {
                throw new Exception("Failed to find .app directory and move it to " + destDir);
            }
            //Log.Info($"did unzip to {unzippedFolderPath}");
        }
    }
}